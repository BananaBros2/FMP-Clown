using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    MovementController movementController;

    private void Start()
    {
        movementController = GetComponent<MovementController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            movementController.TriggerDeath();
            this.enabled = false;
        }
    }

}
