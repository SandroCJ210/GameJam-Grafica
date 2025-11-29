using UnityEngine;
using System.Collections;
public class WizardAnimController : MonoBehaviour
{
    [SerializeField] private string animationName = "Blinking";
    
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(PlayAnimationAfterRandomDelay());
    }

    private IEnumerator PlayAnimationAfterRandomDelay()
    {
        while (true)
        {
            float waitTime = Random.Range(4f, 10f);
            yield return new WaitForSeconds(waitTime);
        
            if (animator != null)
            {
                animator.Play(animationName);
            }    
        }
        
    }
}
