using UnityEngine;

public class Player : Gambler
{
    [SerializeField] private EyesController eyesController;
    [SerializeField] private PlayerUIManager uiManager;
    
    public int TotalCardsValue => totalCardsValue;
    public int deckLength => deck != null ? deck.Count : 0;

    // Sincronizar con EyesController
    public bool areEyesOpen 
    { 
        get 
        { 
            if (eyesController != null)
                return eyesController.AreEyesOpen;
            return true; // Por defecto ojos abiertos
        } 
    }

    private void Awake()
    {
        if (eyesController == null)
        {
            eyesController = GetComponent<EyesController>();
        }

        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<PlayerUIManager>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (eyesController == null)
        {
            eyesController = GetComponent<EyesController>();
        }

        if (uiManager == null)
        {
            uiManager = FindFirstObjectByType<PlayerUIManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Ejecuta el turno del jugador.
    /// Flujo:
    /// 1. Jugador decide abrir o cerrar los ojos (al inicio del turno)
    /// 2. Jugador decide robar carta o pasar
    /// 3. Si elige robar:
    ///    - Si ojos cerrados: escucha sonido (ligero/pesado) antes de decidir
    ///    - Si ojos abiertos: ve la carta directamente
    /// 4. Jugador confirma o cancela
    /// </summary>
    public override void PlayTurn()
    {
        Debug.Log("=== TURNO DEL JUGADOR ===");
        
        // Iniciar la UI del turno
        if (uiManager != null)
        {
            uiManager.StartPlayerTurn();
        }
        else
        {
            Debug.LogWarning("PlayerUIManager no encontrado. El jugador no podrá interactuar.");
        }
        
        // El turno continuará cuando el jugador tome una decisión a través de la UI
        // Los métodos DrawCard() o Pass() serán llamados desde la UI
    }

    /// <summary>
    /// Método llamado cuando el jugador quiere robar una carta.
    /// Primero muestra el preview (sonido si ojos cerrados, carta si ojos abiertos),
    /// luego espera confirmación del jugador.
    /// </summary>
    public void RequestDrawCard()
    {
        Debug.Log("Jugador quiere robar carta...");
        
        var (cardValue, isFavorable) = GameManager.Instance.GetCardPreview();
        
        if (areEyesOpen)
        {
            // Ojos abiertos: mostrar la carta directamente
            int newTotal = totalCardsValue + cardValue;
            Debug.Log($"Carta disponible: {cardValue} | Nuevo total sería: {newTotal}");
            
            // NOTA: Aquí la UI debe mostrar la carta y esperar confirmación
            // Por ahora, asumimos que el jugador acepta si no se pasa de 21
            if (newTotal <= 21)
            {
                ConfirmDrawCard();
            }
            else
            {
                Debug.Log("La carta te haría pasarte de 21. Cancelando...");
                // El jugador puede decidir pasar o intentar otra vez
            }
        }
        else
        {
            // Ojos cerrados: el sonido ya se reprodujo en GetCardPreview()
            Debug.Log($"Sonido reproducido. Carta es {(isFavorable ? "FAVORABLE" : "NO FAVORABLE")}");
            
            // NOTA: Aquí la UI debe esperar a que el jugador confirme o cancele
            // Por ahora, asumimos que acepta si es favorable
            if (isFavorable)
            {
                ConfirmDrawCard();
            }
            else
            {
                Debug.Log("Carta no favorable. El jugador puede cancelar o aceptar.");
                // El jugador puede decidir pasar o intentar otra vez
            }
        }
    }

    /// <summary>
    /// Confirma y roba la carta después del preview.
    /// </summary>
    public void ConfirmDrawCard()
    {
        DrawCard();
    }

    /// <summary>
    /// Cancela la acción de robar carta.
    /// </summary>
    public void CancelDrawCard()
    {
        Debug.Log("Jugador canceló robar carta.");
        // El jugador puede intentar pasar o robar otra vez
    }

    public override void DrawCard()
    {
        Debug.Log("Jugador roba carta...");
        GameManager.Instance.DealCard(Turn.PlayerTurn);
        gamblerChoice = GamblerChoice.Draw;
        Debug.Log($"Jugador robó carta. Nuevo total: {totalCardsValue}");
        GameManager.Instance.SetEndofTurn();
    }

    public override void Pass()
    {
        Debug.Log("Jugador pasa.");
        gamblerChoice = GamblerChoice.Pass;
        GameManager.Instance.SetEndofTurn();
    }
}
