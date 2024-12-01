using SOEvents;
using UnityEngine;
using GliderAudio;

public class MunitionCreator : MonoBehaviour
{
    [SerializeField] MouseInfo mouseInfo;
    [SerializeField] FireOrderInfoSOEvent playerRequestMunitionEvent;
    [SerializeField] ObjectPoolMain objectPool;

    public float timer = 0;

    private void Start() {
        playerRequestMunitionEvent.AddListener(CreatePlayerMunition);
    }

    private void CreatePlayerMunition(FireOrderInfo fOI)
    {
        GameObject newMunition = objectPool.GetObject(fOI.munitionID);
        newMunition.transform.position = GetMunitionStartPos(fOI);

        Rigidbody2D munitionBody = newMunition.GetComponent<Rigidbody2D>();
        munitionBody.velocity = GetMunitionVelocity(fOI, newMunition.transform.position);
        newMunition.transform.rotation = Quaternion.Euler(0, 0, WeaponMath.Math.VectorToRotation(munitionBody.velocity));

        DamageDealer damageDealer = newMunition.GetComponent<DamageDealer>();
        damageDealer.SetLifeToCachedValue();
        damageDealer.ManuallyRestartDotDamageCoroutine();
        damageDealer.PlayAttachedSfx();
    }

    private Vector2 GetMunitionVelocity(FireOrderInfo fOI, Vector2 startPos) {
        Vector2 targetPos;
        float rotation = -fOI.createdBy.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        switch (fOI.targetingType)
        {
            case TargetingType.FIXED_WING: targetPos = startPos + (new Vector2(Mathf.Sin(rotation), Mathf.Cos(rotation)) * 1000); break;
            case TargetingType.AT_MOUSE: targetPos = mouseInfo.MousePosWorld; break;
            case TargetingType.AT_POINT: targetPos = fOI.targetPos; break;
            case TargetingType.TARGET_PREDICT: {
                if (fOI.target == null) targetPos = fOI.targetPos;
                else {  
                    targetPos = fOI.target.transform.position;
                    targetPos += fOI.target.velocity * ((Vector2)fOI.target.transform.position - startPos).magnitude / fOI.munitionSpeed; 
                }
                break;
            }
            default: targetPos = startPos + Random.insideUnitCircle.normalized * 100f; break;
        }

        // Ensures Multiple width shots behave as expected.
        targetPos += new Vector2((Mathf.Sin(-rotation) * fOI.targetOffset.y) + (Mathf.Cos(-rotation) * fOI.targetOffset.x),
                              (Mathf.Sin(-rotation) * fOI.targetOffset.x) + (Mathf.Cos(-rotation) * fOI.targetOffset.y));

        Vector2 output = WeaponMath.Math.GetDirectionVectorToTarget(startPos, targetPos) * fOI.munitionSpeed;
        if (fOI.parentVelocity.sqrMagnitude > 0) output += WeaponMath.Math.ProjectVectorOntoVector(fOI.parentVelocity, output);

        output += WeaponMath.Math.ApplyAccuracyCoefficientToVector(output, fOI.accuracyCoefficient);

        return output;
    }

    private Vector3 GetMunitionStartPos(FireOrderInfo fOI) {
        Vector3 output = fOI.createdBy.transform.position;
        float rotation = fOI.createdBy.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        output += new Vector3((Mathf.Sin(rotation) * fOI.offset.y) + (Mathf.Cos(rotation) * fOI.offset.x),
                              (Mathf.Sin(rotation) * fOI.offset.x) + (Mathf.Cos(rotation) * fOI.offset.y));
        return output;
    }
}
