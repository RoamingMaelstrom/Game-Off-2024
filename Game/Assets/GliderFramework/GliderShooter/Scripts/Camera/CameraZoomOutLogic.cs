using UnityEngine;
using Cinemachine;

public class CameraZoomOutLogic : MonoBehaviour
{
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] CinemachineVirtualCamera vCam;
    [SerializeField] float defaultCameraZoom = 15f;
    [SerializeField] float speedMultiplier = 2f;
    [Tooltip("Speed at which player has to be travelling for the zoom effect to take effect")]
    [SerializeField] float startSpeed = 10f;

    private void Update() 
    {
        if (playerBody.velocity.magnitude < startSpeed) vCam.m_Lens.OrthographicSize = defaultCameraZoom;
        else
        {
            float speedZoomOut = Mathf.Sqrt(playerBody.velocity.magnitude) - Mathf.Sqrt(startSpeed);
            vCam.m_Lens.OrthographicSize = defaultCameraZoom + (speedZoomOut * speedMultiplier);
        }
    }

}
