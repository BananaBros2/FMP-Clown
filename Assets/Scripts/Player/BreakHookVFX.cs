using UnityEngine;

public class BreakHookVFX : MonoBehaviour
{
    public Vector2 additionalForce;
    [SerializeField] private float lifeTime = 2.5f;

    private void Start()
    {
        ScatterParticles();
    }

    public void ScatterParticles()
    {
        foreach (Transform child in transform)
        {
            Vector2 forceDirection = new Vector2(Random.Range(-30, 30), Random.Range(-30, 30)).normalized;
            child.GetComponent<Rigidbody2D>().linearVelocity = forceDirection * 2.5f + Vector2.up + additionalForce;
        }
    }

    private void FixedUpdate()
    {
        // Destroy object after x time
        lifeTime -= Time.fixedDeltaTime;
        if (lifeTime < 0) { Destroy(this.gameObject); }
    }

}
