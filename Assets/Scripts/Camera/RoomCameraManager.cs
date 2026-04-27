using Unity.Cinemachine;
using UnityEngine;

public class RoomCameraManager : MonoBehaviour
{
    [Tooltip("Reference to the GameManager instance")]
    private GameManager gm;
    [Tooltip("Reference to the child cinemachine camera component")]
    private CinemachineCamera roomCam;

    [Tooltip("Custom room name for debugging")]
    [SerializeField] private string roomName = "UNNAMED";


    private void Start()
    {
        // Set the gamemanager reference
        gm = GameManager.Instance;

        // Set room camera Reference
        try { roomCam = GetComponentInChildren<CinemachineCamera>(); }
        catch { Debug.LogError("RoomCameraManager has no children with Cinemachine Camera"); }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // If player entered
        {
            // Increase this camera's priority so that the main camera pans to it 
            roomCam.GetComponent<CinemachineCamera>().Priority = 11;

            gm.HandleRoomTransition(roomName); // Trigger room transition
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // If player exited
        {
            roomCam.GetComponent<CinemachineCamera>().Priority = 10; // Reset this camera's priority
        }
    }


}
