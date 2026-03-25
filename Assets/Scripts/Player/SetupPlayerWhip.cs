using UnityEngine;

public class SetupPlayerWhip : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    SpriteRenderer outlineSpriteRenderer;

    [SerializeField] Sprite exactOutline;
    [SerializeField] Sprite thickOutline;

    public Transform targetPoint;
    public Transform playerTransform;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        outlineSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        SetChainProperties();
    }

    private void FixedUpdate()
    {
        SetChainProperties();
    }

    private void SetChainProperties()
    {
        // Set Position
        transform.position = targetPoint.position;

        // Get Chain Angle
        Vector2 point1 = targetPoint.position;
        Vector2 point2 = playerTransform.position;
        float angle = Mathf.Atan2(point2.y - point1.y, point2.x - point1.x) * Mathf.Rad2Deg;

        // Set Chain Angle
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Change outline thickness based on diagonal-ness
        float angleDepth = Mathf.Abs((Mathf.Round(angle * 10) / 90) % 10);
        if (angleDepth > 2 && angleDepth < 8) { outlineSpriteRenderer.sprite = thickOutline; }
        else { outlineSpriteRenderer.sprite = exactOutline; }

        // Set length of chain
        spriteRenderer.size = new Vector2(Vector2.Distance(transform.position, playerTransform.position), spriteRenderer.size.y);
        outlineSpriteRenderer.size = new Vector2(spriteRenderer.size.x + 0.0625f, spriteRenderer.size.y);
    }

}
