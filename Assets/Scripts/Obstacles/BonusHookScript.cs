using UnityEngine;

public class BonusHookScript : MonoBehaviour
{
    [Tooltip("How many hook uses will be given when collected")]
    [SerializeField] private int hookRefillAmount = 1;


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MovementController playerMov = collision.transform.GetComponent<MovementController>(); 
            if (playerMov.CanGetHooks()) // Check if can recieve more hook uses
            {
                playerMov.RefillHookUses(hookRefillAmount); // Regenerate hook use/s
                Destroy(this.gameObject);
            }

        }

    }
}
