using System.Collections.Generic;
using UnityEngine;

public class Wizard : Gambler
{
    [SerializeField] private Player player;
    [SerializeField] private ProbabilityManager probabilityManager;
    [SerializeField] private MageDifficultyConfig difficulty;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck = new List<int>();
        totalCardsValue = 0;
        
        // Inicializar componentes de dificultad si no están asignados
        InitializeDifficultyComponents();
    }
    
    /// <summary>
    /// Inicializa los componentes de dificultad si no están asignados en el Inspector.
    /// </summary>
    private void InitializeDifficultyComponents()
    {
        // Buscar Player si no está asignado
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
            if (player == null)
            {
                Debug.LogError("Wizard: No se encontró el componente Player!");
            }
            else
            {
                Debug.Log("Wizard: Player encontrado automáticamente.");
            }
        }
        
        // Buscar ProbabilityManager si no está asignado
        if (probabilityManager == null)
        {
            probabilityManager = GetComponent<ProbabilityManager>();
            if (probabilityManager == null)
            {
                probabilityManager = FindFirstObjectByType<ProbabilityManager>();
            }
            if (probabilityManager == null)
            {
                Debug.LogWarning("Wizard: No se encontró ProbabilityManager. El mago funcionará sin influencia de dificultad en las cartas.");
            }
            else
            {
                Debug.Log("Wizard: ProbabilityManager encontrado automáticamente.");
            }
        }
        
        // Buscar MageDifficultyConfig si no está asignado
        // Intentar obtenerlo del ProbabilityManager si lo tiene
        if (difficulty == null && probabilityManager != null)
        {
            // Usar reflexión para acceder al config del ProbabilityManager
            // O mejor: agregar un método público en ProbabilityManager
            Debug.LogWarning("Wizard: MageDifficultyConfig no está asignado directamente. Asegúrate de asignarlo en el Inspector para que el mago use la dificultad correcta.");
        }
        
        // Validar que los componentes críticos estén asignados
        if (player == null)
        {
            Debug.LogError("Wizard: Player no está asignado y no se pudo encontrar. El mago no funcionará correctamente.");
        }
        
        // Log de estado final
        Debug.Log($"Wizard inicializado - Player: {(player != null ? "✓" : "✗")}, ProbabilityManager: {(probabilityManager != null ? "✓" : "✗")}, Difficulty: {(difficulty != null ? "✓" : "✗")}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PlayTurn()
    {
        Debug.Log("Wizard Turn");
        bool eyesOpen = player.areEyesOpen;
        int mageTotal = totalCardsValue;
        int playerVisible = eyesOpen ? player.TotalCardsValue : EstimatePlayerValue();

        // bool playerHasActedBeforeMage = gameManager.PlayerActedThisCycle;
        bool doHit = DecideAction(mageTotal, playerVisible, eyesOpen);

        if (doHit)
            DrawCard();
        else
            Pass();
        
        GameManager.Instance.SetEndofTurn();
    }

    public override void DrawCard()
    {
        // Usar GameManager para robar carta (esto también verifica bust y registra decisiones)
        GameManager.Instance.DealCard(Turn.WizardTurn);
        gamblerChoice = GamblerChoice.Draw;
        Debug.Log($"Mago robó carta. Nuevo total: {totalCardsValue}");
    }

    public override void Pass()
    {
        gamblerChoice = GamblerChoice.Pass;
        Debug.Log("Mago pasa.");
    }

    public int TotalCardsValue => totalCardsValue;

    private bool DecideAction(int mageTotal, int playerVisible, bool eyesOpen)
    {
        // REGLA CRÍTICA: Si el mago tiene 21 o más, SIEMPRE debe pasar
        // Robar otra carta solo puede empeorar su situación (bust o igual)
        if (mageTotal >= 21)
        {
            Debug.Log($"Mago tiene {mageTotal}. Pasando automáticamente (no puede mejorar robando).");
            return false; // Pass
        }
        
        // Si tiene 20, también debería pasar (solo puede empeorar)
        if (mageTotal == 20)
        {
            Debug.Log($"Mago tiene 20. Pasando (solo puede empeorar robando).");
            return false; // Pass
        }
        
        // CASO 1: JUGADOR CON OJOS ABIERTOS
        if (eyesOpen)
        {
            int diff = playerVisible - mageTotal;

            if (diff >= 3)
            {
                return Random.value < 0.8f; // 80% Hit
            }

            else if (diff <= -2)
            {
                return Random.value < 0.2f; // 20% Hit
            }
            else
            {
                return mageTotal < 17; // Hit hasta 16, Pass desde 17
            }
        }

        // CASO 2: JUGADOR CON OJOS CERRADOS 

        bool decisionBase;

        
        if (mageTotal <= 11)
        {
            decisionBase = true;
        }
        
        else if (mageTotal >= 19)
        {
            decisionBase = false;
        }
        else
        {
            // Nivel de audacia del mago
            float boldness = difficulty != null ? difficulty.boldness : 0.3f;

            
            float t = Mathf.InverseLerp(12f, 18f, mageTotal);
           
            float hitProbability = Mathf.Lerp(1f, 0f, t);
            
            hitProbability *= (0.5f + boldness);
           
            hitProbability = Mathf.Clamp01(hitProbability);

            decisionBase = Random.value < hitProbability;
        }

        // Nivel de dificultad: posibilidad de error cuando el jugador cierra los ojos
        float mistakeChance = difficulty != null ? difficulty.mistakeChanceEyesClosed : 0.2f;

        if (Random.value < mistakeChance)
        {
            decisionBase = !decisionBase;
        }

        return decisionBase;
    }


    private int EstimatePlayerValue()
    {
        int count = player.deckLength; 
        int estimate;

        switch (count)
        {
            case 1:
                estimate = Random.Range(3, 10);
                break;

            case 2:
                estimate = Random.Range(8, 15);
                break;

            case 3:
                estimate = Random.Range(12, 18);
                break;

            default:
                estimate = Random.Range(15, 21);
                break;
        }


        estimate += Random.Range(-1, 2);
        estimate = Mathf.Clamp(estimate, 4, 21);

        return estimate;
    }
    

}

