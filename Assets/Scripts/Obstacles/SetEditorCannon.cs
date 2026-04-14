using UnityEngine;

[ExecuteInEditMode]
public class SetEditorCannon : MonoBehaviour
{
    [SerializeField, Range(1, 8)] private int cannonStartDir;

    enum CannonType
    {
        Locked, Free
    }

    [SerializeField] private CannonType cannonType = CannonType.Free;

    [SerializeField] private Sprite straightSpriteA;
    [SerializeField] private Sprite diagonalSpriteA;
    [SerializeField] private Sprite straightSpriteB;
    [SerializeField] private Sprite diagonalSpriteB;

    private void OnValidate()
    {
        CannonScript cannonScript;
        try
        {
            cannonScript = GetComponent<CannonScript>();
            if (cannonScript == null) { return; }
        }
        catch
        {
            return;
        }


        if (cannonType == CannonType.Free)
        {
            cannonScript.cannonType = "Free";
            cannonScript.straightSprite = straightSpriteA;
            cannonScript.diagonalSprite = diagonalSpriteA;
        }
        else
        {
            cannonScript.cannonType = "Locked";
            cannonScript.straightSprite = straightSpriteB;
            cannonScript.diagonalSprite = diagonalSpriteB;
        }

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

        cannonScript.ChangeCannonDirection(cannonScript.cannonDirection);
    }

}
