using System.Collections.Generic;
using UnityEngine;

public enum Target
{
    Player,
    Wizard
}

public class ProbabilityManager : MonoBehaviour
{
    [SerializeField] private MageDifficultyConfig config;
    [SerializeField] private int copiesPerValue = 4; // cuántas copias de cada carta (1–11)

    private List<int> remainingCards;

    private void Awake()
    {
        BuildDeck();
    }

    /// <summary>
    /// Reconstruye el mazo completo (por ejemplo, al inicio de la partida o de una ronda).
    /// </summary>
    public void BuildDeck()
    {
        remainingCards = new List<int>();

        for (int c = 0; c < copiesPerValue; c++)
        {
            for (int value = 1; value <= 11; value++)
            {
                remainingCards.Add(value);
            }
        }
    }

    /// <summary>
    /// Devuelve el valor de la carta, considerando:
    /// - Quién roba (Player / Wizard)
    /// - Si los ojos del jugador están abiertos
    /// - El total actual del que roba
    /// - Las cartas que quedan en el mazo
    /// </summary>
    public int GetNextCard(Target target, bool playerEyesOpen, int currentTotal)
    {
        if (remainingCards == null || remainingCards.Count == 0)
        {
            Debug.LogWarning("Mazo vacío, reconstruyendo deck...");
            BuildDeck();
        }

        // 1. Probabilidad de carta buena según dificultad
        float probGood;

        if (target == Target.Player)
        {
            probGood = playerEyesOpen
                ? config.goodCard_Player_EyesOpen
                : config.goodCard_Player_EyesClosed;
        }
        else // Target.Wizard
        {
            probGood = playerEyesOpen
                ? config.goodCard_Wizard_EyesOpen
                : config.goodCard_Wizard_EyesClosed;
        }

        bool wantGoodCard = Random.value < probGood;

        // 2. Construir pools de cartas buenas y malas según el total actual
        List<int> goodPool = GetGoodPool(currentTotal);
        List<int> badPool = GetBadPool(currentTotal);

        // Asegurarnos de que al menos tengamos algún pool con cartas
        if (goodPool.Count == 0 && badPool.Count == 0)
        {
            // Si por alguna razón no hay pools válidos, elegimos cualquiera del mazo
            int fallbackIndex = Random.Range(0, remainingCards.Count);
            int fallbackCard = remainingCards[fallbackIndex];
            remainingCards.RemoveAt(fallbackIndex);
            return fallbackCard;
        }

        List<int> chosenPool;

        if (wantGoodCard && goodPool.Count > 0)
        {
            chosenPool = goodPool;
        }
        else if (!wantGoodCard && badPool.Count > 0)
        {
            chosenPool = badPool;
        }
        else
        {
            // Si el pool "ideal" está vacío, usamos el otro
            chosenPool = goodPool.Count > 0 ? goodPool : badPool;
        }

        // 3. Elegimos una carta al azar dentro del pool elegido
        int poolIndex = Random.Range(0, chosenPool.Count);
        int chosenValue = chosenPool[poolIndex];

        // 4. Quitamos ESA carta específica del mazo
        int indexInDeck = remainingCards.IndexOf(chosenValue);
        if (indexInDeck >= 0)
        {
            remainingCards.RemoveAt(indexInDeck);
        }
        else
        {
            Debug.LogWarning("Carta elegida no encontrada en el mazo. Esto no debería pasar.");
        }

        return chosenValue;
    }

    /// <summary>
    /// Devuelve las cartas del mazo que son "buenas" dado el total actual.
    /// </summary>
    private List<int> GetGoodPool(int total)
    {
        List<int> pool = new List<int>();

        foreach (int card in remainingCards)
        {
            // Lógica simple: buena = ayuda a acercarse a 21 sin matar
            int newTotal = total + card;

            // Consideramos "buena" la carta que:
            // - no te hace pasarte de 21
            // - te acercan decentemente 
            if (newTotal <= 21 && (card >= 3 || total <= 11))
            {
                pool.Add(card);
            }
        }

        // Si ninguna cumple ese criterio, aflojamos la condición:
        if (pool.Count == 0)
        {
            foreach (int card in remainingCards)
            {
                int newTotal = total + card;
                if (newTotal <= 21)
                    pool.Add(card);
            }
        }

        return pool;
    }

    /// <summary>
    /// Devuelve las cartas del mazo que son "malas" dado el total actual.
    /// </summary>
    private List<int> GetBadPool(int total)
    {
        List<int> pool = new List<int>();

        foreach (int card in remainingCards)
        {
            int newTotal = total + card;

            // Consideramos "mala" la carta que:
            // - te hace pasarte de 21
            // - o casi no mejora tu situación 
            bool busts = newTotal > 21;
            bool uselessWhenLow = total <= 11 && card <= 2;

            if (busts || uselessWhenLow)
            {
                pool.Add(card);
            }
        }

       
        if (pool.Count == 0)
        {
            foreach (int card in remainingCards)
            {
                int newTotal = total + card;

                
                if (newTotal > 18)
                    pool.Add(card);
            }
        }

        return pool;
    }
}

