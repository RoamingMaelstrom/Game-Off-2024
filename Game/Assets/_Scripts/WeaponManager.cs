using SOEvents;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent playerMunitionDespawnEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] ObjectPoolMain objectPool;
    [SerializeField] PlayerFiring playerFiring;
    [SerializeField] Weapon playerWeapon1;
    [SerializeField] Weapon playerWeapon2;
    [SerializeField] LaserbeamWeapon playerWeapon3;
    [SerializeField] Weapon playerWeapon4;
    [SerializeField] LaserbeamWeapon[] satelliteLaserbeamWeapons;
    [SerializeField] GameObject[] explosionPrefabs;

    private readonly TechUpgradeHandler Weapon1Handler = TechUpgradeHandler.WEAPON_1;
    private readonly TechUpgradeHandler Weapon2Handler = TechUpgradeHandler.WEAPON_2;
    private readonly TechUpgradeHandler Weapon3Handler = TechUpgradeHandler.WEAPON_3;
    private readonly TechUpgradeHandler Weapon4Handler = TechUpgradeHandler.WEAPON_4;
    private readonly TechUpgradeHandler AllWeaponHandler = TechUpgradeHandler.PLAYER_WEAPON_MANAGER;


    [SerializeField] float pierceProbability = 0f;


    private void Awake() {
        playerMunitionDespawnEvent.AddListener(HandleDespawn);
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void HandleDespawn(GameObject munition, float arg1) {
        if (arg1 == 1000) Instantiate(explosionPrefabs[0], munition.transform.position, Quaternion.identity);
        
        if (pierceProbability <= 0) objectPool.ReturnObject(munition);
        else if (Random.Range(0f, 1f) > pierceProbability) objectPool.ReturnObject(munition);
        else {
            DamageDealer dD = munition.GetComponent<DamageDealer>();
            dD.alive = true;
            dD.life = 1;
        }
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (tOD.techObject.effects[0].effectType == EffectType.UNLOCK_WEAPON_2) {
            playerFiring.AddWeapon(playerWeapon2);
            return;
        }
        if (tOD.techObject.effects[0].effectType == EffectType.UNLOCK_WEAPON_3) {
            playerWeapon3.gameObject.SetActive(true);
            return;
        }
        if (tOD.techObject.effects[0].effectType == EffectType.UNLOCK_WEAPON_4) {
            playerFiring.AddWeapon(playerWeapon4);
            return;
        }


        if (tOD.techObject.ContainsHandler(Weapon1Handler)) ProcessWeaponUpgrade(tOD, playerWeapon1);
        else if (tOD.techObject.ContainsHandler(Weapon2Handler)) ProcessWeaponUpgrade(tOD, playerWeapon2);
        else if (tOD.techObject.ContainsHandler(Weapon3Handler)) ProcessWeapon3Unlock(tOD, playerWeapon3);
        else if (tOD.techObject.ContainsHandler(Weapon4Handler)) ProcessWeaponUpgrade(tOD, playerWeapon4);
        else if (tOD.techObject.ContainsHandler(AllWeaponHandler)) ProcessAllWeaponsUnlock(tOD);
    }

    private static void ProcessWeaponUpgrade(TechObjectDisplay tOD, Weapon weapon)
    {
        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.ACCURACY: weapon.accuracyCoefficient += (1f - weapon.accuracyCoefficient) * (effect.value / 100f); break;
                case EffectType.BURST_FIRE: weapon.fireCount = effect.value; break;
                case EffectType.DAMAGE: weapon.munitionID++; break;
                case EffectType.FIRE_RATE: weapon.fireRate *= 1f + (effect.value / 100f); break;
                case EffectType.MUNITION_COUNT: weapon.fireCount = effect.value; break;
                case EffectType.MUNITION_SPEED: weapon.munitionSpeed *= 1f + (effect.value / 100f); break;
                default: break;
            }
        }

        weapon.RecreateFOI();
    }

    private static void ProcessWeapon3Unlock(TechObjectDisplay tOD, LaserbeamWeapon weapon)
    {
        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.DROP_OFF: weapon.minDamageMultiplier += (1f - weapon.minDamageMultiplier) * effect.value; break;
                case EffectType.DAMAGE: {
                    weapon.storedDotValue *= 1f + (effect.value / 100f); break;
                }
                case EffectType.FIRE_RATE: weapon.damageDealer.dotInterval *= 1f - (effect.value / 100f); break;
                default: break;
            }
        }
    }

    private void ProcessAllWeaponsUnlock(TechObjectDisplay tOD)
    {
        foreach (var effect in tOD.techObject.effects)
        {
            if (effect.effectType == EffectType.PIERCE_PROBABILITY) pierceProbability = effect.value / 100f;
        }

        ProcessWeaponUpgrade(tOD, playerWeapon1);
        ProcessWeaponUpgrade(tOD, playerWeapon2);
        ProcessWeapon3Unlock(tOD, playerWeapon3);
        ProcessWeaponUpgrade(tOD, playerWeapon4);
    }
}
