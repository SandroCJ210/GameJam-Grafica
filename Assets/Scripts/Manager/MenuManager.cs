using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Maneja la UI del menú principal y la navegación entre escenas.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button exitButton;
    
    [Header("Scene Settings")]
    [SerializeField] private int gameSceneIndex = 1;

    private void Start()
    {
        // Configurar el botón de inicio si está asignado
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        else
        {
            Debug.LogWarning("MenuManager: El botón de inicio no está asignado en el Inspector.");
        }

        // Configurar el botón de salir si está asignado
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(OnExitButtonClicked);
        }
        else
        {
            Debug.LogWarning("MenuManager: El botón de salir no está asignado en el Inspector.");
        }
    }

    /// <summary>
    /// Método llamado cuando se hace clic en el botón de inicio.
    /// Carga la escena de juego especificada por gameSceneIndex.
    /// </summary>
    public void OnStartButtonClicked()
    {
        LoadGameScene();
    }

    /// <summary>
    /// Carga la escena de juego usando el índice especificado.
    /// </summary>
    public void LoadGameScene()
    {
        if (gameSceneIndex < 0 || gameSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"MenuManager: El índice de escena {gameSceneIndex} no es válido. " +
                          $"Asegúrate de que la escena esté agregada en Build Settings.");
            return;
        }

        Debug.Log($"MenuManager: Cargando escena con índice {gameSceneIndex}");
        SceneManager.LoadScene(gameSceneIndex);
    }

    /// <summary>
    /// Método público para cargar una escena específica por índice.
    /// Útil si necesitas cargar diferentes escenas desde el menú.
    /// </summary>
    /// <param name="sceneIndex">El índice de la escena a cargar</param>
    public void LoadSceneByIndex(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogError($"MenuManager: El índice de escena {sceneIndex} no es válido.");
            return;
        }

        Debug.Log($"MenuManager: Cargando escena con índice {sceneIndex}");
        SceneManager.LoadScene(sceneIndex);
    }

    /// <summary>
    /// Método llamado cuando se hace clic en el botón de salir.
    /// Cierra la aplicación.
    /// </summary>
    public void OnExitButtonClicked()
    {
        QuitGame();
    }

    /// <summary>
    /// Cierra el juego.
    /// En el editor de Unity, solo muestra un mensaje de debug.
    /// En builds, cierra la aplicación.
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("MenuManager: Saliendo del juego...");

#if UNITY_EDITOR
        // En el editor, detener el modo Play
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // En builds, cerrar la aplicación
        Application.Quit();
#endif
    }

    private void OnDestroy()
    {
        // Limpiar listeners para evitar memory leaks
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }

        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(OnExitButtonClicked);
        }
    }
}

