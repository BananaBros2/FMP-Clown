using UnityEngine;

public class HairHandler : MonoBehaviour
{
    public Vector2 partOffset = Vector2.zero;

    [SerializeField] private Transform[] hairParts;

    private Transform hairAnchor;

    private Vector2 hairAnchorTargetPosition;
    private bool flipAnchorPosition;


    public float lerpSpeed = 20;



    private void Awake()
    {
        hairAnchor = GetComponent<Transform>();
        hairAnchorTargetPosition = transform.parent.localPosition;
    }

    private void Update()
    {
        Transform pieceToFollow = hairAnchor;

        foreach(Transform hairPart in hairParts)
        {
            if (!hairPart.Equals(hairParts[0]))
            {
                Vector2 targetPosition = (Vector2)pieceToFollow.position + partOffset;
                Vector2 newPositionLerped = Vector2.Lerp(hairPart.position, targetPosition, Time.deltaTime * lerpSpeed);

                hairPart.position = newPositionLerped;
                pieceToFollow = hairPart;
            }
            else
            {
                hairPart.position = (Vector2)pieceToFollow.position;
                pieceToFollow = hairPart;
            }
        }

        SetAnchorPosition();
    }


    public void SetFlipState(bool flip)
    {
        flipAnchorPosition = flip;
    }

    private void SetAnchorPosition()
    {
        float flip = flipAnchorPosition ? -1 : 1;
        float anchorPoint = flipAnchorPosition ? 0.46875f : -0.46875f;

        hairAnchor.transform.parent.localPosition =
            new Vector2(anchorPoint, hairAnchorTargetPosition.y);

        foreach (Transform hairPart in hairParts)
        {
            hairPart.localScale = new Vector3(flip, 1, 1);
        }
    }
    
}
