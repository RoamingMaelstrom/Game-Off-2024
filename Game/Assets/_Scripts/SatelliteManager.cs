using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class SatelliteManager : MonoBehaviour
{
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] Constellation satellite1Constellation;
    [SerializeField] Constellation satellite2Constellation;
    [SerializeField] Constellation satellite3Constellation;
    [SerializeField] Constellation spaceStationConstellation;
    [SerializeField] Weapon[] spaceStationWeapons;

    private readonly TechUpgradeHandler satelliteManagerHandler = TechUpgradeHandler.SATELLITE_MANAGER;
    private readonly TechUpgradeHandler satellite1Handler = TechUpgradeHandler.SATELLITE_1;
    private readonly TechUpgradeHandler satellite2Handler = TechUpgradeHandler.SATELLITE_2;
    private readonly TechUpgradeHandler satellite3Handler = TechUpgradeHandler.SATELLITE_3;
    private readonly TechUpgradeHandler spaceStationHandler = TechUpgradeHandler.SPACE_STATION;

    private void Awake() {
        unlockTechEvent.AddListener(ProcessUnlock);
        satellite1Constellation.SetInactiveSatellitesPositions(Vector3.one * 10000f);
        satellite2Constellation.SetInactiveSatellitesPositions(Vector3.one * 10000f);
        satellite3Constellation.SetInactiveSatellitesPositions(Vector3.one * 10000f);
        spaceStationConstellation.SetInactiveSatellitesPositions(Vector3.one * 10000f);
    }

    private void ProcessUnlock(TechObjectDisplay tOD)
    {
        if (tOD.techObject.effects[0].effectType == EffectType.UNLOCK_SATELLITE_1) {
            satellite1Constellation.AddSatellites(satellite1Constellation.startSatelliteCount);
            return;
        }
        if (tOD.techObject.effects[0].effectType == EffectType.UNLOCK_SATELLITE_2) {
            satellite2Constellation.AddSatellites(satellite2Constellation.startSatelliteCount);
            return;
        }
        if (tOD.techObject.effects[0].effectType == EffectType.UNLOCK_SATELLITE_3) {
            satellite3Constellation.AddSatellites(satellite3Constellation.startSatelliteCount);
            return;
        }
        if (tOD.techObject.effects[0].effectType == EffectType.UNLOCK_SPACE_STATION) {
            spaceStationConstellation.AddSatellites(1);
            return;
        }


        if (tOD.techObject.ContainsHandler(satellite1Handler)) ProcessSatelliteShotUpgrade(tOD, satellite1Constellation);
        else if (tOD.techObject.ContainsHandler(satellite2Handler)) ProcessSatelliteShotUpgrade(tOD, satellite2Constellation);
        else if (tOD.techObject.ContainsHandler(satellite3Handler)) ProcessSatelliteLaserbeamUpgrade(tOD, satellite3Constellation);
        else if (tOD.techObject.ContainsHandler(spaceStationHandler)) ProcessSpaceStationUpgrade(tOD, spaceStationWeapons);
        else if (tOD.techObject.ContainsHandler(satelliteManagerHandler)) ProcessAllSatellitesUpgrade(tOD);
    }

    private static void ProcessSpaceStationUpgrade(TechObjectDisplay tOD, Weapon[] weapons)
    {
        SatelliteDetector[] satelliteDetectors = new SatelliteDetector[weapons.Length];
        for (int i = 0; i < weapons.Length; i++)
        {
            satelliteDetectors[i] = weapons[i].transform.GetChild(0).GetComponent<SatelliteDetector>();
        }

        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.ACCURACY: {
                    foreach (var weapon in weapons) weapon.accuracyCoefficient += (1f - weapon.accuracyCoefficient) * effect.value / 100f;
                    break;
                }
                case EffectType.DAMAGE: {
                    foreach (var weapon in weapons) weapon.munitionID++;
                    break;
                }
                case EffectType.FIRE_RATE: {
                    foreach (var weapon in weapons) weapon.fireRate *= 1f + (effect.value / 100f);
                    break;
                }
                case EffectType.MUNITION_COUNT: {
                    foreach (var weapon in weapons) weapon.fireCount = effect.value;
                    break;
                }
                case EffectType.SATELLITE_RANGE: {
                    foreach (var detector in satelliteDetectors) detector.range *= 1f + (effect.value / 100f);
                    break;
                }
                default: break;
            }
        }

        foreach (var weapon in weapons)
        {
            weapon.RecreateFOI();
        }
    }

    private static void ProcessSatelliteShotUpgrade(TechObjectDisplay tOD, Constellation constellation)
    {
        Weapon[] satelliteWeapons = constellation.GetComponentsInSatellites<Weapon>();
        SatelliteDetector[] satelliteDetectors = constellation.GetComponentsInSatellites<SatelliteDetector>();

        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.ACCURACY: {
                    foreach (var weapon in satelliteWeapons) weapon.accuracyCoefficient += (1f - weapon.accuracyCoefficient) * effect.value / 100f;
                    break;
                }
                case EffectType.DAMAGE: {
                    foreach (var weapon in satelliteWeapons) weapon.munitionID++;
                    break;
                }
                case EffectType.FIRE_RATE: {
                    foreach (var weapon in satelliteWeapons) weapon.fireRate *= 1f + (effect.value / 100f);
                    break;
                }
                case EffectType.MUNITION_COUNT: {
                    foreach (var weapon in satelliteWeapons) weapon.fireWidth = effect.value;
                    break;
                }
                case EffectType.MUNITION_SPEED: {
                    foreach (var weapon in satelliteWeapons) weapon.munitionSpeed *= 1f + (effect.value / 100f);
                    break;
                }
                case EffectType.BURST_FIRE: {
                    foreach (var weapon in satelliteWeapons) weapon.fireCount = effect.value;
                    break;
                }
                case EffectType.SATELLITE_RANGE: {
                    foreach (var detector in satelliteDetectors) detector.range *= 1f + (effect.value / 100f);
                    break;
                }
                case EffectType.SATELLITE_COUNT: {
                    if (constellation.Active()) constellation.AddSatellites(effect.value);
                    else constellation.startSatelliteCount += effect.value;
                    break;
                }
                default: break;
            }
        }

        foreach (var weapon in satelliteWeapons)
        {
            weapon.RecreateFOI();
        }
    }

    private static void ProcessSatelliteLaserbeamUpgrade(TechObjectDisplay tOD, Constellation constellation)
    {
        LaserbeamWeapon[] satelliteLaserbeams = constellation.GetComponentsInSatellites<LaserbeamWeapon>();

        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.ACCURACY: {
                    foreach (var weapon in satelliteLaserbeams) weapon.damageDealer.dotInterval *= 1f - (effect.value / 400f);
                    break;
                }
                case EffectType.DAMAGE: {
                    foreach (var weapon in satelliteLaserbeams) weapon.storedDotValue *= 1f + (effect.value / 100f);
                    break;
                }
                case EffectType.DROP_OFF: {
                    foreach (var weapon in satelliteLaserbeams) weapon.minDamageMultiplier *= 1f + (effect.value / 100f);
                    break;
                }
                case EffectType.FIRE_RATE: {
                    foreach (var weapon in satelliteLaserbeams) weapon.damageDealer.dotInterval *= 1f - (effect.value / 100f);
                    break;
                }
                case EffectType.SATELLITE_RANGE: {
                    foreach (var weapon in satelliteLaserbeams) weapon.range *= 1f + (effect.value / 100f);
                    break;
                }
                case EffectType.SATELLITE_COUNT: {
                    if (constellation.Active()) constellation.AddSatellites(effect.value);
                    else constellation.startSatelliteCount += effect.value;
                    break;
                }
                default: break;
            }
        }
    }

    

    private void ProcessAllSatellitesUpgrade(TechObjectDisplay tOD)
    {
        ProcessSatelliteShotUpgrade(tOD, satellite1Constellation);
        ProcessSatelliteShotUpgrade(tOD, satellite2Constellation);
        ProcessSatelliteLaserbeamUpgrade(tOD, satellite3Constellation);
        ProcessSpaceStationUpgrade(tOD, spaceStationWeapons); 
    }

    private void LateUpdate() {
        satellite1Constellation.Rotate(Time.deltaTime);
        satellite2Constellation.Rotate(Time.deltaTime);
        satellite3Constellation.Rotate(Time.deltaTime);
        spaceStationConstellation.Rotate(Time.deltaTime);
    }
}


