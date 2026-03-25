using UnityEngine;

public class DamageHandler : MonoBehaviour
{

    private void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Hazard"))
        {
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        GameManager.Instance.deathLocations.Add(transform.position);
        GameManager.Instance.RespawnAtCheckpoint();
        //Destroy(this.gameObject);
    }
}
