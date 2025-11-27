using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Maneja el panel que muestra las decisiones del mago durante la ronda.
/// Se actualiza en tiempo real cada vez que el mago toma una decisión.
/// </summary>
public class WizardDecisionsUIManager : MonoBehaviour
{
    [Header("UI - Panel de Decisiones del Mago")]
    [SerializeField] private GameObject decisionsPanel;
    [SerializeField] private TextMeshProUGUI decisionsText;
    [SerializeField] private Button togglePanelButton;
    [SerializeField] private bool panelVisibleByDefault = false;

    private List<string> currentDecisions = new List<string>();

    private void Awake()
    {
        // Configurar estado inicial del panel
        if (decisionsPanel != null)
        {
            decisionsPanel.SetActive(panelVisibleByDefault);
        }

        // Configurar botón de toggle
        if (togglePanelButton != null)
        {
            togglePanelButton.onClick.AddListener(TogglePanel);
        }
    }

    private void Start()
    {
        // Actualizar decisiones al inicio
        UpdateDecisions();
    }

    private void Update()
    {
        // Actualizar decisiones cada frame (puede optimizarse con eventos)
        UpdateDecisions();
    }

    /// <summary>
    /// Actualiza el texto con las decisiones del mago.
    /// </summary>
    private void UpdateDecisions()
    {
        if (GameManager.Instance == null)
        {
            return;
        }

        List<string> newDecisions = GameManager.Instance.GetWizardDecisions();
        
        // Solo actualizar si hay cambios
        if (!currentDecisions.SequenceEqual(newDecisions))
        {
            currentDecisions = new List<string>(newDecisions);
            RefreshDecisionsText();
        }
    }

    /// <summary>
    /// Refresca el texto del panel con la última decisión del mago.
    /// </summary>
    private void RefreshDecisionsText()
    {
        if (decisionsText == null)
        {
            return;
        }

        if (currentDecisions.Count == 0)
        {
            decisionsText.text = "El mago aún no ha tomado decisiones...";
            return;
        }

        // Mostrar solo la última decisión (no listar todas)
        string lastDecision = currentDecisions[currentDecisions.Count - 1];
        decisionsText.text = $"Última decisión del Mago:\n\n{lastDecision}";
    }

    /// <summary>
    /// Muestra el panel de decisiones.
    /// </summary>
    public void ShowPanel()
    {
        if (decisionsPanel != null)
        {
            decisionsPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Oculta el panel de decisiones.
    /// </summary>
    public void HidePanel()
    {
        if (decisionsPanel != null)
        {
            decisionsPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Alterna la visibilidad del panel.
    /// </summary>
    public void TogglePanel()
    {
        if (decisionsPanel != null)
        {
            decisionsPanel.SetActive(!decisionsPanel.activeSelf);
        }
    }

    /// <summary>
    /// Limpia las decisiones mostradas (útil cuando comienza una nueva ronda).
    /// </summary>
    public void ClearDecisions()
    {
        currentDecisions.Clear();
        RefreshDecisionsText();
    }
}

