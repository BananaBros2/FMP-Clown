using UnityEngine;

public class BonusHookScript : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MovementController playerMov = collision.transform.GetComponent<MovementController>();
            if (playerMov.CanGetHooks())
            {
                playerMov.RefillHookUses(1);
                Destroy(this.gameObject);
            }

        }

    }
}
