using UnityEngine;

public class ClosedEyesAudioFeedback : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private EyesController eyesController;
    [SerializeField] private AudioSource audioSource;
    
    [SerializeField] private AudioClip lightSound; // Carta buena
    [SerializeField] private AudioClip heavySound; // Carta mala
    [SerializeField] private float volume = 1f;
    
    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        audioSource.volume = volume;
    }
    
    /// <summary>
    /// Reproduce el feedback de audio ANTES de que el jugador elija si robar o pasar.
    /// Este método debe ser llamado cuando el jugador está considerando robar una carta,
    /// para que escuche el sonido (ligero o pesado) y luego decida si la acepta o no.
    /// </summary>
    /// <param name="cardValue">El valor de la carta que se va a ofrecer (sin robarla aún)</param>
    /// <param name="playerCurrentTotal">El total actual del jugador antes de recibir la carta</param>
    /// <returns>True si la carta es buena, False si es mala</returns>
    public bool PlayCardPreviewFeedback(int cardValue, int playerCurrentTotal)
    {
        // Solo reproducir feedback si los ojos están cerrados
        if (eyesController != null && eyesController.AreEyesOpen)
        {
            return false; // No reproducir feedback si los ojos están abiertos
        }
        
        bool isLightCard = cardValue >= 1 && cardValue <= 5;
        AudioClip clipToPlay = isLightCard ? lightSound : heavySound;
        
        if (clipToPlay != null && audioSource != null)
        {
            audioSource.PlayOneShot(clipToPlay, volume);
            Debug.Log($"ClosedEyesAudioFeedback: Playing {(isLightCard ? "LIGHT" : "HEAVY")} sound for card value: {cardValue} (Player total: {playerCurrentTotal})");
        }
        else
        {
            Debug.LogWarning("ClosedEyesAudioFeedback: AudioClip o AudioSource no asignado. No se puede reproducir sonido.");
        }
        
        return isLightCard;
    }
    
    /// <summary>
    /// Reproduce el feedback de audio cuando el jugador recibe una carta con los ojos cerrados.
    /// Este método se mantiene por compatibilidad, pero el método principal es PlayCardPreviewFeedback.
    /// </summary>
    /// <param name="cardValue">El valor de la carta recibida</param>
    /// <param name="playerTotal">El total del jugador después de recibir la carta</param>
    [System.Obsolete("Usar PlayCardPreviewFeedback en su lugar. Este método se mantiene por compatibilidad.")]
    public void PlayCardFeedback(int cardValue, int playerTotal)
    {
        // Calcular el total anterior restando el valor de la carta
        int previousTotal = playerTotal - cardValue;
        PlayCardPreviewFeedback(cardValue, previousTotal);
    }
    
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }
}
