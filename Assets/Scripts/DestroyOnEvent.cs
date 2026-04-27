using UnityEngine;

public class DestroyOnEvent : MonoBehaviour
{

    /// <summary>
    /// Destroy object
    /// </summary>
    public void TriggerDestruction()
    {
        Destroy(this.gameObject);
    }

}
