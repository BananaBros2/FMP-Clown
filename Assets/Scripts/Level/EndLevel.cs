using UnityEngine;

public class EndLevel : MonoBehaviour
{
    [Tooltip("ID of desired level to load\n0: Tutorial\n1: Level 1\n2: Level 2\n3: Level 3")]
    [SerializeField] private int nextLevelID = 1;

    public void TriggerEnd()
    {
        // Load requested level using ID provided
        GameManager.Instance.SwitchLevel(nextLevelID);
    }

}
