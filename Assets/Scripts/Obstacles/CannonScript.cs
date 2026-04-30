using UnityEngine;

public class CannonScript : MonoBehaviour
{
    [Tooltip("Reference to the SpriteRenderer component")]
    [SerializeField] private SpriteRenderer spriteRenderer;


    [Tooltip("Vector direction the cannon is facing")]
    [HideInInspector] public Vector2 cannonDirection = new Vector2(0,1);
    public Vector2 GetCannonDirection() { return cannonDirection; }
    [Tooltip("Type of cannon")]
    [HideInInspector] public string cannonType = "Free";
    public string GetCannonType() { return cannonType; }


    [Tooltip("Sprite used to show cannon facing in a cardinal direction")]
    [HideInInspector] public Sprite straightSprite;
    [Tooltip("Sprite used to show cannon facing in a diagonal direction")]
    [HideInInspector] public Sprite diagonalSprite;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Check for player collider
        {
            MovementController playerMov = collision.transform.GetComponent<MovementController>(); // Get player movement controller component
            if (playerMov == null) { return; }

            collision.transform.position = transform.position; // Move player to center of cannon
            playerMov.EnterCannon(this); // Trigger player cannon state
        }

    }

    /// <summary>
    /// Change cannon direction based on parameter
    /// </summary>
    /// <param name="newDir">new requested Vector2 cannon direction</param>
    public void ChangeCannonDirection(Vector2 newDir)
    {
        if (newDir == Vector2.zero) { return; }
        cannonDirection = newDir;

        if (newDir.x > 0) // Right directions
        {
            if (newDir.y > 0)
            {
                // Top Right
                spriteRenderer.sprite = diagonalSprite; // Update sprite
                transform.rotation = Quaternion.Euler(0, 0, 0); // Update transform rotation
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
        else if (newDir.x < 0) // Left directions
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
        else // Vertical Directions
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




}
