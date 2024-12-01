using System.Collections;
using SOEvents;
using UnityEngine;

public class ProbeMission : MonoBehaviour
{
    [SerializeField] IntSOEvent missionCompleteEvent;
    [SerializeField] GameObjectFloatSOEvent probeDestroyedEvent;
    [SerializeField] IntSOEvent startMissionEvent;

    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] TechObjectDisplay completionTOD;

    [SerializeField] WorldInfo worldInfo;
    public Rigidbody2D probeBody;
    public Health probeHealth;
    [SerializeField] EnemyController enemyController;

    [SerializeField] float startSpeed = 1f;
    [SerializeField] float missionLength = 90f;
    [SerializeField] int missionScore = 10000;

    public bool missionActive = false;
    private bool missionCompleted = false;
    private float countdown;
    private float acceleration;

    [SerializeField] TechObjectDisplay[] probeTODs;
    [SerializeField] GameObject probeExplosionPrefab;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.___VICTORY___;

    private void Awake() {
        probeDestroyedEvent.AddListener(FailMission);
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void FailMission(GameObject arg0, float arg1) {
        if (missionCompleted) return;
        missionActive = false;
        foreach (var tod in probeTODs)
        {
            tod.techUnlockStatusEncoded &= 15;
        }
        enemyController.trackProbe = false;
        enemyController.SetVelocitiesToEarth();
        Instantiate(probeExplosionPrefab, probeBody.transform.position, Quaternion.identity);
        StartCoroutine(DespawnDelayed(0.5f, 0.75f));
        probeBody.gameObject.SetActive(false);
    }

    private IEnumerator DespawnDelayed(float delay, float velocityMultiplier)
    {
        probeBody.velocity *= velocityMultiplier;
        yield return new WaitForSeconds(delay);
        probeBody.transform.position = Vector3.zero;
        probeBody.gameObject.SetActive(false);
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects) {
            if (effect.effectType == EffectType.START_PROBE_MISSION) {
                StartProbeMission();
                return;
            }
        }
    }

    private void StartProbeMission() {
        missionActive = true;
        probeBody.gameObject.SetActive(true);
        acceleration = 2 * ((worldInfo.gameWorldRadius * 1.2f) - worldInfo.earthRadius - (startSpeed * missionLength)) / (missionLength * missionLength);
        probeBody.transform.position = Random.insideUnitCircle.normalized * worldInfo.earthRadius;
        probeBody.velocity = (Vector2)probeBody.transform.position.normalized * startSpeed;
        probeBody.transform.rotation = Quaternion.Euler(0, 0, WeaponMath.Math.VectorToRotation(probeBody.velocity));

        Health probeHealth = probeBody.GetComponent<Health>();
        probeHealth.ManualSetCurrentHp(probeHealth.maxHp);

        enemyController.trackProbe = true;
        countdown = missionLength;
        startMissionEvent.Invoke(2);
    }

    private void FixedUpdate() {
        if (!missionActive) return;
        probeBody.velocity += probeBody.velocity.normalized * acceleration * Time.fixedDeltaTime;
        countdown -= Time.fixedDeltaTime;
        if (countdown <= 0) SucceedProbeMission();
    }

    private void SucceedProbeMission() {
        enemyController.trackProbe = false;
        enemyController.SetVelocitiesToEarth();
        missionCompleteEvent.Invoke((int)(missionScore * probeHealth.GetCurrentHp() / probeHealth.maxHp));
        missionActive = false;
        StartCoroutine(DespawnDelayed(10f, 2f));
        completionTOD.techUnlockStatusEncoded = 64;
        missionCompleted = true;
    }
}
