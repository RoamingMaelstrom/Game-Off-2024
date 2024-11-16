using System.Collections.Generic;
using SOEvents;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] Float2SOEvent gainXpEvent;
    [SerializeField] IntSOEvent levelUpEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] EarthSimulation earthSimulation;

    [SerializeField] int level = 1;

    private float xp;
    [SerializeField] float passiveXpTick = 10f;
    [SerializeField] float tickInterval = 0.5f;
    // Todo: Add XP from player kills
    [SerializeField] float xpPerKill = 100f;
    [SerializeField] float populationBaseXpMultiplier = 3f;
    [SerializeField] float populationMultiplierPow = 0.75f;
    private float timer;

    [SerializeField] List<LevelBoundMarker> levelBoundMarkers = new();
    [SerializeField] List<float> xpNeededPerLevel = new();

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.PLAYER_LEVEL;
    private float PopulationXpMultiplier {get => 1f + (populationBaseXpMultiplier * (1f - Mathf.Pow(earthSimulation.PopulationCapDeflator, populationMultiplierPow)));}

    private void Awake() {
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.XP_PASSIVE_GAIN: passiveXpTick *= 1f + (effect.value / 100f); break;
                case EffectType.XP_KILL_GAIN: xpPerKill *= 1f + (effect.value / 100f); break;
                case EffectType.POPULATION_XP_GAIN_MODIFIER: populationBaseXpMultiplier *= 1f + (effect.value / 100f); break;
                default: break;
            }
        }
    }

    private void Start() {
        GenerateXpNeededPerLevel();
        gainXpEvent.Invoke(level, xp);
    }

    private void GenerateXpNeededPerLevel() {
        xpNeededPerLevel.Add(0);
        for (int i = 0; i < levelBoundMarkers.Count - 1; i++) 
        {
            LevelBoundMarker upper = levelBoundMarkers[i+1];
            LevelBoundMarker lower = levelBoundMarkers[i];
            int levelDelta = upper.level - lower.level;

            // Todo: Make increase non-linear, (greater xp needed at higher levels within a bound)
            for (float j = 0; j < levelDelta; j++)
            {
                xpNeededPerLevel.Add(lower.xpNeeded + ((upper.xpNeeded - lower.xpNeeded) * (j / levelDelta)));
            }
        }
    }

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        if (timer > tickInterval) {
            xp += passiveXpTick * PopulationXpMultiplier;
            timer -= tickInterval;
            if (xp > xpNeededPerLevel[level]) LevelUp();
            gainXpEvent.Invoke(level, xp / xpNeededPerLevel[level]);
        }

    }

    private void LevelUp() {
        xp -= xpNeededPerLevel[level];
        level ++;
        levelUpEvent.Invoke(level);
    }
}

[System.Serializable]
public class LevelBoundMarker {
    public int level;
    public float xpNeeded;
}
