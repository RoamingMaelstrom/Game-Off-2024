using UnityEngine;
using UnityEngine.InputSystem;
using SOEvents;
using System.Collections;
using GliderSave;
public class BasePlayerController : MonoBehaviour
{
    [SerializeField] FloatSaveObject sfxVolume;
    [SerializeField] GameObjectSOEvent playerStartTargetingEvent;
    [SerializeField] SOEvent playerStopTargetingEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;

    [SerializeField] MouseInfo mouseInfo;
    [SerializeField] TechPanelLogic techPanelLogic;

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

    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] public Rigidbody2D playerBody;
    [SerializeField] ParticleSystem boostParticles;
    [SerializeField] ParticleSystem thrustParticles;
    [SerializeField] ParticleSystem tunnelBoostParticles;

    [SerializeField] AudioSource boostAudioSource;
    [SerializeField] AudioSource thrustAudioSource;
    [SerializeField] float boostMaxVolume = 0.25f;
    [SerializeField] float thrustMaxVolume = 0.75f;
    [SerializeField] float volumeGainMultiplier = 3f;
    [SerializeField] string[] warpSfxs;

    public bool controlsActive = true;
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

        boostAudioSource.volume = 0;
        thrustAudioSource.volume = 0;
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.BOOST_THRUST: break;
                case EffectType.NORMAL_THRUST:  {
                    playerThrust *= 1f + (effect.value / 100f);
                    playerMaxSpeed *= 1f + (effect.value / 100f);
                    break;
                }
                case EffectType.ROTATION_SPEED: maxAngularVelocity *= 1f + (effect.value / 100f); break;
                case EffectType.WARP_JUMP_DISTANCE: tunnelBoostDistance *= 1f + (effect.value / 100f); break;
                case EffectType.UNLOCK_BOOST: boostingEnabled = true; break;
                case EffectType.UNLOCK_TUNNEL_WARP: tunnelBoostEnabled = true; break;
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

    private void Update() {
        float sfxSettingVolume = sfxVolume.GetValue();

        if (Time.timeScale <= 0 || movement.sqrMagnitude <= 0.1f) {
            UpdateSourceVolume(boostAudioSource, false, volumeGainMultiplier * Time.unscaledDeltaTime, boostMaxVolume * sfxSettingVolume);
            UpdateSourceVolume(thrustAudioSource, false, volumeGainMultiplier * Time.unscaledDeltaTime, thrustMaxVolume * sfxSettingVolume);
        }

        bool isBoosting =  spacebarDown != 0 && boostingEnabled;

        if (isBoosting) {
            UpdateSourceVolume(boostAudioSource, true, boostMaxVolume * volumeGainMultiplier * Time.unscaledDeltaTime, boostMaxVolume * sfxSettingVolume);
            UpdateSourceVolume(thrustAudioSource, false, thrustMaxVolume * volumeGainMultiplier * Time.unscaledDeltaTime, thrustMaxVolume * sfxSettingVolume);
        }
        else {
            UpdateSourceVolume(boostAudioSource, false, boostMaxVolume * volumeGainMultiplier * Time.unscaledDeltaTime, boostMaxVolume * sfxSettingVolume);
            UpdateSourceVolume(thrustAudioSource, true, thrustMaxVolume * volumeGainMultiplier * Time.unscaledDeltaTime, thrustMaxVolume * sfxSettingVolume);
        }
    }

    private static void UpdateSourceVolume(AudioSource source, bool louder, float volumeChangeRate, float maxVolume) {
        source.volume = Mathf.Clamp(source.volume + (volumeChangeRate * (louder ? 1f : -1f)), 0, maxVolume);
        if (!source.isPlaying) source.Play();
    }

    private IEnumerator TunnelBoost() {
        float rotation = playerBody.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
        Color startColour = playerRenderer.color;
        playerRenderer.color = startColour * 0.5f;
        tunnelBoostParticles.Play();
        GliderAudio.SFX.PlayRelativeToTransform(warpSfxs[Random.Range(0, warpSfxs.Length)], playerBody.transform);

        Vector2 dir = new Vector2(Mathf.Sin(-rotation), Mathf.Cos(rotation)).normalized;
        if (dir.magnitude == 0) dir = Vector2.up;
        tunnelBoostCooldown = tunnelBoostDuration * 1.1f;

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
        playerRenderer.color = startColour;
    }

    void OnBoost(InputValue value) {
        if (!controlsActive) spacebarDown = 0;
        else spacebarDown = value.Get<float>();
    }


    void OnMove(InputValue value) {
        if (!controlsActive) movement = Vector2.zero;
        movement = value.Get<Vector2>();
    }

    //void OnPause(InputValue value) => togglePauseScreenEvent.Invoke();

    void OnToggleTechPanel(InputValue value) {
        if (!controlsActive || techPanelLogic == null) return;
         techPanelLogic.ToggleTechPanel();
    }

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

        boostParticles.transform.rotation = playerBody.transform.rotation;
        boostParticles.transform.localPosition = -movement.normalized * 0.5f;

        if (isBoosting && !boostParticles.isEmitting) boostParticles.Play();
        else if (!isBoosting && boostParticles.isEmitting) boostParticles.Stop();

        thrustParticles.transform.rotation = playerBody.transform.rotation; //Quaternion.Euler(0, 0, WeaponMath.Math.VectorToRotation(movement));
        thrustParticles.transform.localPosition = -movement.normalized * 0.5f;

        if (movement.magnitude > 0 && !thrustParticles.isEmitting) thrustParticles.Play();
        else if (movement.magnitude == 0 && thrustParticles.isEmitting) thrustParticles.Stop();

        //if (isBoosting && boostAudioSource.clip != boostClip) boostAudioSource.clip = boostClip;
        //else if (!isBoosting && boostAudioSource.clip != thrustClip) boostAudioSource.clip = thrustClip;
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
