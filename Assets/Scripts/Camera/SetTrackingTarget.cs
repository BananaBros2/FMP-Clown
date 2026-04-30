using Unity.Cinemachine;
using UnityEngine;

public class SetTrackingTarget : MonoBehaviour
{
    [Tooltip("Reference to player object in scene")]
    private GameObject playerObject;
    [Tooltip("Reference to the main cinema machine camera")]
    private CinemachineCamera cinCam;

    /// <summary>
    /// Setup values to be used by each individual room camera 
    /// </summary>
    public void SetupTarget()
    {
        cinCam = GetComponent<CinemachineCamera>();

        playerObject = GameManager.Instance.GetPlayerObject();
        cinCam.Follow = playerObject.transform;
    }
}
