using System.Collections;
using SOEvents;
using UnityEngine;

public class EarthDefence : MonoBehaviour
{
    [SerializeField] FireOrderInfoSOEvent requestPlayerMunitionEvent;
    [SerializeField] FloatSOEvent playerDamagedEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
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

    [SerializeField] float missileStartSpeed = 15f;

    private float retaliationTimer = 0;
    private float fireMissileTimer = 0;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.EARTH_DEFENCE;

    private void Awake() {
        playerDamagedEvent.AddListener(TryRetaliate);
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;
        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.UNLOCK_EARTH_MISSILES: fireMissilesEnabled = true; break;
                case EffectType.UNLOCK_RETALIATION: {
                    retaliationEnabled = true; 
                    retaliationCooldown = effect.value;
                    break;
                }

                case EffectType.FIRE_RATE: fireMissileCooldown *= 1f - (effect.value / 100f); break;
                case EffectType.MUNITION_SPEED: missileStartSpeed *= 1f + (effect.value / 100f); break;
                case EffectType.DAMAGE: fireMissileMunitionID++; break;

                case EffectType.RETALIATION_COOLDOWN: retaliationCooldown *= 1f - (effect.value / 100f); break;
                case EffectType.RETALIATION_COUNT: retaliationMissileCount += effect.value; break;
                case EffectType.RETALIATION_NUKES: {
                    fireMissileMunitionID++;
                    retaliationMunitionID++; 
                    break;
                }

                default: break;
            }
        }
    }

    private void TryRetaliate(float arg1) {
        if (retaliationTimer < 0 && retaliationEnabled) StartCoroutine(Retaliate());
    }

    private IEnumerator Retaliate() {
        retaliationTimer = retaliationCooldown;
        yield return new WaitForSeconds(0.1f);

        float timer = 0.1f;
        float interval = (retaliationDuration - 0.1f) / retaliationMissileCount;

        while(timer < retaliationDuration) {
            FireMissile(retaliationMunitionID, 0.25f);
            timer += interval;
            yield return new WaitForSeconds(interval);
        }

        yield return null;
    }

    private void FixedUpdate() {
        fireMissileTimer -= Time.fixedDeltaTime;
        retaliationTimer -= Time.fixedDeltaTime;

        if (fireMissileTimer < 0 && fireMissilesEnabled) FireMissile(fireMissileMunitionID, 0.6f);
    }

    private void FireMissile(int munitionID, float accuracyCoefficient) {
        Vector2 targetPos = enemyController.GetRandomEnemyPosition();
        transform.position = targetPos.normalized * spawnDistanceFromEarthCentre;
        FireOrderInfo fOI = new() {
            munitionID = munitionID,
            createdBy = gameObject,
            targetPos = transform.position * 10f,
            munitionSpeed = missileStartSpeed,
            accuracyCoefficient = accuracyCoefficient,
            targetingType = TargetingType.AT_POINT
        };

        requestPlayerMunitionEvent.Invoke(fOI);
        fireMissileTimer = fireMissileCooldown;
    }
}
