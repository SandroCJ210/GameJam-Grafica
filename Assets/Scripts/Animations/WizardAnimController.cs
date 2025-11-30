using UnityEngine;
using System.Collections;

public class WizardAnimController : MonoBehaviour
{
    [SerializeField] private string idleStateName = "Idle";
    [SerializeField] private string blinkTriggerName = "Blink";   // Trigger en el Animator
    [SerializeField] private string eyesClosedBoolName = "EyesClosed"; // Bool en el Animator

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(BlinkLoop());
    }

    private IEnumerator BlinkLoop()
    {
        while (true)
        {
            // Espera aleatoria entre parpadeos
            float waitTime = Random.Range(4f, 10f);
            yield return new WaitForSeconds(waitTime);

            if (animator == null)
            {
                continue;
            }

            // No parpadear si los ojos están cerrados
            bool eyesClosed = animator.GetBool(eyesClosedBoolName);
            if (eyesClosed)
            {
                continue;
            }

            // No parpadear si está en transición (cerrando/abriendo)
            if (animator.IsInTransition(0))
            {
                continue;
            }

            // Solo parpadear si estamos en Idle (ojos abiertos)
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(idleStateName))
            {
                animator.SetTrigger(blinkTriggerName);
            }
        }
    }

    // Llamado desde fuera cuando el juego cierra/abre los ojos del personaje
    public void SetEyesClosed(bool closed)
    {
        if (animator != null)
        {
            animator.SetBool(eyesClosedBoolName, closed);
        }
    }
}