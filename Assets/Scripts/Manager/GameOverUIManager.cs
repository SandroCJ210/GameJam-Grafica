using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverCanvas;
    
    [Header("Panels")]
    [SerializeField] private Image darkBackgroundPanel; // Panel oscuro que cubre toda la pantalla
    [SerializeField] private GameObject playerWinPanel;
    [SerializeField] private GameObject wizardWinPanel;
    [SerializeField] private GameObject tiePanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Buttons")]
    [SerializeField] private Button returnToMenuButton;
    [SerializeField] private Button playAgainButton;

    private void Awake()
    {
        // Asegurarse de que el Canvas empiece oculto
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);

        // Configurar el panel de fondo oscuro
        SetupDarkBackground();

        // Configurar botones
        returnToMenuButton.onClick.AddListener(OnMenuClicked);
        playAgainButton.onClick.AddListener(OnPlayAgainClicked);
    }

    /// <summary>
    /// Configura el panel de fondo oscuro si no está asignado manualmente.
    /// </summary>
    private void SetupDarkBackground()
    {
        // Si el panel no está asignado, intentar crearlo automáticamente
        if (darkBackgroundPanel == null && gameOverCanvas != null)
        {
            // Buscar si ya existe un panel de fondo
            darkBackgroundPanel = gameOverCanvas.GetComponentInChildren<Image>();
            
            // Si no existe, crear uno nuevo
            if (darkBackgroundPanel == null)
            {
                GameObject bgObject = new GameObject("DarkBackground");
                bgObject.transform.SetParent(gameOverCanvas.transform, false);
                
                // Configurar RectTransform para cubrir toda la pantalla
                RectTransform rectTransform = bgObject.AddComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.anchoredPosition = Vector2.zero;
                
                // Agregar Image component con color oscuro semitransparente
                darkBackgroundPanel = bgObject.AddComponent<Image>();
                darkBackgroundPanel.color = new Color(0, 0, 0, 0.85f); // Negro con 85% de opacidad
                
                // Asegurar que esté detrás de todo (primer hijo = más atrás)
                bgObject.transform.SetAsFirstSibling();
            }
        }
    }

    private void OnEnable()
    {
        // Nos suscribimos a los mensajes de la consola
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        // Nos desuscribimos para evitar errores
        Application.logMessageReceived -= HandleLog;
    }

    /// <summary>
    /// Este método se ejecuta cada vez que Unity hace un Debug.Log
    /// </summary>
    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // El GameManager imprime: "Ganador: [Nombre] - [Razón]"
        // Usamos este texto para activar la UI sin tocar el GameManager.
        if (logString.StartsWith("Ganador:"))
        {
            ParseLogAndShowUI(logString);
        }
    }

    private void ParseLogAndShowUI(string logString)
    {
        // 1. Pausar el juego para que el GameManager no inicie la siguiente ronda inmediatamente
        Time.timeScale = 0;

        // 2. Activar Canvas y mostrar fondo oscuro
        gameOverCanvas.SetActive(true);
        
        // Activar el panel de fondo oscuro para oscurecer el resto de la pantalla
        if (darkBackgroundPanel != null)
        {
            darkBackgroundPanel.gameObject.SetActive(true);
            // Asegurar que esté detrás de todo
            darkBackgroundPanel.transform.SetAsFirstSibling();
        }

        // 3. Limpiar y ocultar paneles de resultado
        playerWinPanel.SetActive(false);
        wizardWinPanel.SetActive(false);
        tiePanel.SetActive(false);

        // 3. Analizar el texto para saber quién ganó
        // Formato esperado: "Ganador: Mago - Razón..."
        
        string[] parts = logString.Split(new string[] { " - " }, System.StringSplitOptions.None);
        string winnerPart = parts[0].Replace("Ganador: ", "").Trim(); // "Mago", "Jugador" o "Empate"
        string reasonPart = parts.Length > 1 ? parts[1] : "";

        // Asignar texto de razón
        messageText.text = reasonPart;

        // Activar panel correcto según el texto del log
        if (winnerPart.Contains("Jugador"))
        {
            playerWinPanel.SetActive(true);
            winnerText.text = "¡JUGADOR GANA!";
        }
        else if (winnerPart.Contains("Mago"))
        {
            wizardWinPanel.SetActive(true);
            winnerText.text = "¡EL MAGO GANA!";
        }
        else // Empate
        {
            tiePanel.SetActive(true);
            winnerText.text = "¡EMPATE!";
        }
    }

    private void OnPlayAgainClicked()
    {
        // Importante: Reactivar el tiempo antes de recargar
        Time.timeScale = 1;
        
        // Resetear el singleton del GameManager si es necesario (aunque al recargar escena se suele reiniciar)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMenuClicked()
    {
        Time.timeScale = 1; // Reactivamos el tiempo
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetGame();
        }
        
        // Cargar la escena por su índice (0) en lugar de por nombre
        SceneManager.LoadScene(0); 
    }
}