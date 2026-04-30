using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [Tooltip("ID for this checkpoint, must be unique to the scene to work correctly")]
    [SerializeField] private int checkPointID;
    [Tooltip("Reference to object that determines spawn position")]
    [SerializeField] private Transform spawnPosition;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Update current checkpoint to this one
            SetCheckpoint();
        }
    }

    /// <summary>
    /// Update current checkpoint in game manager
    /// </summary>
    private void SetCheckpoint()
    {
        GameManager.Instance.UpdateCheckpoint(checkPointID);
    }


    /// <summary>
    /// Returns the ID of the checkpoint
    /// </summary>
    public int GetID()
    {
        return checkPointID;
    }

    /// <summary>
    /// Returns the respawn location of the checkpoint
    /// </summary>
    public Vector3 GetSpawnPosition()
    {
        return spawnPosition.position;
    }
}
