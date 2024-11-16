using UnityEngine;

public class LaserbeamWeapon : MonoBehaviour
{
    public DamageDealer damageDealer;
    [SerializeField] LineRenderer lineRenderer;
    public Transform target;
    public float range;
    [SerializeField] float rotationSpeed = 90f;
    [SerializeField] float angle;
    public float minDamageMultiplier = 0.25f;
    private float angleToTarget;

    public float storedDotValue;

    private void Start() {
        storedDotValue = damageDealer.dotDamageValue;
    }

    // Update is called once per frame
    void FixedUpdate() {
        UpdateAngle();

        if (target == null || Mathf.Abs(angle - angleToTarget) > 1f) {
            lineRenderer.positionCount = 0;
            damageDealer.transform.position = Vector3.one * 100000f;
            return;
        }

        damageDealer.transform.position = target.position;
        damageDealer.dotDamageValue = storedDotValue * (1f - Mathf.Clamp((target.transform.position - transform.position).magnitude / range, 0, 1f - minDamageMultiplier));
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, target.position);
    }

    private void UpdateAngle() {
        if (target == null) return;

        angleToTarget = WeaponMath.Math.VectorToRotation(target.position - transform.position) % 360f;
        angle %= 360f;
        float nAngle = angle;

        float dAngle = angle - angleToTarget;

        if (Mathf.Abs(dAngle) > 0.1f) {
            if (dAngle < - 180 || (dAngle > 0 && dAngle < 180)) nAngle -= Mathf.Min(Mathf.Abs(dAngle), rotationSpeed * Time.fixedDeltaTime);
            else nAngle += Mathf.Min(Mathf.Abs(dAngle), rotationSpeed * Time.fixedDeltaTime);
        }

        angle = nAngle % 360f;
    }
}
