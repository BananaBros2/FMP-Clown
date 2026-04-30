using UnityEngine;

public class BreakHookSpawner : MonoBehaviour
{
    [Tooltip("Reference to prefab with hook pieces")]
    [SerializeField] private GameObject brokenHookPrefab;

    /// <summary>
    /// Spawn object with hook pieces 
    /// </summary>
    /// <param name="additionalForce">additional velocity added onto the pieces</param>
    public void BreakHook(Vector2 additionalForce)
    {
        // Instantiate hook pieces prefab
        BreakHookVFX vfx = Instantiate(brokenHookPrefab, transform.position, Quaternion.identity).GetComponent<BreakHookVFX>();
        vfx.additionalForce = additionalForce;
    }
}
