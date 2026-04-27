using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [Tooltip("Reference to the movement controller script")]
    private MovementController movementController;


    private void Start()
    {
        // Set reference to movement controller (Specifically for the player as there is nothing else that can take 'damage')
        movementController = GetComponent<MovementController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard")) // Detect hazardous objects entered
        {
            // Start the player death sequence
            movementController.TriggerDeath();
            this.enabled = false; // Disable damage detection to avoid triggering multiple times
        }
    }

}
