using UnityEngine;

public class BonusHookScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MovementController playerMov = collision.transform.GetComponent<MovementController>();
            playerMov.RefillHookUses(1);
            Destroy(this.gameObject);
        }

    }
}
