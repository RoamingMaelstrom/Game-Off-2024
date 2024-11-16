using System;
using System.Collections;
using SOEvents;
using Unity.VisualScripting;
using UnityEngine;

public class EarthDefence : MonoBehaviour
{
    [SerializeField] FireOrderInfoSOEvent requestPlayerMunitionEvent;
    [SerializeField] GameObjectFloatSOEvent playerDamagedEvent;
    [SerializeField] EnemyController enemyController;
    [SerializeField] bool fireMissilesEnabled = true;
    [SerializeField] bool retaliationEnabled = true;
    [SerializeField] float spawnDistanceFromEarthCentre = 35f;

    [SerializeField] int fireMissileMunitionID;
    [SerializeField] int retaliationMunitionID;

    [SerializeField] float fireMissileCooldown = 0.75f;
    [SerializeField] float retaliationCooldown = 30;
    [SerializeField] int retaliationMissileCount = 16;
    [SerializeField] float retaliationDuration = 2f;

    private float retaliationTimer = 0;
    private float fireMissileTimer = 0;

    private void Awake() {
        playerDamagedEvent.AddListener(TryRetaliate);
    }

    private void TryRetaliate(GameObject arg0, float arg1) {
        if (retaliationTimer < 0 && retaliationEnabled) StartCoroutine(Retaliate());
    }

    private IEnumerator Retaliate() {
        yield return new WaitForSeconds(0.1f);

        float timer = 0.1f;
        float interval = (retaliationDuration - 0.1f) / retaliationMissileCount;

        while(timer < retaliationDuration) {
            FireMissile(retaliationMunitionID);
            fireMissileTimer += interval;
            yield return new WaitForSeconds(interval);
        }
        yield return null;
    }

    private void FixedUpdate() {

        fireMissileTimer -= Time.fixedDeltaTime;
        retaliationTimer -= Time.fixedDeltaTime;

        if (fireMissileTimer < 0 && fireMissilesEnabled) FireMissile(fireMissileMunitionID);
    }

    private void FireMissile(int munitionID) {
        Vector2 targetPos = enemyController.GetRandomEnemyPosition();
        transform.position = targetPos.normalized * spawnDistanceFromEarthCentre;
        FireOrderInfo fOI = new() {
            munitionID = munitionID,
            createdBy = gameObject,
            targetPos = transform.position * 10f,
            munitionSpeed = 20f,
            accuracyCoefficient = 0.95f,
            targetingType = TargetingType.AT_POINT
        };

        requestPlayerMunitionEvent.Invoke(fOI);
        fireMissileTimer = fireMissileCooldown;
    }
}
