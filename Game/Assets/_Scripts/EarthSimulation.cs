using SOEvents;
using UnityEngine;

public class EarthSimulation : MonoBehaviour
{
    [SerializeField] FloatSOEvent gameLostEvent;
    [SerializeField] FloatSOEvent growthEvent;
    [SerializeField] FloatSOEvent playerDamagedEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] float startPopulation = 8000;
    [SerializeField] float populationLimit = 50000;
    public float currentPopulation;
    private float maxPopulationReached;

    [SerializeField] int growthPerTick = 5;
    [SerializeField] float growthPercentPerTick = 0.0003f;
    [SerializeField] int maxPercentGrowth = 50;
    [SerializeField] float growthInterval = 1f;

    [SerializeField] int randomGrowthMinValue = 2;
    [SerializeField] int randomGrowthMaxValue = 12;
    [SerializeField] float randomGrowthProb = 0.1f;

    [SerializeField] float damageMultiplier = 1f;
    [SerializeField] float emergencyRepopulationMultiplier = 0;
    [SerializeField] float emergencyRepopulationCap = 50f;

    [SerializeField] float populationGainedPassiveFlat;
    [SerializeField] float populationGainedPassiveExp;
    [SerializeField] float populationGainedRandom;
    [SerializeField] float populationGainedEmergency;
    [SerializeField] float populationGainedPopulationMultiplier;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.EARTH_SIMULATION;

    private float timer;
    private float randomUpperBound = 1000f;


    public float MaxPopulationReached {get => maxPopulationReached; private set{}}
    public float StartPopulation {get => startPopulation; private set{}}
    public float PopulationCapDeflator {get => 1f - (currentPopulation / populationLimit);}

    private void Awake() {
        growthEvent.AddListener(UpdateMaxPopulation);
        playerDamagedEvent.AddListener(UpdateMaxPopulation);
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void UpdateMaxPopulation(float arg0) {
        maxPopulationReached = Mathf.Max(maxPopulationReached, currentPopulation);
    }

    public void TakeDamage(float damageValue) {
        currentPopulation -= damageMultiplier * damageValue;
        playerDamagedEvent.Invoke(damageMultiplier * damageValue);
        currentPopulation = Mathf.Clamp(currentPopulation, 0, populationLimit);
        if (currentPopulation <= 0) gameLostEvent.Invoke(0f);
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.POPULATION_GROWTH_RANDOM_PROB: randomGrowthProb *= 1f + (effect.value / 100f); break;
                case EffectType.POPULATION_GROWTH_RANDOM_VALUE: {
                    randomGrowthMinValue = (int)(randomGrowthMinValue * (1f + (effect.value / 100f))) + 1;
                    randomGrowthMaxValue = (int)(randomGrowthMaxValue * (1f + (effect.value / 100f))) + 1;
                    break;
                }
                case EffectType.POPULATION_GROWTH_TICK_RATE: growthInterval *= 1f - (effect.value / 100f); break;
                case EffectType.POPULATION_GROWTH_TICK_VALUE: {
                    growthPercentPerTick *= 1f + (effect.value / 100f);
                    growthPerTick = (int)(growthPerTick * (1f + (effect.value / 100f))) + 1;
                    break;
                }
                case EffectType.DAMAGE_REDUCTION: damageMultiplier *= 1f - (effect.value / 100f); break;
                case EffectType.EMERGENCY_REPOPULATION: emergencyRepopulationMultiplier += effect.value * 100f; break;
                default: break;
            }
        }
    }

    private void Start() {
        currentPopulation = startPopulation;
        growthEvent.Invoke(currentPopulation);
    }

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        if (timer > growthInterval) {
            timer -= growthInterval;
            GrowTick();
        }

        if (Random.Range(0f, randomUpperBound) * Time.fixedDeltaTime < randomGrowthProb) {
            GrowRandom(); 
            randomUpperBound = Mathf.Min(randomUpperBound * 1.1f, 1000f);
        }
        else randomUpperBound = Mathf.Clamp(randomUpperBound * 0.999f, 500f, 1000f);
    }

    private void GrowRandom() {
        float randomGrowth = Random.Range(randomGrowthMinValue, randomGrowthMaxValue);
        currentPopulation += Random.Range(randomGrowthMinValue, randomGrowthMaxValue) * PopulationCapDeflator;
        growthEvent.Invoke(currentPopulation);

        populationGainedRandom += randomGrowth;
        populationGainedPopulationMultiplier += randomGrowth * (PopulationCapDeflator - (8f / populationLimit));
    }

    private void GrowTick() {
        float expGrowth = Mathf.Clamp(growthPercentPerTick * currentPopulation, 0, maxPercentGrowth);

        currentPopulation += growthPerTick * PopulationCapDeflator;
        currentPopulation += expGrowth * PopulationCapDeflator;


        float emergencyGrowth = Mathf.Clamp((maxPopulationReached - currentPopulation) * emergencyRepopulationMultiplier, 0, emergencyRepopulationCap);
        currentPopulation += emergencyGrowth;
        growthEvent.Invoke(currentPopulation);

        populationGainedPassiveFlat += growthPerTick;
        populationGainedPassiveExp += expGrowth;
        populationGainedEmergency += emergencyGrowth;
        populationGainedPopulationMultiplier += expGrowth * (PopulationCapDeflator - (8f / populationLimit));
        populationGainedPopulationMultiplier += growthPerTick * (PopulationCapDeflator - (8f / populationLimit));
    }


    private void OnApplicationQuit() {
        if (!Application.isEditor) return;
        Debug.Log("Population Growth Stats");
        Debug.Log(string.Format("Growth from Passive Flatrate - {0}", populationGainedPassiveFlat));
        Debug.Log(string.Format("Growth from Passive Exponential - {0}", populationGainedPassiveExp));
        Debug.Log(string.Format("Growth from Random Sources - {0}", populationGainedRandom));
        Debug.Log(string.Format("Growth from Emergency Sources - {0}", populationGainedEmergency));
        Debug.Log(string.Format("Growth from Population Multiplier - {0}", populationGainedPopulationMultiplier));
        Debug.Log(string.Format("Population - {0}", currentPopulation));
        Debug.Log("");
    }

}
