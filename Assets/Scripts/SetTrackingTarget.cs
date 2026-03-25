using Unity.Cinemachine;
using UnityEngine;

public class SetTrackingTarget : MonoBehaviour
{
    private GameObject playerObject;
    private CinemachineCamera cinCam;

    public void SetupTarget()
    {
        cinCam = GetComponent<CinemachineCamera>();
        playerObject = GameManager.Instance.GetPlayerObject();
        cinCam.Follow = playerObject.transform;
    }
}
