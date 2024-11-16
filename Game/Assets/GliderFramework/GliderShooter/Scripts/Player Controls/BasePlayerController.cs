using UnityEngine;
using UnityEngine.InputSystem;
using SOEvents;
using System;
using System.Collections;

public class BasePlayerController : MonoBehaviour
{
    [SerializeField] GameObjectSOEvent playerStartTargetingEvent;
    [SerializeField] SOEvent playerStopTargetingEvent;
    [SerializeField] SOEvent togglePauseScreenEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;

    [SerializeField] MouseInfo mouseInfo;

    [SerializeField] public float playerThrust = 10000f;
    [SerializeField] public float playerMaxSpeed = 25f;
    [SerializeField] public float playerBoostMultiplier = 1.5f;
    [SerializeField] float boostLerpSpeed = 2f;
    [SerializeField] float tunnelBoostDistance = 10f;
    [SerializeField] float tunnelBoostDuration = 0.5f;
    [SerializeField] float tunnelBoostTriggerSensitivity = 0.25f;
    [SerializeField] public float maxAngularVelocity = 360f;
    [Tooltip("When player rotates around object, determines how much it determines by mouse position (0=None, 1=100%)")] 
    [SerializeField] float targetModeMouseLerp = 0.2f;
    [Tooltip("When player Targeting Object, dictates the maximum amount (degrees) their rotation can be from facing the target")]
    [SerializeField] float maxRotationDifference = 30f;

    [SerializeField] bool boostingEnabled = true;
    [SerializeField] bool tunnelBoostEnabled = true;

    private Vector2 movement;
    private float spacebarDown;

    public bool isLockedOn {get; private set;}
    GameObject targetObject;

    [SerializeField] public Rigidbody2D playerBody;

    public bool useShipDirectionThrust = true;

    private RotationConstraints rotationConstraints;

    private float actualMaxSpeed;
    private float timeSpacebarDown;
    private float tunnelBoostCooldown;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.PLAYER_CONTROLLER;


