using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    public UnityEvent playerTriggered;

    [Tooltip("Bool for determining if this script can only be triggered once")]
    [SerializeField] private bool oneTime = true;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTriggered.Invoke(); // Invokes event to trigger anything connected

            if (oneTime) { this.enabled = false; } // Disable the trigger if oneTime is true
        }
    }



}
