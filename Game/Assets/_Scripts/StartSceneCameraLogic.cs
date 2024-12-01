using UnityEngine;

public class StartSceneCameraLogic : MonoBehaviour
{
    [SerializeField] float defaultMinZoom = 64f;
    [SerializeField] float defaultMaxZoom = 128f;
    [SerializeField] float oscillationDuration = 30f;

    [SerializeField] Camera mainCamera;
    [SerializeField] Vector3 earthFocusPosition;
    [SerializeField] BasePlayerController basePlayerController;
    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] Vector3 playerStartPos;

    [SerializeField] float playerCameraZoom = 24f;

    private bool earthFocus = true;
    private float zoomInterval;

    private float timer = 0;

    private void Start() {
        mainCamera.orthographicSize = defaultMaxZoom;
        zoomInterval = 0.5f * Mathf.PI / oscillationDuration;
    }

    public void FocusPlayer() {
        earthFocus = false;
        playerBody.transform.position = playerStartPos;
    }

    public void FocusEarth() {
        earthFocus = true;
    }

    private void LateUpdate() {
        timer += Time.unscaledDeltaTime;
        if (earthFocus) {
            basePlayerController.controlsActive = false;
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, defaultMinZoom + Mathf.Abs(Mathf.Cos(zoomInterval * timer) * (defaultMaxZoom - defaultMinZoom)), Time.unscaledDeltaTime);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, earthFocusPosition, Time.unscaledDeltaTime);
        }
        else {
            basePlayerController.controlsActive = true;
            
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, playerCameraZoom, Time.unscaledDeltaTime * 2f);
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, new Vector3(1060, 0, -10), Time.unscaledDeltaTime * 3f);
        }
    }

}
