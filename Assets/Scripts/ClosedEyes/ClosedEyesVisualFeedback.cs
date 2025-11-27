using UnityEngine;
using System.Collections;

public class ClosedEyesVisualFeedback : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private EyesController eyesController;
    [SerializeField] private GameObject eyesClosedOverlay;
    [SerializeField] private SpriteRenderer eyesClosedSprite;
    [SerializeField] private Color overlayColor = new Color(0, 0, 0, 0.8f);
    [SerializeField] private float fadeSpeed = 2f;
    
    private CanvasGroup canvasGroup;
    private bool isTransitioning = false;
    
    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
        }
        
        if (eyesController == null)
        {
            eyesController = GetComponent<EyesController>();
        }
        
        if (eyesClosedOverlay != null)
        {
            canvasGroup = eyesClosedOverlay.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = eyesClosedOverlay.AddComponent<CanvasGroup>();
            }
        }
    }
    
    private void OnEnable()
    {
        if (eyesController != null)
        {
            eyesController.OnEyesStateChanged += HandleEyesStateChanged;
        }
    }
    
    private void OnDisable()
    {
        if (eyesController != null)
        {
            eyesController.OnEyesStateChanged -= HandleEyesStateChanged;
        }
    }
    
    private void HandleEyesStateChanged(bool areOpen)
    {
        UpdateVisualFeedback(areOpen);
    }
    
    private void UpdateVisualFeedback(bool areEyesOpen)
    {
        if (eyesClosedOverlay != null)
        {
            eyesClosedOverlay.SetActive(!areEyesOpen);
        }
        
        if (eyesClosedSprite != null)
        {
            eyesClosedSprite.enabled = !areEyesOpen;
        }
        
        if (canvasGroup != null && !isTransitioning)
        {
            StartCoroutine(FadeOverlay(!areEyesOpen));
        }
    }
    
    private IEnumerator FadeOverlay(bool fadeIn)
    {
        isTransitioning = true;
        float targetAlpha = fadeIn ? 1f : 0f;
        
        while (Mathf.Abs(canvasGroup.alpha - targetAlpha) > 0.01f)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha;
        isTransitioning = false;
    }
    
    public void ShowCardReceivedEffect(bool isGoodCard)
    {
        Debug.Log($"Card received effect - Good: {isGoodCard}");
    }
}