    private void Awake() 
    {
        playerStartTargetingEvent.AddListener(OnPlayerStartTargetingEvent);
        playerStopTargetingEvent.AddListener(OnPlayerStopTargetingEvent);
        rotationConstraints = new(maxAngularVelocity, maxRotationDifference, targetModeMouseLerp);
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    // Todo: Add Boost Jump

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.BOOST_THRUST: playerBoostMultiplier *= 1f + (effect.value / 100f); break;
                case EffectType.NORMAL_THRUST:  {
                    playerThrust *= 1f + (effect.value / 100f);
                    playerMaxSpeed *= 1f + (effect.value / 100f);
                    break;
                }
                case EffectType.ROTATION_SPEED: maxAngularVelocity *= 1f + (effect.value / 100f); break;
                case EffectType.WARP_JUMP_DISTANCE: break; // Todo: Implement
                case EffectType.UNLOCK_BOOST: boostingEnabled = true; break;
                case EffectType.UNLOCK_TUNNEL_WARP: break; // Todo: Implement
                default: break;
            }
        }
    }

    public void FixedUpdate() 
    {
        if (!playerBody) return;

        HandleThrust();
        HandleRotation();

        if (spacebarDown != 0) timeSpacebarDown += Time.fixedDeltaTime;
        else {
            if (timeSpacebarDown < tunnelBoostTriggerSensitivity && tunnelBoostEnabled && timeSpacebarDown > 0 && tunnelBoostCooldown < 0) StartCoroutine(TunnelBoost());
            timeSpacebarDown = 0;
        }
        tunnelBoostCooldown -= Time.fixedDeltaTime;
    }

    private IEnumerator TunnelBoost() {
        float rotation = playerBody.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Vector2 dir = new Vector2(Mathf.Sin(-rotation), Mathf.Cos(rotation)).normalized;
        if (dir.magnitude == 0) dir = Vector2.up;
        tunnelBoostCooldown = tunnelBoostDuration * 1.1f;

        Vector2 startPos = playerBody.transform.position;
        float speed = tunnelBoostDistance / tunnelBoostDuration;
        float timer = 0;
        while (timer < tunnelBoostDuration) {
            timer += Time.deltaTime;
            playerBody.transform.position += Mathf.Sin(Mathf.PI * timer / tunnelBoostDuration) * speed * Time.deltaTime * (Vector3)dir;
            yield return new WaitForEndOfFrame();
        }
        actualMaxSpeed = playerMaxSpeed * playerBoostMultiplier;
        rotation = playerBody.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        playerBody.velocity = Vector2.Lerp(new Vector2(Mathf.Sin(-rotation), Mathf.Cos(rotation)).normalized * actualMaxSpeed, playerBody.velocity.normalized * actualMaxSpeed, 0.5f);
    }

    void OnBoost(InputValue value) => spacebarDown = value.Get<float>();

    void OnMove(InputValue value) => movement = value.Get<Vector2>();

    void OnPause(InputValue value) => togglePauseScreenEvent.Invoke();


    private void OnPlayerStopTargetingEvent()
    {
        isLockedOn = false;
        targetObject = null;
    }

    private void OnPlayerStartTargetingEvent(GameObject target)
    {
        isLockedOn = true;
        targetObject = target;
    }

    private void HandleThrust()
    {
        bool isBoosting = spacebarDown != 0 && boostingEnabled;
        float thrust = isBoosting ? playerThrust * playerBoostMultiplier : playerThrust;
        float targetMaxSpeed = isBoosting ? playerMaxSpeed * playerBoostMultiplier : playerMaxSpeed;
        actualMaxSpeed = Mathf.Lerp(actualMaxSpeed, targetMaxSpeed, Time.fixedDeltaTime * boostLerpSpeed);

        if (useShipDirectionThrust) PlayerThrust.CalculateShipDirectionalThrust(playerBody, movement, thrust, actualMaxSpeed);
        else PlayerThrust.CalculateNonDirectionalThrust(playerBody, movement, thrust, actualMaxSpeed);
    }

    private void HandleRotation()
    {
         PlayerRotation.CalculateShipRotation(playerBody.transform, mouseInfo.MousePosWorld, targetObject, rotationConstraints);
    }


    public static class PlayerThrust
    {
        public static void CalculateShipDirectionalThrust(Rigidbody2D playerBody, Vector2 dirVector, float playerThrust, float playerMaxSpeed)
        {
            Vector2 force = dirVector * playerThrust;
            playerBody.AddRelativeForce(force);
            playerBody.velocity = playerBody.velocity.normalized * Mathf.Min(playerBody.velocity.magnitude, playerMaxSpeed);
        }

        public static void CalculateNonDirectionalThrust(Rigidbody2D playerBody, Vector2 dirVector, float playerThrust, float playerMaxSpeed)
        {
            Vector2 force = dirVector * playerThrust;
            playerBody.AddForce(force);
            playerBody.velocity = playerBody.velocity.normalized * Mathf.Min(playerBody.velocity.magnitude, playerMaxSpeed);
        }
    }

    public static class PlayerRotation
    {
        public static void CalculateShipRotation(Transform playerTransform, Vector2 mousePosWorld, GameObject targetObject, RotationConstraints constraints)
        {
            float currentRotation = (playerTransform.rotation.eulerAngles.z + 360f) % 360;
            float targetRotation = targetObject == null ? GetRotationAroundMouse(playerTransform.position, mousePosWorld) : 
             GetRotationAroundTarget(playerTransform.position, mousePosWorld, targetObject.transform.position, constraints.targetModeMouseLerp, constraints.maxTargetRotationDifference);
            float deltaRotation = currentRotation - targetRotation;

            if (Mathf.Abs(deltaRotation) < 0.1f) return;

            float newRotation = currentRotation;

            if (deltaRotation < - 180 || (deltaRotation > 0 && deltaRotation < 180)) newRotation -= Mathf.Min(Mathf.Abs(deltaRotation), constraints.maxAngularVelocity * Time.fixedDeltaTime);
            else newRotation += Mathf.Min(Mathf.Abs(deltaRotation), constraints.maxAngularVelocity * Time.fixedDeltaTime);
            playerTransform.rotation = Quaternion.Euler(0, 0, newRotation);
        }

        private static float GetRotationAroundMouse(Vector2 playerPos, Vector2 point)
        {
            Vector2 deltaPos = point - playerPos;
            float targetRotation = - Mathf.Atan2(deltaPos.x, deltaPos.y) * Mathf.Rad2Deg;
            return (targetRotation + 360f) % 360;
        }

        private static float GetRotationAroundTarget(Vector3 playerPos, Vector3 mousePositionInWorld, Vector3 targetPos, float targetModeMouseLerp, float maxRotationDifference)
        {
            targetPos = Vector2.Lerp(targetPos, mousePositionInWorld, targetModeMouseLerp);
            Vector2 deltaPos = targetPos - playerPos;

            float midValidRotation = - Mathf.Atan2(deltaPos.x, deltaPos.y) * Mathf.Rad2Deg;
            if (midValidRotation < 0) midValidRotation += 360f;

            float targetRotation = - Mathf.Atan2(deltaPos.x, deltaPos.y) * Mathf.Rad2Deg;
            if (targetRotation < 0) targetRotation += 360f;

            if (targetRotation - midValidRotation > 180) midValidRotation += 360f;
            else if (targetRotation - midValidRotation < -180) midValidRotation -= 360f;

            float upperBoundRotation = midValidRotation + maxRotationDifference;
            float lowerBoundRotation = midValidRotation - maxRotationDifference;

            return Mathf.Clamp(targetRotation, lowerBoundRotation, upperBoundRotation);
        }
    }

    public class RotationConstraints {
        public float maxAngularVelocity;
        public float maxTargetRotationDifference;
        public float targetModeMouseLerp;

        public RotationConstraints(float maxAngularVelocity, float maxTargetRotationDifference, float targetModeMouseLerp) {
            this.maxAngularVelocity = maxAngularVelocity;
            this.maxTargetRotationDifference = maxTargetRotationDifference;
            this.targetModeMouseLerp = targetModeMouseLerp;
        }
    }
}
