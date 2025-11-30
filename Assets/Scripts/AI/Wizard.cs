using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Gambler
{
    [SerializeField] private Player player;

    [Header("Personalidad del Mago")]
    [Range(0f, 1f)] public float boldness = 0.5f; // Qué tan arriesgado es (0 = conservador, 1 = agresivo)
    [Range(0f, 1f)] public float mistakeChanceEyesClosed = 0.15f; // Probabilidad de error con ojos cerrados
    [Range(0f, 1f)] public float bluffDetectionChance = 0.3f; // Probabilidad de detectar un bluff del jugador

    [Header("Debug")]
    [SerializeField] private bool showDecisionReasoning = true;

    private bool isTakingTurn = false;

    void Start()
    {
        deck = new List<int>();
        totalCardsValue = 0;

        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }
    }

    public override void PlayTurn()
    {
        if (!isTakingTurn)
        {
            Debug.Log("Wizard Turn");
            for (int i = 0; i < deck.Count; i++)
            {
                Debug.Log(deck[i]);
            }
            StartCoroutine(WizardTurnRoutine());
        }
    }

    private IEnumerator WizardTurnRoutine()
    {
        isTakingTurn = true;
        
        Debug.Log($"=== TURNO DEL MAGO === (Total actual: {totalCardsValue})");
        
        // Simular "pensamiento" del mago
        float waitTime = Random.Range(1.5f, 3f);
        yield return new WaitForSeconds(waitTime);

        bool eyesOpen = player.areEyesOpen;
        int mageTotal = totalCardsValue;
        int playerVisible = eyesOpen ? player.TotalCardsValue : EstimatePlayerValue();

        bool doHit = DecideAction(mageTotal, playerVisible, eyesOpen);

        if (doHit)
        {
            DrawCard();
        }
        else
        {
            Pass();
        }

        GameManager.Instance.SetEndofTurn();
        isTakingTurn = false;
    }

    public override void DrawCard()
    {
        gamblerChoice = GamblerChoice.Draw;
        int previousTotal = totalCardsValue;
        
        GameManager.Instance.DealCard(Turn.WizardTurn);
        
        Debug.Log($"Mago robó carta: {totalCardsValue - previousTotal} (Total: {previousTotal} → {totalCardsValue})");
        
        if (totalCardsValue > 21)
        {
            Debug.Log("¡Mago se pasó de 21!");
        }
    }

    public override void Pass()
    {
        gamblerChoice = GamblerChoice.Pass;
        Debug.Log($"Mago pasa (Total: {totalCardsValue})");
    }

    /// <summary>
    /// IA mejorada del mago con múltiples factores de decisión.
    /// </summary>
    private bool DecideAction(int mageTotal, int playerVisible, bool eyesOpen)
    {
        string reasoning = "";
        
        // === REGLA 0: Nunca robar si ya tienes 21 ===
        if (mageTotal == 21)
        {
            reasoning = "Ya tengo 21 perfecto";
            LogDecision(false, reasoning, mageTotal, playerVisible, eyesOpen);
            return false;
        }
        
        // === REGLA 1: Si me pasé o estoy en 20+, evaluar más cuidadosamente ===
        if (mageTotal >= 20)
        {
            // Si el jugador tiene más que yo y estoy cerca, arriesgarme
            if (eyesOpen && playerVisible == 21)
            {
                reasoning = "Jugador tiene 21, debo arriesgarme";
                LogDecision(true, reasoning, mageTotal, playerVisible, eyesOpen);
                return Random.value < (boldness * 0.5f); // Arriesgado pero basado en boldness
            }
            else
            {
                reasoning = "Tengo 20+, muy arriesgado robar";
                LogDecision(false, reasoning, mageTotal, playerVisible, eyesOpen);
                return false;
            }
        }
        
        // === CASO: OJOS ABIERTOS (información perfecta) ===
        if (eyesOpen)
        {
            return DecideWithOpenEyes(mageTotal, playerVisible, out reasoning);
        }
        
        // === CASO: OJOS CERRADOS (información imperfecta) ===
        return DecideWithClosedEyes(mageTotal, playerVisible, out reasoning);
    }

    /// <summary>
    /// Decisión cuando el jugador tiene ojos ABIERTOS (información perfecta).
    /// </summary>
    private bool DecideWithOpenEyes(int mageTotal, int playerTotal, out string reasoning)
    {
        int diff = playerTotal - mageTotal;
        
        // Si el jugador se pasó de 21, jugar conservador
        if (playerTotal > 21)
        {
            reasoning = "Jugador se pasó, juego conservador";
            LogDecision(mageTotal < 17, reasoning, mageTotal, playerTotal, true);
            return mageTotal < 17;
        }
        
        // Si estoy muy por debajo (diferencia >= 4), robar agresivamente
        if (diff >= 4)
        {
            reasoning = $"Voy {diff} puntos atrás, debo robar";
            LogDecision(true, reasoning, mageTotal, playerTotal, true);
            return Random.value < (0.7f + boldness * 0.3f); // 70-100% chance
        }
        
        // Si estoy ligeramente atrás (diferencia 1-3)
        if (diff > 0)
        {
            float hitChance = Mathf.Lerp(0.6f, 0.3f, diff / 3f) + (boldness * 0.2f);
            reasoning = $"Voy {diff} puntos atrás, robar con {hitChance * 100:F0}% probabilidad";
            bool decision = Random.value < hitChance;
            LogDecision(decision, reasoning, mageTotal, playerTotal, true);
            return decision;
        }
        
        // Si estoy empatado o ligeramente adelante (diferencia 0 a -2)
        if (diff >= -2)
        {
            reasoning = "Estoy cerca, jugar seguro";
            bool decision = mageTotal < 17;
            LogDecision(decision, reasoning, mageTotal, playerTotal, true);
            return decision;
        }
        
        // Si estoy muy adelante (diferencia <= -3), jugar ultra conservador
        reasoning = $"Voy {-diff} puntos adelante, jugar conservador";
        bool shouldHit = mageTotal < 15 && Random.value < (0.3f - boldness * 0.1f);
        LogDecision(shouldHit, reasoning, mageTotal, playerTotal, true);
        return shouldHit;
    }

    /// <summary>
    /// Decisión cuando el jugador tiene ojos CERRADOS (información imperfecta).
    /// </summary>
    private bool DecideWithClosedEyes(int mageTotal, int playerEstimate, out string reasoning)
    {
        bool baseDecision;
        
        // Estrategia base según mi total
        if (mageTotal <= 11)
        {
            reasoning = "Tengo 11 o menos, imposible pasarme";
            baseDecision = true;
        }
        else if (mageTotal >= 19)
        {
            reasoning = "Tengo 19+, muy arriesgado";
            baseDecision = false;
        }
        else if (mageTotal >= 17)
        {
            // Entre 17-18: depende de boldness y estimación del jugador
            float hitProb = 0.3f * boldness;
            
            if (playerEstimate > mageTotal + 2)
            {
                hitProb += 0.3f; // Aumentar si creo que voy atrás
            }
            
            reasoning = $"Tengo {mageTotal}, estimación jugador: {playerEstimate}, robar con {hitProb * 100:F0}%";
            baseDecision = Random.value < hitProb;
        }
        else // 12-16
        {
            // Zona intermedia: interpolación suave
            float t = Mathf.InverseLerp(12f, 16f, mageTotal);
            float hitProb = Mathf.Lerp(0.8f, 0.4f, t);
            hitProb *= (0.7f + boldness * 0.6f); // Ajustar por boldness
            
            reasoning = $"Tengo {mageTotal}, robar con {hitProb * 100:F0}% probabilidad";
            baseDecision = Random.value < hitProb;
        }
        
        // Aplicar posibilidad de error/confusión
        if (Random.value < mistakeChanceEyesClosed)
        {
            baseDecision = !baseDecision;
            reasoning += " (pero cometí un error!)";
        }
        
        LogDecision(baseDecision, reasoning, mageTotal, playerEstimate, false);
        return baseDecision;
    }

    /// <summary>
    /// Estima el valor del jugador cuando tiene los ojos cerrados.
    /// Usa el número de cartas y algo de aleatoriedad.
    /// </summary>
    private int EstimatePlayerValue()
    {
        int cardCount = player.deckLength;
        int estimate;
        
        // Estimación base según número de cartas
        switch (cardCount)
        {
            case 1:
                estimate = Random.Range(4, 11);
                break;
            case 2:
                estimate = Random.Range(9, 16);
                break;
            case 3:
                estimate = Random.Range(13, 19);
                break;
            case 4:
                estimate = Random.Range(16, 21);
                break;
            default:
                estimate = Random.Range(18, 22);
                break;
        }
        
        // Añadir variación según capacidad de "intuición"
        int variance = Random.Range(-2, 3);
        estimate += variance;
        
        // Detectar posibles bluffs (si el jugador pasó muy temprano)
        if (player.gamblerChoice == GamblerChoice.Pass && cardCount <= 2)
        {
            if (Random.value < bluffDetectionChance)
            {
                estimate -= 3; // Asumir que tiene menos de lo esperado
                Debug.Log($"¡Mago sospecha de bluff! Reduciendo estimación en 3 puntos");
            }
        }
        
        estimate = Mathf.Clamp(estimate, 4, 24);
        
        Debug.Log($"Estimación del jugador: {estimate} (Cartas: {cardCount})");
        return estimate;
    }

    /// <summary>
    /// Log de decisiones del mago para debugging.
    /// </summary>
    private void LogDecision(bool willHit, string reasoning, int mageTotal, int playerValue, bool eyesOpen)
    {
        if (!showDecisionReasoning) return;
        
        string decision = willHit ? "ROBAR" : "PASAR";
        string playerInfo = eyesOpen ? $"Real: {playerValue}" : $"Estimado: {playerValue}";
        
        Debug.Log($"[Mago Decide] {decision} | Mi total: {mageTotal} | Jugador {playerInfo} | Razón: {reasoning}");
    }
}