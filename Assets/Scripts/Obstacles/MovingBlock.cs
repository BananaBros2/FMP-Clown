using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class MovingBlock : MonoBehaviour
{
    List<GameObject> childObjs = new List<GameObject>();

    Vector2 lastPosition;
    Vector2 objectDisplacement;

    Vector2 nextPos;

    [SerializeField] bool waitForPlayer = true;
    bool playerDetected = false;

    [SerializeField] List<SurfaceVelocity> velocitySharers = new List<SurfaceVelocity>();

    [Header("Track Control")]
    public Vector3[] stageTrack;
    public int currentPoint = 0;
    public bool loop; // Whether will loop back to start point
    [UnityEngine.Range(0.04f, 1)] public float pointSizeDebug = 0.2f; // Size of track points

    [Header("Current State Values")]
    private int flipped = 1;
    private Vector2 offset;

    private float gridSize = 0.625f; // Used to translate units into tile size (just so I don't need to work in units of 0.625 to allign to grid)

    private void Start()
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child != transform) // Exclude the parent itself
            {
                childObjs.Add(child.gameObject);
            }
        }

        offset = transform.position;

        // Change the platforms parts' starting position based on what the currentPoint is set to
        Vector2 startingPos = new Vector2(stageTrack[currentPoint].x * gridSize + offset.x, stageTrack[currentPoint].y * gridSize + offset.y);

        foreach (GameObject child in childObjs)
        {
            child.transform.position = startingPos;
        }
        nextPos = transform.GetChild(0).position;
    }

    void FixedUpdate()
    {
        if (waitForPlayer && !playerDetected) { return; }

        Vector2 targetPosition = new Vector2(stageTrack[currentPoint].x * gridSize + offset.x, stageTrack[currentPoint].y * gridSize + offset.y);
        float platformSpeed = (stageTrack[currentPoint].z + stageTrack[Mathf.Clamp(currentPoint - flipped, 0, stageTrack.Length - 1)].z) / 2; // Mmm math

        nextPos = Vector2.MoveTowards(nextPos, targetPosition, platformSpeed / 10);

        // If platform has is within 0.001m of the targeted point
        if (Vector2.Distance(transform.GetChild(0).position, new Vector2(stageTrack[currentPoint].x * gridSize + offset.x, stageTrack[currentPoint].y * gridSize + offset.y)) < 0.001f)
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

        }

        Vector2 lastChildPos = transform.GetChild(0).position;
        foreach (GameObject child in childObjs)
        {
            child.transform.position = nextPos;
        }
        objectDisplacement = (Vector2)transform.GetChild(0).position - lastChildPos;
      
        foreach (SurfaceVelocity velocitySharer in velocitySharers)
        {
            velocitySharer.SetDisplacement(objectDisplacement);
        }



    }

    public void DetectedPlayer()
    {
        if (waitForPlayer)
        {
            playerDetected = true;
        }
    }



#if UNITY_EDITOR
    void OnDrawGizmos() // Used to visually show route that the platform will travel
    {
        Vector3 lastPoint = new Vector3(0, 0, 0);
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
