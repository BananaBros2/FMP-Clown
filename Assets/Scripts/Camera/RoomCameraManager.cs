using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class RoomCameraManager : MonoBehaviour
{
    [SerializeField] private GameObject roomCam; // Camera Controller

    [SerializeField] private string roomName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Increase the camera (location's) priority so that the camera pans over to the created area 
            roomCam.GetComponent<CinemachineCamera>().Priority = 11;  
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            roomCam.GetComponent<CinemachineCamera>().Priority = 10; // Reset the camera (location's) priority
        }
    }


}
