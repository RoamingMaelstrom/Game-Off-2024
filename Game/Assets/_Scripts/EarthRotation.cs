using UnityEngine;

public class EarthRotation : MonoBehaviour
{
    [SerializeField] Transform earthTransform;
    [SerializeField] Transform shadowTransform;
    [SerializeField] float earthRotationSpeed = 1f;
    [SerializeField] float shadowRotationSpeed = 0.05f;

    private void LateUpdate() {
        earthTransform.rotation = Quaternion.Euler(0f, 0f, earthTransform.rotation.eulerAngles.z - (earthRotationSpeed * Time.deltaTime));
        shadowTransform.rotation = Quaternion.Euler(0f, 0f, shadowTransform.rotation.eulerAngles.z + (shadowRotationSpeed * Time.deltaTime));
    }
}
