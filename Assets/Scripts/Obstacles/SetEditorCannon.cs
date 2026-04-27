using UnityEngine;

[ExecuteInEditMode]
public class SetEditorCannon : MonoBehaviour
{
    [Tooltip("Direction cannon will face in build")]
    [SerializeField, Range(1, 8)] private int cannonStartDir;

    /// <summary>
    /// <para>Enumerator for cannon type</para>
    /// Locked: Direction cannot be changed by player<br />
    /// Free: Can rotated freely by player<br />
    /// </summary>
    enum CannonType
    {
        Locked, Free
    }

    [Tooltip("Type of Cannon\nLocked: Direction cannot be changed by player\nFree: Can rotated freely by player")]
    [SerializeField] private CannonType cannonType = CannonType.Free;

    [Tooltip("Sprite for horizontal/vertical free cannon")]
    [SerializeField] private Sprite straightFreeSprite;
    [Tooltip("Sprite for diagonal free cannon")]
    [SerializeField] private Sprite diagonalFreeSprite;
    [Tooltip("Sprite for horizontal/vertical locked cannon")]
    [SerializeField] private Sprite straightLockedSprite;
    [Tooltip("Sprite for diagonal locked cannon")]
    [SerializeField] private Sprite diagonalLockedSprite;


    // Called when script is loaded or when a value is changed in the inspector
    private void OnValidate()
    {
        CannonScript cannonScript;
        try
        {
            cannonScript = GetComponent<CannonScript>(); // Attempt to get reference to the CannonScript
            if (cannonScript == null) { return; }
        }
        catch { return; }


        // Set cannon sprites
        if (cannonType == CannonType.Free)
        {
            cannonScript.cannonType = "Free";
            cannonScript.straightSprite = straightFreeSprite;
            cannonScript.diagonalSprite = diagonalFreeSprite;
        }
        else
        {
            cannonScript.cannonType = "Locked";
            cannonScript.straightSprite = straightLockedSprite;
            cannonScript.diagonalSprite = diagonalLockedSprite;
        }

        // Set cannon direction (interatively clockwise)
        switch (cannonStartDir)
        {
            case 1:
                cannonScript.cannonDirection = new Vector2(0, 1);
                break;
            case 2:
                cannonScript.cannonDirection = new Vector2(1, 1);
                break;
            case 3:
                cannonScript.cannonDirection = new Vector2(1, 0);
                break;
            case 4:
                cannonScript.cannonDirection = new Vector2(1, -1);
                break;
            case 5:
                cannonScript.cannonDirection = new Vector2(0, -1);
                break;
            case 6:
                cannonScript.cannonDirection = new Vector2(-1, -1);
                break;
            case 7:
                cannonScript.cannonDirection = new Vector2(-1, 0);
                break;
            case 8:
                cannonScript.cannonDirection = new Vector2(-1, 1);
                break;
        }

        // Trigger cannon to update it's direction
        cannonScript.ChangeCannonDirection(cannonScript.cannonDirection);
    }

}
