using System;
using System.Collections;
using SOEvents;
using UnityEngine;

public class EnemyDespawner : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent enemyDestroyedEvent;
    [SerializeField] GameObjectFloatSOEvent enemyReachedEarthEvent;
    [SerializeField] GameObjectFloatSOEvent enemyReachedProbeEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;

    [SerializeField] ObjectPoolMain objectPoolMain;
    [SerializeField] PlayerLevel playerLevel;
    [SerializeField] EarthSimulation earthSimulation;
    
    [SerializeField] GameObject alienDeathPrefab;
    [SerializeField] GameObject alienHitProbePrefab;
    [SerializeField] GameObject alienHitEarthPrefab;
    [SerializeField] GameObject alienHitProbeExplosionPrefab;

    private float satelliteXpMultiplier = 0f;
    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.ENEMY_MANAGER;

    private int[] previousEnemiesObjectIDs = new int[32];
    private float[] accessTimes = new float[32];
    private int idsArrayIndex = 0;

    [SerializeField] bool startScene = false;



    private void Awake() {
        enemyDestroyedEvent.AddListener(HandleEnemyDestroyed);
        enemyReachedEarthEvent.AddListener(HandleEnemyReachedEarth);
        enemyReachedProbeEvent.AddListener(HandleEnemyReachedProbe);
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;
        foreach (var effect in tOD.techObject.effects)
        {
            if (effect.effectType == EffectType.SATELLITE_XP) satelliteXpMultiplier += effect.value / 100f;
        }
    }

    private void HandleEnemyReachedEarth(GameObject enemyObject, float arg1) {
        Enemy enemy = enemyObject.GetComponent<Enemy>();
        earthSimulation.TakeDamage(enemy.damageDealer.damageValue);
        SpawnAlienAnimation(enemy, alienHitEarthPrefab, transform, 1f);
        objectPoolMain.ReturnObject(enemyObject);
    }

    private void HandleEnemyDestroyed(GameObject enemyObject, float despawnValue) {
        // Handles case where multiple Event calls leads to enemy being passed through twice, and triggering XP/Animation twice
        int enemyObjectID = enemyObject.GetInstanceID();
        for (int i = 0; i < previousEnemiesObjectIDs.Length; i++)
        {
            if (enemyObjectID == previousEnemiesObjectIDs[i] && Mathf.Abs(Time.time - accessTimes[i]) < 2.5f) {
                accessTimes[i] = Time.time;
                objectPoolMain.ReturnObject(enemyObject);
                return;
            }
        }

        previousEnemiesObjectIDs[idsArrayIndex] = enemyObjectID;
        accessTimes[idsArrayIndex] = Time.time;

        idsArrayIndex = (idsArrayIndex + 1) % 32;

        

        SpawnAlienAnimation(enemyObject.GetComponent<Enemy>(), alienDeathPrefab, transform, despawnValue > 10 ? 0.2f : 1f);
        
        if ((despawnValue == 1 || despawnValue == 11) && !startScene) playerLevel.GainXpPlayerKill(enemyObject.GetComponent<Health>().maxHp);
        if (despawnValue == 2 && satelliteXpMultiplier > 0 && !startScene) playerLevel.GainXpSatelliteKill(enemyObject.GetComponent<Health>().maxHp * satelliteXpMultiplier);
        objectPoolMain.ReturnObject(enemyObject);
    }

    private void HandleEnemyReachedProbe(GameObject enemyObject, float arg1) {
        SpawnAlienAnimation(enemyObject.GetComponent<Enemy>(), alienHitProbePrefab, transform, 1f);
        Instantiate(alienHitProbeExplosionPrefab, enemyObject.transform.position, Quaternion.identity);
        objectPoolMain.ReturnObject(enemyObject);
    }

    private static void SpawnAlienAnimation(Enemy enemy, GameObject prefab, Transform parent, float animationDurationMultiplier) {
        GameObject deathObject = Instantiate(prefab, enemy.transform.position, enemy.transform.rotation, parent);
        EnemyDeathAnimation deathAnimation = deathObject.GetComponent<EnemyDeathAnimation>();
        deathAnimation.Setup(enemy.spriteRenderer.sprite, enemy.body.velocity, enemy.transform.localScale, animationDurationMultiplier);
    }
}
