using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Maneja el panel de resultados al finalizar una ronda.
/// Muestra las sumas del jugador y del mago, y quién ganó.
/// </summary>
public class RoundResultUIManager : MonoBehaviour
{
    [Header("UI - Panel de Resultados")]
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI playerTotalText;
    [SerializeField] private TextMeshProUGUI wizardTotalText;
    [SerializeField] private TextMeshProUGUI reasonText;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        // Ocultar panel al inicio
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        // Configurar botón de continuar
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
        }
    }

    /// <summary>
    /// Muestra el panel de resultados con la información de la ronda.
    /// </summary>
    /// <param name="winner">Nombre del ganador ("Jugador", "Mago", o "Empate")</param>
    /// <param name="playerTotal">Total del jugador</param>
    /// <param name="wizardTotal">Total del mago</param>
    /// <param name="reason">Razón por la que ganó</param>
    public void ShowRoundResult(string winner, int playerTotal, int wizardTotal, string reason)
    {
        if (resultPanel == null)
        {
            Debug.LogWarning("RoundResultUIManager: Panel de resultados no asignado!");
            return;
        }

        // Actualizar textos
        if (winnerText != null)
        {
            winnerText.text = $"Ganador: {winner}";
            
            // Cambiar color según ganador
            if (winner == "Jugador")
            {
                winnerText.color = Color.green;
            }
            else if (winner == "Mago")
            {
                winnerText.color = Color.red;
            }
            else
            {
                winnerText.color = Color.yellow;
            }
        }

        if (playerTotalText != null)
        {
            playerTotalText.text = $"Jugador: {playerTotal}";
            
            // Resaltar si se pasó de 21
            if (playerTotal > 21)
            {
                playerTotalText.color = Color.red;
            }
            else
            {
                playerTotalText.color = Color.white;
            }
        }

        if (wizardTotalText != null)
        {
            wizardTotalText.text = $"Mago: {wizardTotal}";
            
            // Resaltar si se pasó de 21
            if (wizardTotal > 21)
            {
                wizardTotalText.color = Color.red;
            }
            else
            {
                wizardTotalText.color = Color.white;
            }
        }

        if (reasonText != null)
        {
            reasonText.text = reason;
        }

        // Mostrar panel
        resultPanel.SetActive(true);
        
        Debug.Log($"RoundResultUIManager: Mostrando resultados - {winner} ganó");
    }

    /// <summary>
    /// Oculta el panel de resultados.
    /// </summary>
    public void HideRoundResult()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Callback del botón "Continuar".
    /// Oculta el panel y permite continuar con la siguiente ronda.
    /// </summary>
    private void OnContinueClicked()
    {
        HideRoundResult();
        Debug.Log("RoundResultUIManager: Continuando a la siguiente ronda (reiniciando juego)...");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGameFromUI();
        }
        else
        {
            Debug.LogWarning("RoundResultUIManager: GameManager.Instance es null, no se puede reiniciar el juego.");
        }
    }
}

