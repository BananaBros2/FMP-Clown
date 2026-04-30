using UnityEngine;

public class SurfaceVelocity : MonoBehaviour
{
    [Tooltip("Distance moved by this object between fixed frames")]
    Vector2 objectDisplacement;

    [Tooltip("Reference to player")]
    public MovementController playerToMove;

    [Tooltip("Reference to script that this object is getting moved by")]
    MovingBlock owningGroup;


    private void Start()
    {
        owningGroup = transform.parent.GetComponent<MovingBlock>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player")) // Player was detected, notify moving group
        {
            owningGroup.DetectedPlayer(true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player")) // Player no longer detected, notify moving group
        {
            owningGroup.DetectedPlayer(false);
        }
    }


    /// <summary>
    /// Add displacement to player
    /// </summary>
    public void SetDisplacement(Vector2 displacement)
    {
        objectDisplacement = displacement;

        if (playerToMove != null) // Attempt to move 'attached' player object
        {
            playerToMove.PlayerMoveRequest(objectDisplacement);
        }

    }

    /// <summary>
    /// Get the rough velocity of this object 
    /// </summary>
    public Vector2 GetVelocity()
    {
        return objectDisplacement * 50;
    }
}
