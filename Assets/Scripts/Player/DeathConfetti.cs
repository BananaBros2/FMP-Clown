using UnityEngine;

public class DeathConfetti : MonoBehaviour
{
    [SerializeField] private GameObject confettiObject;
    [SerializeField] private Sprite[] confettiSprites;
    [SerializeField] private Vector2 spread;
    [SerializeField, Range(1,100)] private int amount;
    [SerializeField] private float boost;

    void Start()
    {
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
            conRb.angularVelocity = Random.Range(-180, 180);

            // Gravity
            conRb.gravityScale = Random.Range(0.4f, 0.7f);
        }
    }

}
