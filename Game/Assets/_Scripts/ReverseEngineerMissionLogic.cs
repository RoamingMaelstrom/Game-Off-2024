using System.Collections.Generic;
using System.Collections;
using SOEvents;
using UnityEngine;

public class ReverseEngineerMissionLogic : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent enemyDestroyedEvent;
    [SerializeField] IntSOEvent missionCompleteEvent;
    [SerializeField] IntSOEvent startMissionEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] TechObjectDisplay completionTOD;
    [SerializeField] WorldInfo worldInfo;
    [SerializeField] EnemySpawner enemySpawner;

    [SerializeField] Rigidbody2D playerBody;
    [SerializeField] GameObject[] pickupPrefabs;

    public int currentPickups = 0;
    public int pickupsRequired = 10;


    [SerializeField] float pickupAttractionRadius = 15f;
    [SerializeField] float pickupPickedUpRadius = 3f;
    [SerializeField] float maxPickupAcceleration = 100f;
    [SerializeField] int missionScore = 2500;
    [SerializeField] int enemyID;
    [SerializeField] string pickupSfx;
    [SerializeField] string pickupAmountReachedSfx;

    private List<Rigidbody2D> pickups = new();
    public bool missionActive = false;
    private bool spawningPickups = false;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.___VICTORY___;

    private void Awake() {
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects) {
            if (effect.effectType == EffectType.START_COLLECTION_MISSION) {
                StartReverseEngineerMission();
                return;
            }
        }
    }

    private void FixedUpdate() {
        if (!missionActive) return;

        for (int i = pickups.Count - 1; i >= 0; i--)
        {
            Rigidbody2D pickup = pickups[i];
            float dPos = (pickup.transform.position - playerBody.transform.position).magnitude;
            if (dPos < pickupPickedUpRadius) {
                pickups.RemoveAt(i);
                GliderAudio.SFX.PlayAtPoint(pickupSfx, pickup.transform.position, 0.66f);
                Destroy(pickup.gameObject);
                IncrementCurrentPickups();
                break;
            }
            else if (dPos < pickupAttractionRadius) {
                pickup.velocity += (Vector2)(playerBody.transform.position - pickup.transform.position).normalized * maxPickupAcceleration * Time.fixedDeltaTime * dPos / pickupAttractionRadius;
            }
        }

        if (!spawningPickups && playerBody.transform.position.sqrMagnitude <= worldInfo.earthRadius * worldInfo.earthRadius * 1.1f) EndMission();
    }

    private void EndMission()
    {
        missionActive = false;
        completionTOD.techUnlockStatusEncoded = 64;
        missionCompleteEvent.Invoke(missionScore);
        enemySpawner.reverseEngineerMissionActive = false;
    }

    private void IncrementCurrentPickups()
    {
        currentPickups++;
        if (currentPickups >= pickupsRequired) {
            spawningPickups = false;
            GliderAudio.SFX.PlayRelativeToListener(pickupAmountReachedSfx, Vector3.zero);
            for (int i = pickups.Count - 1; i >= 0; i--) Destroy(pickups[i].gameObject);
            pickups.Clear();
        }
    }

    private void StartReverseEngineerMission() {
        missionActive = true;
        spawningPickups = true;
        enemySpawner.reverseEngineerMissionActive = true;
        enemyDestroyedEvent.AddListener(SpawnMissionPickups);
        startMissionEvent.Invoke(0);
    }

    private void SpawnMissionPickups(GameObject enemyObject, float arg1)
    {
        if (!missionActive || !spawningPickups) return;
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        if (enemy.enemyID != enemyID) return;

        int c = Random.Range(-1f, 1f) > 0 ? 2 : 1;
        GameObject pickupObject;
        for (int i = 0; i < c; i++)
        {
            pickupObject = Instantiate(pickupPrefabs[Random.Range(0, pickupPrefabs.Length)], enemyObject.transform.position, Quaternion.identity, transform);
            pickups.Add(pickupObject.GetComponent<Rigidbody2D>());
            pickups[^1].velocity = Random.insideUnitCircle * 15f;
        }
    }
}
