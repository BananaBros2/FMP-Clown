using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class RoomCameraManager : MonoBehaviour
{
    [Header("References")]
    public GameObject roomCam; // Camera Controller

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            roomCam.GetComponent<CinemachineCamera>().Priority = 11; // Increase the camera (location's) priority so that the camera pans over to the created area 
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
