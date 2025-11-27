using UnityEngine;

public class CardQualityEvaluator : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
    /// <summary>
    /// Evalúa si una carta es "buena" para el jugador dado su total actual.
    /// Este método se usa ANTES de que el jugador elija si robar o pasar,
    /// para determinar qué sonido reproducir (ligero = buena, pesado = mala).
    /// Usa la misma lógica que ProbabilityManager.
    /// </summary>
    /// <param name="cardValue">El valor de la carta que se está considerando</param>
    /// <param name="currentTotal">El total actual del jugador ANTES de recibir la carta</param>
    /// <returns>True si la carta es buena, False si es mala</returns>
    public bool IsGoodCard(int cardValue, int currentTotal)
    {
        int newTotal = currentTotal + cardValue;
        
        // Carta es BUENA si:
        // 1. No te hace pasarte de 21
        // 2. Te ayuda a acercarte decentemente (carta >= 3, o si tu total es bajo <= 11)
        bool doesNotBust = newTotal <= 21;
        bool helpsDecently = (cardValue >= 3) || (currentTotal <= 11);
        
        if (doesNotBust && helpsDecently)
        {
            return true;
        }
        
        if (doesNotBust)
        {
            if (currentTotal <= 11)
            {
                return true;
            }
            return cardValue >= 3;
        }
        
        return false;
    }
    
    public bool IsBadCard(int cardValue, int currentTotal)
    {
        return !IsGoodCard(cardValue, currentTotal);
    }
    
    public string GetCardQualityDescription(int cardValue, int currentTotal)
    {
        bool isGood = IsGoodCard(cardValue, currentTotal);
        int newTotal = currentTotal + cardValue;
        
        if (newTotal > 21)
        {
            return "BUST - Te pasas de 21";
        }
        else if (isGood)
        {
            return "BUENA - Te ayuda a acercarte a 21";
        }
        else
        {
            return "MALA - No te ayuda mucho";
        }
    }
}