[System.Serializable]
public class Constellation 
{
    [SerializeField] List<GameObject> satellitesInUse = new();
    [SerializeField] List<GameObject> satellitesInactive = new();
    public float constellationRotation;
    public float rotationSpeed = 1f;
    public float orbitRadius = 50f;
    public int startSatelliteCount;

    public bool Active() => satellitesInUse.Count > 0;

    public void SetInactiveSatellitesPositions(Vector3 pos) {
        foreach (var satellite in satellitesInactive)
        {
            satellite.transform.position = pos;
        }
    }

    public void Rotate(float dt) {
        constellationRotation -= rotationSpeed * dt / 60f;
        for (int i = 0; i < satellitesInUse.Count; i++)
        {
            float pRotation = satellitesInUse[i].transform.rotation.eulerAngles.z % 360f;
            float satRotation = i * Mathf.PI * 2f / satellitesInUse.Count;
            satRotation += constellationRotation;
            satRotation *= Mathf.Rad2Deg;
            satRotation %= 360f;

            if (pRotation - satRotation > 180f) satRotation += 360f;
            if (pRotation - satRotation < -180f) satRotation -= 360f;

            satRotation = Mathf.Lerp(pRotation, satRotation, dt / 2f);

            satellitesInUse[i].transform.rotation = Quaternion.Euler(0, 0, satRotation);
            satellitesInUse[i].transform.position = new Vector3(Mathf.Sin(-satRotation * Mathf.Deg2Rad), Mathf.Cos(satRotation * Mathf.Deg2Rad)) * orbitRadius;
        }
    }

    public void AddSatellites(int num) {
        int inactiveCount = satellitesInactive.Count;

        for (int i = 0; i < num; i++)
        {
            if (satellitesInactive.Count == 0) Debug.Log("Need more satellites");
            satellitesInUse.Add(satellitesInactive[inactiveCount - 1]);
            satellitesInactive.RemoveAt(inactiveCount - 1);
            inactiveCount --;
        }
        
        Rotate(2f);
    }

    public T[] GetComponentsInSatellites<T>() {
        T[] output = new T[satellitesInUse.Count + satellitesInactive.Count];
        int c = 0;
        foreach (var satellite in satellitesInUse)
        {
            output[c] = satellite.GetComponentInChildren<T>();
            c++;
        }

        foreach (var satellite in satellitesInactive)
        {
            output[c] = satellite.GetComponentInChildren<T>();
            c++;
        }

        return output;
    }
}
