using UnityEngine;

public class DeathConfetti : MonoBehaviour
{
    [Tooltip("Reference to confetti prefab")]
    [SerializeField] private GameObject confettiObject;
    [Tooltip("Array of confetti sprites")]
    [SerializeField] private Sprite[] confettiSprites;

    [Tooltip("Positional spread of confetti pieces")]
    [SerializeField] private Vector2 spread = new Vector2(0.6f, 0.8f);
    [Tooltip("Number of confetti pieces spawned")]
    [SerializeField, Range(1,100)] private int amount = 40;
    [Tooltip("Initial velocity of confetti pieces (with randomness)")]
    [SerializeField] private float boost = 6;
    [Tooltip("Maximum initial angular velocity of confetti pieces")]
    [SerializeField] private float angularVelocity = 180;
    [Tooltip("Gravity Range of confetti pieces")]
    [SerializeField] private Vector2 gravityRange = new Vector2(0.4f, 0.7f);


    void Start()
    {
        // Repeat until all confetti pieces have been set up
        for (int i = 0; i < amount; i++) 
        {
            // Location
            GameObject confPiece = Instantiate(confettiObject, transform);
            confPiece.transform.localPosition =
                new Vector2(Random.Range(-spread.x, spread.x) / 2,
                    Random.Range(-spread.y, spread.y) / 2
                );

            // Colour
            int conColour = Random.Range(0, confettiSprites.Length - 1);
            confPiece.GetComponent<SpriteRenderer>().sprite = confettiSprites[conColour];

            // Velocity
            Rigidbody2D conRb = confPiece.GetComponent<Rigidbody2D>();
            conRb.linearVelocity =
                new Vector2(Random.Range(-boost / 2, boost / 2), Random.Range(boost / 5, boost));

            // Angular Velocity
            conRb.angularVelocity = Random.Range(-angularVelocity, angularVelocity);

            // Gravity
            conRb.gravityScale = Random.Range(gravityRange.x, gravityRange.y);
        }
    }

}
