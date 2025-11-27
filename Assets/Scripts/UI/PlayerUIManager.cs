using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Maneja la UI del jugador durante su turno.
/// Conecta los botones de la UI con los métodos del Player.
/// </summary>
public class PlayerUIManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Player player;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private EyesController eyesController;

    [Header("UI - Decisión de Ojos (Al inicio del turno)")]
    [SerializeField] private GameObject eyesDecisionPanel;
    [SerializeField] private Button openEyesButton;
    [SerializeField] private Button closeEyesButton;

    [Header("UI - Acciones (Robar/Pasar)")]
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private Button drawCardButton;
    [SerializeField] private Button passButton;

    [Header("UI - Confirmación (Después de preview)")]
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI cardPreviewText; // Muestra info de la carta

    [Header("UI - Información")]
    [SerializeField] private TextMeshProUGUI playerTotalText;
    [SerializeField] private TextMeshProUGUI eyesStatusText;

    private bool isWaitingForConfirmation = false;
    private int previewCardValue = 0;
    private bool previewIsFavorable = false;

    private void Awake()
    {
        // Buscar referencias si no están asignadas
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }

        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }

        if (eyesController == null && player != null)
        {
            eyesController = player.GetComponent<EyesController>();
        }
    }

    private void Start()
    {
        // Verificar EventSystem
        CheckEventSystem();
        
        // Verificar referencias
        ValidateReferences();
        
        // Configurar botones
        SetupButtons();
        
        // Ocultar todos los paneles al inicio
        HideAllPanels();
    }

    private void CheckEventSystem()
    {
        EventSystem eventSystem = FindFirstObjectByType<EventSystem>();
        if (eventSystem == null)
        {
            Debug.LogError("PlayerUIManager: No hay EventSystem en la escena! Los botones no funcionarán. Crea uno: GameObject → UI → Event System");
        }
        else
        {
            Debug.Log($"PlayerUIManager: EventSystem encontrado: {eventSystem.name}");
            if (!eventSystem.enabled)
            {
                Debug.LogWarning("PlayerUIManager: EventSystem está deshabilitado! Habilitándolo...");
                eventSystem.enabled = true;
            }
        }
    }

    private void ValidateReferences()
    {
        if (player == null)
        {
            Debug.LogError("PlayerUIManager: Player no está asignado!");
        }

        if (gameManager == null)
        {
            Debug.LogError("PlayerUIManager: GameManager no está asignado!");
        }

        if (eyesController == null)
        {
            Debug.LogWarning("PlayerUIManager: EyesController no está asignado!");
        }

        if (eyesDecisionPanel == null)
        {
            Debug.LogWarning("PlayerUIManager: EyesDecisionPanel no está asignado!");
        }

        if (actionPanel == null)
        {
            Debug.LogWarning("PlayerUIManager: ActionPanel no está asignado!");
        }

        if (confirmationPanel == null)
        {
            Debug.LogWarning("PlayerUIManager: ConfirmationPanel no está asignado!");
        }
    }

    private void SetupButtons()
    {
        Debug.Log("PlayerUIManager: Configurando botones...");

        // Limpiar listeners anteriores para evitar duplicados
        RemoveAllListeners();

        // Botones de decisión de ojos
        if (openEyesButton != null)
        {
            if (!openEyesButton.interactable)
            {
                Debug.LogWarning("PlayerUIManager: openEyesButton no es interactuable!");
                openEyesButton.interactable = true;
            }
            openEyesButton.onClick.AddListener(OnOpenEyesClicked);
            Debug.Log($"PlayerUIManager: Botón 'Ojos Abiertos' configurado. Interactable: {openEyesButton.interactable}");
        }
        else
        {
            Debug.LogError("PlayerUIManager: openEyesButton no está asignado!");
        }
        
        if (closeEyesButton != null)
        {
            if (!closeEyesButton.interactable)
            {
                Debug.LogWarning("PlayerUIManager: closeEyesButton no es interactuable!");
                closeEyesButton.interactable = true;
            }
            closeEyesButton.onClick.AddListener(OnCloseEyesClicked);
            Debug.Log($"PlayerUIManager: Botón 'Ojos Cerrados' configurado. Interactable: {closeEyesButton.interactable}");
        }
        else
        {
            Debug.LogError("PlayerUIManager: closeEyesButton no está asignado!");
        }

        // Botones de acción
        if (drawCardButton != null)
        {
            if (!drawCardButton.interactable)
            {
                Debug.LogWarning("PlayerUIManager: drawCardButton no es interactuable!");
                drawCardButton.interactable = true;
            }
            drawCardButton.onClick.AddListener(OnDrawCardClicked);
            Debug.Log($"PlayerUIManager: Botón 'Robar Carta' configurado. Interactable: {drawCardButton.interactable}");
        }
        else
        {
            Debug.LogError("PlayerUIManager: drawCardButton no está asignado!");
        }
        
        if (passButton != null)
        {
            if (!passButton.interactable)
            {
                Debug.LogWarning("PlayerUIManager: passButton no es interactuable!");
                passButton.interactable = true;
            }
            passButton.onClick.AddListener(OnPassClicked);
            Debug.Log($"PlayerUIManager: Botón 'Pasar' configurado. Interactable: {passButton.interactable}");
        }
        else
        {
            Debug.LogError("PlayerUIManager: passButton no está asignado!");
        }

        // Botones de confirmación
        if (confirmButton != null)
        {
            if (!confirmButton.interactable)
            {
                Debug.LogWarning("PlayerUIManager: confirmButton no es interactuable!");
                confirmButton.interactable = true;
            }
            confirmButton.onClick.AddListener(OnConfirmClicked);
            Debug.Log($"PlayerUIManager: Botón 'Confirmar' configurado. Interactable: {confirmButton.interactable}");
        }
        else
        {
            Debug.LogError("PlayerUIManager: confirmButton no está asignado!");
        }
        
        if (cancelButton != null)
        {
            if (!cancelButton.interactable)
            {
                Debug.LogWarning("PlayerUIManager: cancelButton no es interactuable!");
                cancelButton.interactable = true;
            }
            cancelButton.onClick.AddListener(OnCancelClicked);
            Debug.Log($"PlayerUIManager: Botón 'Cancelar' configurado. Interactable: {cancelButton.interactable}");
        }
        else
        {
            Debug.LogError("PlayerUIManager: cancelButton no está asignado!");
        }

        Debug.Log("PlayerUIManager: Configuración de botones completada.");
    }

    private void RemoveAllListeners()
    {
        if (openEyesButton != null) openEyesButton.onClick.RemoveAllListeners();
        if (closeEyesButton != null) closeEyesButton.onClick.RemoveAllListeners();
        if (drawCardButton != null) drawCardButton.onClick.RemoveAllListeners();
        if (passButton != null) passButton.onClick.RemoveAllListeners();
        if (confirmButton != null) confirmButton.onClick.RemoveAllListeners();
        if (cancelButton != null) cancelButton.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        // Limpiar listeners al destruir el objeto
        RemoveAllListeners();
    }

    private void Update()
    {
        // Actualizar información del jugador
        UpdatePlayerInfo();
    }

    /// <summary>
    /// Muestra el panel de decisión de ojos al inicio del turno del jugador.
    /// </summary>
    public void ShowEyesDecisionPanel()
    {
        Debug.Log("PlayerUIManager: Mostrando panel de decisión de ojos...");
        HideAllPanels();
        
        if (eyesDecisionPanel != null)
        {
            eyesDecisionPanel.SetActive(true);
            Debug.Log("PlayerUIManager: Panel de decisión de ojos activado.");
        }
        else
        {
            Debug.LogError("PlayerUIManager: eyesDecisionPanel es null! No se puede mostrar.");
        }
    }

    /// <summary>
    /// Muestra el panel de acciones (Robar/Pasar) después de decidir los ojos.
    /// </summary>
    public void ShowActionPanel()
    {
        HideAllPanels();
        isWaitingForConfirmation = false; // Resetear estado de confirmación
        
        if (actionPanel != null)
        {
            actionPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Muestra el panel de acciones pero en modo confirmación (después de escuchar el sonido con ojos cerrados).
    /// Los botones ahora significan "Aceptar la carta" y "Pasar".
    /// </summary>
    private void ShowActionPanelWithConfirmation()
    {
        HideAllPanels();
        
        if (actionPanel != null)
        {
            actionPanel.SetActive(true);
            // El panel de acciones ahora actúa como confirmación
            // "Robar Carta" = Aceptar la carta que se preview
            // "Pasar" = Pasar (no robar)
        }
    }

    /// <summary>
    /// Muestra el panel de confirmación para preguntar si está seguro de robar carta o pasar.
    /// NO muestra el número de la carta, solo pregunta por confirmación.
    /// </summary>
    public void ShowConfirmationPanel(int cardValue, bool isFavorable, bool eyesOpen)
    {
        previewCardValue = cardValue;
        previewIsFavorable = isFavorable;
        isWaitingForConfirmation = true;

        HideAllPanels();
        
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            
            // Actualizar texto del preview - solo mensaje de confirmación, NO el número de la carta
            if (cardPreviewText != null)
            {
                if (eyesOpen)
                {
                    // Con ojos abiertos: solo pregunta si está seguro
                    cardPreviewText.text = "¿Estás seguro de robar una carta?";
                }
                else
                {
                    // Con ojos cerrados: menciona el sonido pero NO el número
                    string soundType = isFavorable ? "LIGERO" : "PESADO";
                    cardPreviewText.text = $"Escuchaste un sonido {soundType}.\n¿Estás seguro de robar esta carta?";
                }
            }
        }
    }

    private void HideAllPanels()
    {
        if (eyesDecisionPanel != null)
            eyesDecisionPanel.SetActive(false);
        
        if (actionPanel != null)
            actionPanel.SetActive(false);
        
        if (confirmationPanel != null)
            confirmationPanel.SetActive(false);
    }

    private void UpdatePlayerInfo()
    {
        if (player == null) return;

        // Actualizar total del jugador
        if (playerTotalText != null)
        {
            playerTotalText.text = $"Total: {player.TotalCardsValue}";
        }

        // Actualizar estado de ojos - usar EyesController directamente para obtener el estado actualizado
        if (eyesStatusText != null)
        {
            bool eyesOpen = false;
            if (eyesController != null)
            {
                eyesOpen = eyesController.AreEyesOpen;
            }
            else if (player != null)
            {
                eyesOpen = player.areEyesOpen;
            }
            
            eyesStatusText.text = $"Ojos: {(eyesOpen ? "ABIERTOS" : "CERRADOS")}";
        }
    }

    // ========== CALLBACKS DE BOTONES ==========

    private void OnOpenEyesClicked()
    {
        Debug.Log("PlayerUIManager: Botón 'Ojos Abiertos' CLICKEADO!");
        
        if (eyesController != null)
        {
            eyesController.OpenEyes();
            Debug.Log("PlayerUIManager: EyesController.OpenEyes() llamado.");
        }
        else
        {
            Debug.LogWarning("PlayerUIManager: eyesController es null!");
        }
        
        Debug.Log("Jugador eligió: Ojos ABIERTOS");
        
        // Forzar actualización inmediata del texto
        UpdatePlayerInfo();
        
        ShowActionPanel();
    }

    private void OnCloseEyesClicked()
    {
        Debug.Log("PlayerUIManager: Botón 'Ojos Cerrados' CLICKEADO!");
        
        if (eyesController != null)
        {
            eyesController.CloseEyes();
            Debug.Log("PlayerUIManager: EyesController.CloseEyes() llamado.");
        }
        else
        {
            Debug.LogWarning("PlayerUIManager: eyesController es null!");
        }
        
        Debug.Log("Jugador eligió: Ojos CERRADOS");
        
        // Forzar actualización inmediata del texto
        UpdatePlayerInfo();
        
        ShowActionPanel();
    }

    private void OnDrawCardClicked()
    {
        Debug.Log("PlayerUIManager: Botón 'Robar Carta' CLICKEADO!");
        
        if (player == null)
        {
            Debug.LogError("PlayerUIManager: player es null! No se puede robar carta.");
            return;
        }

        if (gameManager == null)
        {
            Debug.LogError("PlayerUIManager: gameManager es null! No se puede obtener preview.");
            return;
        }

        // Si ya estamos esperando confirmación, entonces confirmar y robar la carta
        if (isWaitingForConfirmation)
        {
            Debug.Log("Jugador confirma robar la carta.");
            player.ConfirmDrawCard();
            isWaitingForConfirmation = false;
            HideAllPanels();
            return;
        }

        bool eyesOpen = player.areEyesOpen;
        
        // Obtener preview de la carta (esto reproduce el sonido si los ojos están cerrados)
        var (cardValue, isFavorable) = gameManager.GetCardPreview();
        Debug.Log($"PlayerUIManager: Preview obtenido - Carta: {cardValue}, Favorable: {isFavorable}");
        
        // Guardar información del preview para cuando el jugador confirme
        previewCardValue = cardValue;
        previewIsFavorable = isFavorable;
        isWaitingForConfirmation = true;
        
        // Mostrar panel de confirmación (tanto para ojos abiertos como cerrados)
        // NO muestra el número de la carta, solo pregunta si está seguro
        ShowConfirmationPanel(cardValue, isFavorable, eyesOpen);
    }

    private void OnPassClicked()
    {
        Debug.Log("PlayerUIManager: Botón 'Pasar' CLICKEADO!");
        
        if (player == null)
        {
            Debug.LogError("PlayerUIManager: player es null! No se puede pasar.");
            return;
        }

        // Si estamos esperando confirmación (después de preview), mostrar confirmación de pasar
        if (isWaitingForConfirmation)
        {
            // Mostrar confirmación: "¿Estás seguro de pasar?"
            ShowConfirmationPanelForPass();
            return;
        }

        // Pasar directamente (sin preview previo)
        Debug.Log("Jugador pasa.");
        isWaitingForConfirmation = false;
        player.Pass();
        HideAllPanels();
    }

    /// <summary>
    /// Muestra el panel de confirmación para pasar.
    /// </summary>
    private void ShowConfirmationPanelForPass()
    {
        HideAllPanels();
        
        if (confirmationPanel != null)
        {
            confirmationPanel.SetActive(true);
            
            if (cardPreviewText != null)
            {
                cardPreviewText.text = "¿Estás seguro de pasar?";
            }
        }
        
        // Marcar que estamos confirmando pasar (no robar)
        previewCardValue = 0; // No hay carta para robar
    }

    private void OnConfirmClicked()
    {
        if (player == null || !isWaitingForConfirmation) return;

        // Verificar qué acción se está confirmando basándose en el texto del preview
        if (cardPreviewText != null && cardPreviewText.text.Contains("pasar"))
        {
            // Confirmar pasar
            Debug.Log("Jugador confirma pasar.");
            isWaitingForConfirmation = false;
            player.Pass();
            HideAllPanels();
        }
        else if (previewCardValue > 0)
        {
            // Confirmar robar carta (hay una carta en preview)
            Debug.Log("Jugador confirma robar carta.");
            player.ConfirmDrawCard();
            isWaitingForConfirmation = false;
            HideAllPanels();
        }
        else
        {
            // Confirmar pasar (no hay carta en preview)
            Debug.Log("Jugador confirma pasar.");
            isWaitingForConfirmation = false;
            player.Pass();
            HideAllPanels();
        }
    }

    private void OnCancelClicked()
    {
        if (!isWaitingForConfirmation) return;

        Debug.Log("Jugador cancela la acción.");
        player.CancelDrawCard();
        isWaitingForConfirmation = false;
        ShowActionPanel(); // Volver al panel de acciones
    }

    // ========== MÉTODOS PÚBLICOS PARA LLAMAR DESDE PLAYER ==========

    /// <summary>
    /// Llamado desde Player.PlayTurn() para iniciar la UI del turno.
    /// </summary>
    public void StartPlayerTurn()
    {
        ShowEyesDecisionPanel();
    }
}

