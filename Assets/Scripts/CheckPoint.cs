using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private int checkPointID;
    [SerializeField] private Transform spawnPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetCheckpoint();
        }
    }

    private void SetCheckpoint()
    {
        GameManager.Instance.UpdateCheckpoint(checkPointID);
    }

    public int GetID()
    {
        return checkPointID;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPosition.position;
    }
}
