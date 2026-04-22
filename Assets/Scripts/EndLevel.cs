using UnityEngine;

public class EndLevel : MonoBehaviour
{
    [SerializeField] private int nextLevelID = 1;

    public void TriggerEnd()
    {
        GameManager.Instance.SwitchLevel(nextLevelID);
    }
}
