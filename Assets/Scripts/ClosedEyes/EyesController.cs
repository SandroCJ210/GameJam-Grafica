using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Controla el estado de los ojos del jugador (abiertos/cerrados).
/// 
/// IMPORTANTE SOBRE EL SISTEMA DE TURNOS:
/// - El sistema de turnos debe funcionar alternadamente: Player -> Wizard -> Player -> Wizard...
/// - En cada turno, el jugador y el mago deciden entre "Robar carta" o "Pasar"
/// - Mientras ambos sigan pidiendo cartas, la ronda continúa
/// - Cuando ambos dicen "Paso", se termina la ronda
/// 
/// IMPORTANTE SOBRE EL FEEDBACK DE AUDIO:
/// - El sonido (ligero o pesado) debe sonar ANTES de que el player elija si robar o pasar
/// - Esto permite que el jugador escuche el feedback y luego decida si acepta la carta o no
/// </summary>
public class EyesController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    
#if ENABLE_INPUT_SYSTEM
    [SerializeField] private Key toggleEyesKey = Key.Space;
#else
    [SerializeField] private KeyCode toggleEyesKey = KeyCode.Space;
#endif
    
    private bool _areEyesOpen = true;
    
    public System.Action<bool> OnEyesStateChanged;
    
    public bool AreEyesOpen 
    { 
        get => _areEyesOpen; 
        private set 
        {
            if (_areEyesOpen != value)
            {
                _areEyesOpen = value;
                OnEyesStateChanged?.Invoke(_areEyesOpen);
            }
        }
    }
    
    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
    }
    
    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current[toggleEyesKey].wasPressedThisFrame)
        {
            ToggleEyes();
        }
#else
        if (Input.GetKeyDown(toggleEyesKey))
        {
            ToggleEyes();
        }
#endif
    }
    
    public void ToggleEyes()
    {
        AreEyesOpen = !AreEyesOpen;
        Debug.Log($"Eyes are now: {(AreEyesOpen ? "OPEN" : "CLOSED")}");
    }
    
    public void OpenEyes()
    {
        AreEyesOpen = true;
        Debug.Log("Eyes OPENED");
    }
    
    public void CloseEyes()
    {
        AreEyesOpen = false;
        Debug.Log("Eyes CLOSED");
    }
    
    public void SetEyesState(bool open)
    {
        AreEyesOpen = open;
    }
}
