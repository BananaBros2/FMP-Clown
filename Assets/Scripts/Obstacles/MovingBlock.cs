using System.Collections.Generic;
using UnityEngine;

public class MovingBlock : MonoBehaviour
{

    [Tooltip("List of children")]
    List<GameObject> childObjs = new List<GameObject>();
    List<Vector2> childOffsets = new List<Vector2>();
    private List<SurfaceVelocity> velocitySharers = new List<SurfaceVelocity>();

    [Tooltip("Size of grid compared to 1 unity meter")]
    private float gridSize = 0.625f; // Used to translate units into tile size (just so I don't need to work in units of 0.625 to allign to grid)


    public Vector3[] stageTrack;

    Vector2 nextPos;

    public bool loop; // Whether will loop back to start point
    private int flipped = 1;

    [SerializeField] bool waitForPlayer = true;
    [SerializeField] bool stopWhenNoPlayer = false;
    bool playerDetected = false;
    bool finishedTrack = true;

    [SerializeField] private int startPoint = 0;
    private int currentPoint = 0;


    [Header("Debug")]
    [Tooltip("Size of track points")]
    [SerializeField, Range(0.04f, 1)] private float pointSizeDebug = 0.2f;












    private void Start()
    {
        // Setup starting point
        currentPoint = startPoint;
        if (startPoint > stageTrack.Length) { Debug.LogWarning("Starting point on \"" + transform.name + "\" was set higher than available points (" + startPoint + " > " + stageTrack.Length + ")"); currentPoint = stageTrack.Length - 1; }


        // Add all applicable children to list
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child != transform && child.parent == transform) // Exclude the parent itself and only allow children directly under
            {
                childObjs.Add(child.gameObject);
            }
        }


        // Change the platforms parts' starting position based on what the currentPoint is set to
        Vector2 startingPos = new Vector2(stageTrack[currentPoint].x * gridSize + transform.position.x, stageTrack[currentPoint].y * gridSize + transform.position.y);


        foreach (GameObject child in childObjs)
        {
            childOffsets.Add((Vector2)child.transform.localPosition); // Add child's default position to calculate offset whilst moving
            child.transform.position = startingPos + (Vector2)child.transform.localPosition; // Move child to designated starting position on track

            if (child.TryGetComponent<SurfaceVelocity>(out SurfaceVelocity childSurVelScript)) // If found, add child's surfaceVelocity in order to relay information
            {
                velocitySharers.Add(childSurVelScript);
            }

        }

        nextPos = transform.GetChild(0).position;
    }


    void FixedUpdate()
    {
        if (GameManager.Instance.GetEnvironmentPausedStatus()) { return; } // Check if game has paused motion
        if (waitForPlayer && !playerDetected && !stopWhenNoPlayer) { return; } //
        if (stopWhenNoPlayer && finishedTrack) { return; }

        
        Vector2 targetPosition = new Vector2(stageTrack[currentPoint].x * gridSize + transform.position.x, stageTrack[currentPoint].y * gridSize + transform.position.y);
        float platformSpeed = (stageTrack[currentPoint].z + stageTrack[Mathf.Clamp(currentPoint - flipped, 0, stageTrack.Length - 1)].z) / 2; // Mmm math

        nextPos = Vector2.MoveTowards(nextPos, targetPosition, platformSpeed / 10);

        // If platform has is within 0.001m of the targeted point
        if (Vector2.Distance(transform.GetChild(0).position, new Vector2(stageTrack[currentPoint].x * gridSize + transform.position.x, stageTrack[currentPoint].y * gridSize + transform.position.y)) < 0.001f)
        {
            currentPoint += flipped;
        }


        if (currentPoint + 1 > stageTrack.Length || currentPoint < 0) // If platform has reached the end of the track
        {

            if (loop) { currentPoint = 0; } // Loop back to start point
            else
            {
                flipped *= -1; // Reverse direction
                currentPoint += flipped;
            }

            if (currentPoint == startPoint && stopWhenNoPlayer)
            {
                finishedTrack = true;
            }
        }

        Vector2 lastChildPos = transform.GetChild(0).position;
        int childListIndex = 0;
        foreach (GameObject child in childObjs)
        {
            child.transform.position = nextPos + childOffsets[childListIndex];
            childListIndex++;
        }
        Vector2 objectDisplacement = (Vector2)transform.GetChild(0).position - lastChildPos;
      
        foreach (SurfaceVelocity velocitySharer in velocitySharers)
        {
            velocitySharer.SetDisplacement(objectDisplacement);
        }



    }


    /// <summary>
    /// Method to get notified of the player being detected on a child
    /// </summary>
    /// <param name="state"></param>
    public void DetectedPlayer(bool state)
    {
        if (!stopWhenNoPlayer && !state) { return; } // Handle trigger to stop when no player is detected

        if (waitForPlayer) // Handle trigger to start platform when detecting player
        {
            playerDetected = state;
            finishedTrack = false;
        }
    }



#if UNITY_EDITOR

    // Using to visually show route that the platform will travel
    void OnDrawGizmos() 
    {
        Vector3 lastPoint = Vector3.zero;
        Vector2 gizmoOffset = transform.position;

        Gizmos.DrawSphere(transform.position, pointSizeDebug); // Will draw a sphere at the starting position located at the platform's placement


        foreach (Vector3 point in stageTrack) // Iterates through all the given vectors and draws a sphere plus a line linking it with the previous point
        {
            Gizmos.DrawSphere(new Vector3(point.x * gridSize + gizmoOffset.x, point.y * gridSize + gizmoOffset.y, 0), pointSizeDebug);

            Gizmos.DrawLine(new Vector3(lastPoint.x * gridSize + gizmoOffset.x, lastPoint.y * gridSize + gizmoOffset.y, 0), new Vector3(point.x * gridSize + gizmoOffset.x, point.y * gridSize + gizmoOffset.y, 0));
            lastPoint = point;
        }

        if (loop) // Draw a line between the start and end positions
        {
            Gizmos.DrawSphere(new Vector3(stageTrack[0].x * gridSize + gizmoOffset.x, stageTrack[0].y * gridSize + gizmoOffset.y, 0), pointSizeDebug);
            Gizmos.DrawLine(new Vector3(lastPoint.x * gridSize + gizmoOffset.x, lastPoint.y * gridSize + gizmoOffset.y, 0), 
                new Vector3(stageTrack[0].x * gridSize + gizmoOffset.x, stageTrack[0].y * gridSize + gizmoOffset.y, 0));
        }
    }

# endif

}
