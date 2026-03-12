using UnityEngine;

public class HookAimSprite : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite flatHook;

    [SerializeField]
    Sprite diagonalHook;

    private void Start()
    {
        //SetHookDirection(new Vector2(0,1));
    }

    public void SetHookDirection(Vector2 direction)
    {
        if (direction.x > 0)
        {
            if (direction.y > 0)
            {
                // Top Right
                spriteRenderer.sprite = diagonalHook;
                transform.rotation = Quaternion.Euler(0, 0, 90);
            }
            else if (direction.y < 0)
            {
                // Bottom Right
                spriteRenderer.sprite = diagonalHook;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                // Right
                spriteRenderer.sprite = flatHook;
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else if (direction.x < 0)
        {
            if (direction.y > 0)
            {
                // Top Left
                spriteRenderer.sprite = diagonalHook;
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
            else if (direction.y < 0)
            {
                // Bottom Left
                spriteRenderer.sprite = diagonalHook;
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }
            else
            {
                // Left
                spriteRenderer.sprite = flatHook;
                transform.rotation = Quaternion.Euler(0, 0, 180);
            }
        }
        else
        {
            if (direction.y > 0)
            {
                // Up
                spriteRenderer.sprite = flatHook;
                transform.rotation = Quaternion.Euler(0,0,90);
            }
            else if (direction.y < 0)
            {
                // Down
                spriteRenderer.sprite = flatHook;
                transform.rotation = Quaternion.Euler(0, 0, 270);
            }
            else
            {
                // Error
            }
        }
    }
}
