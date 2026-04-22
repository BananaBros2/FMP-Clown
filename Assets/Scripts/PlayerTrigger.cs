using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    [SerializeField] private bool oneTime = true;

    public UnityEvent playerTriggered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTriggered.Invoke();
            if (oneTime)
            {
                this.enabled = false;
            }
        }
    }



}
