using UnityEngine;

public class BreakHookVFX : MonoBehaviour
{
    [Tooltip("Additional force applied to each particle")]
    public Vector2 additionalForce;
    [Tooltip("Duration this game object will be active before destroying itself")]
    [SerializeField] private float lifeTime = 2.5f;


    private void Start()
    {
        ScatterParticles();
    }

    /// <summary>
    /// Apply random velocity to the child particles
    /// </summary>
    public void ScatterParticles()
    {
        foreach (Transform child in transform) // Apply random velocity plus any additional force specified
        {
            Vector2 forceDirection = new Vector2(Random.Range(-30, 30), Random.Range(-30, 30)).normalized;
            child.GetComponent<Rigidbody2D>().linearVelocity = forceDirection * 2.5f + Vector2.up + additionalForce;
        }
    }


    private void FixedUpdate()
    {
        // Destroy object after x amount of time
        lifeTime -= Time.fixedDeltaTime;
        if (lifeTime < 0) { Destroy(this.gameObject); }
    }

}
