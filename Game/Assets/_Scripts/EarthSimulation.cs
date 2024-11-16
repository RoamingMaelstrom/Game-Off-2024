using SOEvents;
using UnityEngine;

public class EarthSimulation : MonoBehaviour
{
    [SerializeField] FloatSOEvent growthEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] float startPopulation = 8000;
    [SerializeField] float maxPopulation = 50000;
    [SerializeField] float currentPopulation;

    [SerializeField] int growthPerTick = 5;
    [SerializeField] float growthPercentPerTick = 0.0003f;
    [SerializeField] int maxPercentGrowth = 50;
    [SerializeField] float growthInterval = 1f;

    [SerializeField] int randomGrowthMinValue = 2;
    [SerializeField] int randomGrowthMaxValue = 12;
    [SerializeField] float randomGrowthProb = 0.1f;

    [SerializeField] float damageMultiplier = 1f;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.EARTH_SIMULATION;

    private float timer;
    private float randomUpperBound = 1000f;

    public float PopulationCapDeflator {get => 1f - (currentPopulation / maxPopulation);}

    private void Awake() {
        unlockTechEvent.AddListener(ProcessUnlock);
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
                case EffectType.POPULATION_GROWTH_TICK_RATE: growthInterval *= 1f + (effect.value / 100f); break;
                case EffectType.POPULATION_GROWTH_TICK_VALUE: {
                    growthPercentPerTick *= 1f + (effect.value / 100f);
                    growthPerTick = (int)(growthPerTick * (1f + (effect.value / 100f))) + 1;
                    break;
                }
                case EffectType.DAMAGE_REDUCTION: damageMultiplier *= 1f - (effect.value / 100f); break;
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
        currentPopulation += Random.Range(randomGrowthMinValue, randomGrowthMaxValue) * PopulationCapDeflator;
        growthEvent.Invoke(currentPopulation);
    }

    private void GrowTick() {
        currentPopulation += growthPerTick * PopulationCapDeflator;
        currentPopulation += Mathf.Clamp(growthPercentPerTick * currentPopulation, 0, maxPercentGrowth) * PopulationCapDeflator;
        growthEvent.Invoke(currentPopulation);
    }



}
