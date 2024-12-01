using SOEvents;
using UnityEngine;

public class ScanningMission : MonoBehaviour
{
    [SerializeField] IntSOEvent missionCompleteEvent;
    [SerializeField] IntSOEvent startMissionEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;

    [SerializeField] TechObjectDisplay completionTOD;

    [SerializeField] EnemySpawner enemySpawner;

    public float scanDuration = 60f;
    [SerializeField] float spawnMultiplier = 2f;
    public float timeRemaining;

    public bool missionActive = false;
    [SerializeField] int missionScore = 5000;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.___VICTORY___;

    private void Awake() {
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects) {
            if (effect.effectType == EffectType.START_SCANNING_MISSION) {
                StartScanningMission();
                return;
            }
        }
    }

    private void StartScanningMission() {
        missionActive = true;
        timeRemaining = scanDuration;
        enemySpawner.spawnRateMultiplier *= spawnMultiplier;
        startMissionEvent.Invoke(1);
    }

    private void FixedUpdate() {
        if (!missionActive) return;
        timeRemaining -= Time.fixedDeltaTime;
        if (timeRemaining <= 0) {
            missionActive = false;
            enemySpawner.spawnRateMultiplier /= spawnMultiplier;
            completionTOD.techUnlockStatusEncoded = 64;
            missionCompleteEvent.Invoke(missionScore);
        }
    }
}

