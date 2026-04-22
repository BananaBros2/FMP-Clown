using UnityEngine;

public class SurfaceVelocity : MonoBehaviour
{
    Vector2 objectDisplacement;

    public MovementController playerToMove;

    MovingBlock owningGroup;

    private void Start()
    {
        owningGroup = transform.parent.GetComponent<MovingBlock>();
    }
    public void SetDisplacement(Vector2 displacement)
    {
        objectDisplacement = displacement;

        if (playerToMove != null) 
        {
            playerToMove.PlayerMoveRequest(objectDisplacement);
        }

    }

    public Vector2 GetVelocity()
    {
        return objectDisplacement;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            owningGroup.DetectedPlayer();
        }
    }


}
