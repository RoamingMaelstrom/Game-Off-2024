using UnityEngine;
using SOEvents;
using System;
using System.Collections;

public class Weapon : MonoBehaviour
{
    [SerializeField] FireOrderInfoSOEvent playerRequestMunitionEvent;
    [SerializeField] Rigidbody2D parentBody;
    public float fireRate;
    public int fireWidth = 1;
    public int fireCount = 1;
    public int munitionID;
    [Range(0f, 1f)] public float accuracyCoefficient;
    public float munitionSpeed;
    [SerializeField] Vector2 offset;
    [SerializeField] TargetingType targetingType;

    private float timer;
    private FireOrderInfo fOI;

    private void Awake() {
        RecreateFOI();
    }

    public void RecreateFOI() {
        fOI = new()
        {
            createdBy = gameObject,
            offset = offset,
            munitionID = munitionID,
            accuracyCoefficient = accuracyCoefficient,
            targetingType = targetingType,
            munitionSpeed = munitionSpeed,
            parentVelocity = Vector2.zero
        };
    }

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
    }

    public void TryFire() {
        if (timer > 1f / fireRate) {
            timer = 0;
            if (fireCount == 1) FireSingle();
            else StartCoroutine(FireMultiple());
        }
    }

    private IEnumerator FireMultiple()
    {
        float interval = Mathf.Clamp(0.5f * fireCount / fireRate, 0.04f, 0.12f);
        for (int i = 0; i < fireCount; i++)
        {
            FireSingle();
            if (i + 1 < fireCount) yield return new WaitForSeconds(interval);
        } 
    }

    private void FireSingle() {
        for (int j = 0; j < fireWidth; j++)
        {
            Vector2 munitionOffset = offset + ((1 - fireWidth) * -0.25f * Vector2.left);
            if (parentBody) fOI.parentVelocity = parentBody.velocity;
            munitionOffset.x += j * 0.5f;
            fOI.offset = munitionOffset;
            fOI.targetOffset = ((1 - fireWidth) * -0.25f * Vector2.left) + (0.5f * j * Vector2.right);
            playerRequestMunitionEvent.Invoke(fOI);
        }
    }

    public void SetTarget(Rigidbody2D target) => fOI.target = target;
}


public class FireOrderInfo
{
    public GameObject createdBy;
    public Vector2 offset;
    public Rigidbody2D target;
    public Vector3 targetPos;
    public Vector2 targetOffset;
    public int munitionID;
    public float accuracyCoefficient;
    public TargetingType targetingType;
    public float munitionSpeed;
    public Vector2 parentVelocity;
}

public enum TargetingType
{
    FIXED_WING,
    AT_MOUSE,
    AT_POINT,
    TARGET_PREDICT
}
