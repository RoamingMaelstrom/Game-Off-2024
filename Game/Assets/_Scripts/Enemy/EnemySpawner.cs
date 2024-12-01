using System.Collections;
using SOEvents;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] IntSOEvent levelUpEvent;
    [SerializeField] IntSOEvent startMissionEvent;
    [SerializeField] IntSOEvent missionCompleteEvent;
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] WorldInfo worldInfo;
    [SerializeField] ObjectPoolMain objectPool;
    [SerializeField] EnemyController enemyController;

    [SerializeField] EnemyProfile[] enemyProfiles;
    [SerializeField] EnemyWave[] enemyWaves;
    [SerializeField] EnemyWave infiniteWave;
    [SerializeField] Difficulty difficultyObject;
    public int waveNumber;
    private EnemyWave currentWave;
    private SpawnTracker currentWaveSpawnTracker;
    private float waveTimeRemaining;

    public float spawnRateMultiplier = 1f;
    public int basicEnemyID;

    public float timer;

    [SerializeField] float reverseEngineerTimer;
    [SerializeField] float reverseEngineerSpawnRate = 1f;
    [SerializeField] int reverseEngineerEnemyID = 1;
    public bool reverseEngineerMissionActive = false;
    public Rigidbody2D probeBody;

    [SerializeField] float damageMultiplier = 1f;
    [SerializeField] float healthMultiplier = 1f;
    [SerializeField] float speedMultiplier = 1f;
    [SerializeField] float levelMultiplierRate = 0.025f;
    [SerializeField] int levelMultiplierRateStart = 40;
    [SerializeField] float infiniteWaveMultiplierGain = 0.005f;

    private bool isInfiniteWave = false;
    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.ENEMY_MANAGER;
    public float finaleTimer = 0;
    public float finaleDuration = 150;
    public bool finale = false;
    private int finaleWaveNumber = 0;
    [SerializeField] PrototypeLaser prototypeLaser;
    [SerializeField] TechObjectDisplay completionTOD;
    [SerializeField] string completedPrototypeSfx;
    [SerializeField] bool startMenuSpawning = false;

    private void Awake() {
        levelUpEvent.AddListener(CheckForMultiplierBoost);
        unlockTechEvent.AddListener(ProcessUnlock);

        healthMultiplier = difficultyObject.GetDifficultyStrengthForType(DifficultySpecifierType.HEALTH);
        damageMultiplier = difficultyObject.GetDifficultyStrengthForType(DifficultySpecifierType.DAMAGE);
        speedMultiplier = difficultyObject.GetDifficultyStrengthForType(DifficultySpecifierType.SPEED);

        spawnRateMultiplier = difficultyObject.GetDifficultyStrengthForType(DifficultySpecifierType.SPAWN_RATE);

        infiniteWaveMultiplierGain *= (healthMultiplier + damageMultiplier + speedMultiplier) / 3f;
        levelMultiplierRate *= (healthMultiplier + damageMultiplier + speedMultiplier) / 3f;
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects)
        {
            if (effect.effectType == EffectType.START_FINALE_MISSION) StartFinaleWaves();
            if (effect.effectType == EffectType.PROTOTYPE_WEAPON) {
                spawnRateMultiplier += 0.1f * difficultyObject.GetDifficultyStrengthForType(DifficultySpecifierType.SPAWN_RATE);
                damageMultiplier += 0.05f * difficultyObject.GetDifficultyStrengthForType(DifficultySpecifierType.DAMAGE);
            }
        }
    }

    private void StartFinaleWaves() {
        finale = true;
        finaleWaveNumber = 0;
        GliderAudio.SFX.PlayRelativeToListener(completedPrototypeSfx, Vector3.zero, 0.66f);
        startMissionEvent.Invoke(3);
    }

    private void CheckForMultiplierBoost(int level) {
        if (level < levelMultiplierRateStart) return;

        healthMultiplier += levelMultiplierRate;
        speedMultiplier += levelMultiplierRate;
    }

    private void Start() {
        LoadWave(0);
        SpawnEnemy(0, new Vector3(0, 90, 0));
        SpawnEnemy(0, new Vector3(100 * Mathf.Sign(Random.Range(-1f, 1f)), 60, 0));
        SpawnEnemy(0, new Vector3(50 * Random.Range(-1f, 1f), 150, 0));
    }

    private void FixedUpdate() {
        if (finale) {
            finaleTimer += Time.fixedDeltaTime;
            if (finaleTimer > 0 && finaleWaveNumber == 0) StartCoroutine(SpawnManyEnemies(1, 75, 3f));
            if (finaleTimer > 20 && finaleWaveNumber == 1) StartCoroutine(SpawnManyEnemies(2, 75, 5f));
            if (finaleTimer > 55 && finaleWaveNumber == 2) StartCoroutine(SpawnManyEnemies(3, 50, 10f));
            if (finaleTimer > 65 && finaleWaveNumber == 3) StartCoroutine(SpawnManyEnemies(4, 35, 10f));
            if (finaleTimer > 115 && finaleWaveNumber == 4) StartCoroutine(SpawnManyEnemies(5, 60, 10f));
            if (finaleTimer > finaleDuration) {
                prototypeLaser.UpgradePrototype();
                prototypeLaser.UpgradePrototype();
                prototypeLaser.UpgradePrototype();
                finale = false;
                completionTOD.techUnlockStatusEncoded = 64;
                missionCompleteEvent.Invoke(50000);
            }
        }


        if (reverseEngineerMissionActive) {
            reverseEngineerTimer -= Time.fixedDeltaTime;
            if (reverseEngineerTimer <= 0) {
                SpawnEnemy(reverseEngineerEnemyID);
                reverseEngineerTimer += 1f / reverseEngineerSpawnRate;
            }
        }

        if (isInfiniteWave) {
            healthMultiplier += infiniteWaveMultiplierGain * Time.fixedDeltaTime;
            speedMultiplier += infiniteWaveMultiplierGain * Time.fixedDeltaTime;
        }

        waveTimeRemaining -= Time.fixedDeltaTime;
        timer += Time.fixedDeltaTime;

        currentWaveSpawnTracker.IncrementTimers(Time.fixedDeltaTime * spawnRateMultiplier);
        for (int i = 0; i < currentWaveSpawnTracker.enemySpawnTimers.Length; i++)
        {
            float timeThreshold = Mathf.Lerp(currentWaveSpawnTracker.enemyStartSpawnIntervals[i],
             currentWaveSpawnTracker.enemyEndSpawnIntervals[i], (currentWave.duration - waveTimeRemaining) / currentWave.duration);
            float enemyTimer = currentWaveSpawnTracker.enemySpawnTimers[i];
            if (enemyTimer > timeThreshold) {
                SpawnEnemy(currentWaveSpawnTracker.enemyIDs[i]);
                currentWaveSpawnTracker.DecrementTimer(i, timeThreshold);
            }
        }

        if (waveTimeRemaining <= 0 && !isInfiniteWave) LoadWave(waveNumber);
    }

    private IEnumerator SpawnManyEnemies(int enemyID, int enemyCount, float duration) {
        finaleWaveNumber++;
        int c = 0;
        float interval = duration / enemyCount;

        while (c < enemyCount) {
            c++;
            SpawnEnemy(enemyID);
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnEnemy(int enemyProfileID) {
        GameObject newEnemy = objectPool.GetObject(basicEnemyID);
        Enemy enemy = newEnemy.GetComponent<Enemy>();
        EnemyProfile profile = enemyProfiles[enemyProfileID];

        enemy.enemyID = enemyProfileID;
        enemy.spriteRenderer.sprite = profile.sprite;
        enemy.health.maxHp = profile.health * healthMultiplier;
        enemy.health.ManualSetCurrentHp(enemy.health.maxHp);
        enemy.damageDealer.damageValue = profile.damage * damageMultiplier;
        enemy.transform.localScale = profile.scale;
        enemy.speed = profile.speed * speedMultiplier;

        if (!startMenuSpawning) {
            do {
                newEnemy.transform.position = Random.insideUnitCircle.normalized * worldInfo.gameWorldRadius * 1.33f;
            } while((newEnemy.transform.position - (probeBody.transform.position + (Vector3)(probeBody.velocity * 10f))).sqrMagnitude < 1600);
        }
        else newEnemy.transform.position = new Vector3(1110, Random.Range(-25, 25), 0);
        
        enemyController.AddEnemy(enemy);
    }

    private void SpawnEnemy(int enemyProfileID, Vector3 spawnPos) {
        GameObject newEnemy = objectPool.GetObject(basicEnemyID);
        Enemy enemy = newEnemy.GetComponent<Enemy>();
        EnemyProfile profile = enemyProfiles[enemyProfileID];

        enemy.enemyID = enemyProfileID;
        enemy.spriteRenderer.sprite = profile.sprite;
        enemy.health.maxHp = profile.health;
        enemy.health.ManualSetCurrentHp(enemy.health.maxHp);
        enemy.damageDealer.damageValue = profile.damage;
        enemy.transform.localScale = profile.scale;
        enemy.speed = profile.speed;

        newEnemy.transform.position = spawnPos;
        
        enemyController.AddEnemy(enemy);
    }


    public void LoadWave(int index) {
        if (index >= enemyWaves.Length) {
            currentWave = infiniteWave;
            waveTimeRemaining = currentWave.duration;
            currentWaveSpawnTracker = new SpawnTracker();
            currentWaveSpawnTracker.Configure(currentWave);
            isInfiniteWave = true;
            return;
        }
        currentWave = enemyWaves[index];
        waveTimeRemaining = currentWave.duration;
        currentWaveSpawnTracker = new SpawnTracker();
        currentWaveSpawnTracker.Configure(currentWave);
        waveNumber++;
    }

    [System.Serializable]
    private class SpawnTracker
    {
        public int[] enemyIDs;
        public float[] enemyStartSpawnIntervals;
        public float[] enemyEndSpawnIntervals;
        public float[] enemySpawnTimers;

        public void Configure(EnemyWave wave) {
            enemyIDs = new int[wave.spawnInfoArray.Length];
            enemyStartSpawnIntervals = new float[wave.spawnInfoArray.Length];
            enemyEndSpawnIntervals = new float[wave.spawnInfoArray.Length];
            enemySpawnTimers = new float[wave.spawnInfoArray.Length];

            for (int i = 0; i < enemyStartSpawnIntervals.Length; i++)
            {
                enemyIDs[i] = wave.spawnInfoArray[i].profile.enemyID;
                enemyStartSpawnIntervals[i] = Mathf.Clamp(1f / wave.spawnInfoArray[i].startSpawnRate, 0.05f, 100f);
                enemyEndSpawnIntervals[i] = Mathf.Clamp(1f / wave.spawnInfoArray[i].endSpawnRate, 0.05f, 100f);
                enemySpawnTimers[i] = 0;
            }
        }

        public void IncrementTimers(float dt) {
            for (int i = 0; i < enemySpawnTimers.Length; i++) enemySpawnTimers[i] += dt;
        }

        public void DecrementTimer(int index, float value) {
            enemySpawnTimers[index] -= value;
        }
    }

    private void OnApplicationQuit() {
        if (!Application.isEditor) return;
        Debug.Log("Wave Stats");
        Debug.Log(string.Format("Current Wave - {0}", waveNumber));
        Debug.Log(string.Format("Current Time - {0:n0}:{1}", timer / 60, timer % 60));
        Debug.Log("");
    }
}
