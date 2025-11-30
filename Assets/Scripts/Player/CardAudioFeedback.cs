using UnityEngine;

public class CardAudioFeedback : PersistentSingleton<CardAudioFeedback>
{
    [Header("Clips por rango")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip lowRangeClip;   
    [SerializeField] private AudioClip highRangeClip;  

    protected override void Awake()
    {
        base.Awake();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void PlayCardRangeSound(int cardValue)
    {
        if (audioSource == null)
        {
            return;
        }

        AudioClip clipToPlay = null;

        if (cardValue >= 1 && cardValue <= 6)
        {
            clipToPlay = lowRangeClip;
        }
        else if (cardValue >= 7 && cardValue <= 11)
        {
            clipToPlay = highRangeClip;
        }

        if (clipToPlay != null)
        {
            audioSource.PlayOneShot(clipToPlay);
        }
    }
}

