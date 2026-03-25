using UnityEngine;

public class BreakHookSpawner : MonoBehaviour
{
    [SerializeField] private GameObject brokenHookPrefab;
    public void BreakHook(Vector2 additionalForce)
    {
        BreakHookVFX vfx = Instantiate(brokenHookPrefab, transform.position, Quaternion.identity).GetComponent<BreakHookVFX>();
        vfx.additionalForce = additionalForce;
    }
}
