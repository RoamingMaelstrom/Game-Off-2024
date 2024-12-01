using SOEvents;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent enemyDestroyedEvent;
    [SerializeField] IntSOEvent missionCompleteEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] EarthSimulation earthSimulation;
    [SerializeField] PlayerLevel playerLevel;

    [SerializeField] int pointsPerSecond = 10;
    [SerializeField] int pointsPerKill = 25;
    [SerializeField] int pointsPerMaxPopulation = 10;
    [SerializeField] float populationHealthModifier = 1f;
    [SerializeField] int pointsPerLevel = 50;

    private int killCount = 0;

    public int pointsFromMissions = 0;
    public int pointsFromVictoryUnlocks = 0;
    private float timer = 0;
    private bool scoreFrozen = false;

    private int killsFrozenScore;
    private int levelFrozenScore;
    private int populationFrozenScore;
    private int timeFrozenScore;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.___VICTORY___;


    public int GetKillsScore() => scoreFrozen ? killsFrozenScore : killCount * pointsPerKill;
    public int GetPopulationScore() {
        if (scoreFrozen) return populationFrozenScore;
        float output = pointsPerMaxPopulation * (earthSimulation.MaxPopulationReached - earthSimulation.StartPopulation);
        output *= 1f + (populationHealthModifier * earthSimulation.currentPopulation / earthSimulation.MaxPopulationReached);
        return (int)output;
    }

    public int GetTimeScore() => scoreFrozen ? timeFrozenScore : (int)timer * pointsPerSecond;
    public int GetLevelScore() => scoreFrozen ? levelFrozenScore : playerLevel.Level * pointsPerLevel;

    private void Awake() {
        enemyDestroyedEvent.AddListener(RegisterKill);
        missionCompleteEvent.AddListener(AddToPointsFromMission);
        missionCompleteEvent.AddListener(CheckForScoreFreeze);
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void CheckForScoreFreeze(int points)
    {
        if (points != 50000) return;
        CalculateFrozenScores();
        scoreFrozen = true;
    }

    private void CalculateFrozenScores()
    {
        killsFrozenScore = GetKillsScore();
        levelFrozenScore = GetLevelScore();
        populationFrozenScore = GetPopulationScore();
        timeFrozenScore = GetTimeScore();
    }

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
    }

    private void RegisterKill(GameObject arg0, float arg1) {
        if (Random.Range(playerLevel.Level / 2, playerLevel.Level) > 45) return;
        killCount++;
    }

    private void AddToPointsFromMission(int points) {
        pointsFromMissions += points;
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects)
        {
            if (effect.effectType == EffectType.ADD_SCORE) pointsFromVictoryUnlocks += effect.value;
        }
    }
}
