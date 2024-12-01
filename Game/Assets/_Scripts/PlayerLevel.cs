using System.Collections.Generic;
using JetBrains.Annotations;
using SOEvents;
using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] Float2SOEvent gainXpEvent;
    [SerializeField] IntSOEvent levelUpEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] EarthSimulation earthSimulation;
    [SerializeField] Difficulty difficultyObject;

    [SerializeField] int level = 1;

    private float xp;
    [SerializeField] float passiveXpTick = 10f;
    [SerializeField] float tickInterval = 0.5f;
    [SerializeField] float xpPerKillMultiplier = 1f;
    [SerializeField] float xpPerKillCap = 2500f;
    [SerializeField] float xpPerKillAboveCapPower = 0.85f;
    [SerializeField] float populationBaseXpMultiplier = 3f;
    [SerializeField] float populationMultiplierPow = 0.75f;
    private float timer;

    [SerializeField] List<LevelBoundMarker> levelBoundMarkers = new();
    [SerializeField] List<float> xpNeededPerLevel = new();

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.PLAYER_LEVEL;

    [SerializeField] float passiveXpTotal;
    [SerializeField] float playerKillXpTotal;
    [SerializeField] float satelliteKillXpTotal;
    [SerializeField] float populationMultiplierXpTotal;


    private void Awake() {
        unlockTechEvent.AddListener(ProcessUnlock);
        populationBaseXpMultiplier *= difficultyObject.GetDifficultyStrengthForType(DifficultySpecifierType.XP);
        xpPerKillMultiplier *= difficultyObject.GetDifficultyStrengthForType(DifficultySpecifierType.XP);
    }

    public int Level {get => level; private set{}}
    public float PopulationXpMultiplier {get => 1f + (populationBaseXpMultiplier * (1f - Mathf.Pow(earthSimulation.PopulationCapDeflator, populationMultiplierPow))); private set{}}
    public float XpNeededToLevel {get => xpNeededPerLevel[level]; private set{}}
    public float XpPercent {get => xp / XpNeededToLevel; private set {}}

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.XP_PASSIVE_GAIN: passiveXpTick *= 1f + (effect.value / 100f); break;
                case EffectType.XP_KILL_GAIN: xpPerKillMultiplier *= 1f + (effect.value / 100f); break;
                case EffectType.POPULATION_XP_GAIN_MODIFIER: populationBaseXpMultiplier *= 1f + (effect.value / 100f); break;
                default: break;
            }
        }
    }

    private void Start() {
        GenerateXpNeededPerLevel();
        gainXpEvent.Invoke(level, 0);
    }

    private void GenerateXpNeededPerLevel() {
        xpNeededPerLevel.Add(0);
        for (int i = 0; i < levelBoundMarkers.Count - 1; i++) 
        {
            LevelBoundMarker upper = levelBoundMarkers[i+1];
            LevelBoundMarker lower = levelBoundMarkers[i];
            int levelDelta = upper.level - lower.level;

            for (float j = 0; j < levelDelta; j++)
            {
                xpNeededPerLevel.Add(lower.xpNeeded + ((upper.xpNeeded - lower.xpNeeded) * (j / levelDelta)));
            }
        }
    }

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        if (timer > tickInterval) {
            GainXpTick();
            timer -= tickInterval;
        }

    }

    private void LevelUp() {
        xp -= xpNeededPerLevel[level];
        level ++;
        levelUpEvent.Invoke(level);
    }

    private void GainXpTick() {
        xp += passiveXpTick * PopulationXpMultiplier;

        passiveXpTotal += passiveXpTick;
        populationMultiplierXpTotal += passiveXpTick * (PopulationXpMultiplier - 1);

        gainXpEvent.Invoke(level, XpPercent);
        if (xp > XpNeededToLevel) LevelUp();
    }

    public void GainXpPlayerKill(float pointValue) {
        float value = Mathf.Clamp(pointValue * xpPerKillMultiplier, 0, xpPerKillCap);
        if (value >= xpPerKillCap) value += Mathf.Clamp(Mathf.Pow((pointValue * xpPerKillMultiplier) - xpPerKillCap, xpPerKillAboveCapPower), 0, xpPerKillCap);

        xp += value * PopulationXpMultiplier;
        playerKillXpTotal += value;
        populationMultiplierXpTotal += value * (PopulationXpMultiplier - 1);

        gainXpEvent.Invoke(level, XpPercent);
        if (xp > XpNeededToLevel) LevelUp();
    }

    public void GainXpSatelliteKill(float pointValue) {
        float value = Mathf.Clamp(pointValue, 0, xpPerKillCap);
        if (value >= xpPerKillCap) value += Mathf.Clamp(Mathf.Pow(pointValue - xpPerKillCap, xpPerKillAboveCapPower), 0, xpPerKillCap);

        xp += value * PopulationXpMultiplier;

        satelliteKillXpTotal += value;
        populationMultiplierXpTotal += value * (PopulationXpMultiplier - 1);

        gainXpEvent.Invoke(level, XpPercent);
        if (xp > XpNeededToLevel) LevelUp();
    }

    private void OnApplicationQuit() {
        if (!Application.isEditor) return;
        Debug.Log("Player XP Stats");
        Debug.Log(string.Format("XP gained passively - {0}", passiveXpTotal));
        Debug.Log(string.Format("XP gained from Player Alien Kills - {0}", playerKillXpTotal));
        Debug.Log(string.Format("XP gained from Satellite Alien Kills - {0}", satelliteKillXpTotal));
        Debug.Log(string.Format("XP gained from population Multiplier - {0}", populationMultiplierXpTotal));
        Debug.Log(string.Format("Level - {0}", level));
        Debug.Log(string.Format("XP - {0}", passiveXpTotal + playerKillXpTotal + satelliteKillXpTotal + populationMultiplierXpTotal));
        Debug.Log("");
    }
}

[System.Serializable]
public class LevelBoundMarker {
    public int level;
    public float xpNeeded;
}
