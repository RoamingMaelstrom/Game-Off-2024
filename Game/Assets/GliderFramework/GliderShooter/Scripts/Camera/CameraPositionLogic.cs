using UnityEngine;

public class CameraPositionLogic : MonoBehaviour
{
    [SerializeField] WorldInfo worldInfo;
    [SerializeField] PlayerTargeting playerTargetingScript;
    [SerializeField] Rigidbody2D playerBody;


    [SerializeField] float velocityCameraPositionMultiplier = 0.5f;
    [SerializeField] float zeroLerpDistance = 50f;



    Vector3 tMinus1Pos = new Vector3();
    Vector3 tMinus2Pos = new Vector3();
    Vector3 currentPos = new Vector3();

    float tMinus1 = 0.5f;

    Transform playerTransform;
    GameObject target;
    
    private void Start() 
    {
        playerTransform = playerBody.transform;
    }

    private void LateUpdate() 
    {
        tMinus2Pos = tMinus1Pos;
        tMinus1Pos = currentPos;
        Vector3 centrePosition = playerTransform.position;
        if (playerTargetingScript.isTargeting)
        {
            target = playerTargetingScript.target;
            float t = Mathf.Clamp((zeroLerpDistance - (playerTransform.position - target.transform.position).magnitude) / zeroLerpDistance, 0, 0.5f);
            t = Mathf.Lerp(t, tMinus1, Mathf.Clamp(Mathf.Abs(t - tMinus1) / Mathf.Max(t, 0.001f), 0f, 0.5f));
            tMinus1 = t;
            centrePosition = Vector3.Lerp(playerTransform.position, target.transform.position, t);
        }

        currentPos = centrePosition + ((Vector3)playerBody.velocity * velocityCameraPositionMultiplier);
        currentPos = Vector3.Lerp(currentPos, tMinus1Pos, 0.5f);
        currentPos = Vector3.Lerp(currentPos, tMinus2Pos, 0.25f);

        transform.position = currentPos;
        transform.position = transform.position.magnitude > worldInfo.gameWorldRadius ? transform.position.normalized * worldInfo.gameWorldRadius : transform.position;
        tMinus2Pos = tMinus1Pos;
        tMinus1Pos = currentPos;
    }
    
}
