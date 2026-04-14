using UnityEngine;

public class CannonScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [HideInInspector] public Sprite straightSprite;
    [HideInInspector] public Sprite diagonalSprite;

    [HideInInspector] public Vector2 cannonDirection = new Vector2(0,1);

    [HideInInspector] public string cannonType = "Free";




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            MovementController playerMov = collision.transform.GetComponent<MovementController>();
            if (playerMov == null) { return; }

            collision.transform.position = transform.position;
            playerMov.EnterCannon(this);

            print("cannon time");
        }

    }

    public void ChangeCannonDirection(Vector2 newDir)
    {
        if (newDir == Vector2.zero) { return; }
        cannonDirection = newDir;

        if (newDir.x > 0)
        {
            if (newDir.y > 0)
            {
                // Top Right
                spriteRenderer.sprite = diagonalSprite;
                transform.rotation = Quaternion.Euler(0, 0, 00);
            }
            else if (newDir.y < 0)
            {
                // Bottom Right
                spriteRenderer.sprite = diagonalSprite;
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }
            else
            {
                // Right
                spriteRenderer.sprite = straightSprite;
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }
        }
        else if (newDir.x < 0)
        {
            if (newDir.y > 0)
            {
                // Top Left
                spriteRenderer.sprite = diagonalSprite;
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else if (newDir.y < 0)
            {
                // Bottom Left
                spriteRenderer.sprite = diagonalSprite;
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else
            {
                // Left
                spriteRenderer.sprite = straightSprite;
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
        }
        else
        {
            if (newDir.y > 0)
            {
                // Up
                spriteRenderer.sprite = straightSprite;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (newDir.y < 0)
            {
                // Down
                spriteRenderer.sprite = straightSprite;
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }

        }
    }

    public Vector2 GetCannonDirection()
    {
        return cannonDirection;
    }


}
