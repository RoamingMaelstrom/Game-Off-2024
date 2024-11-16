using SOEvents;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] ObjectPoolMain objectPool;
    [SerializeField] EnemyController enemyController;
    [SerializeField] float enemySpawnDistance = 100f;
    
    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.ENEMY_MANAGER;


    private void Awake() {
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects)
        {
            if (effect.effectType == EffectType.ALIEN_SPAWN_DISTANCE) enemySpawnDistance *= 1f + (effect.value / 100f); 
        }
    }

    public int testEnemyID;


    public float spawnInterval = 2f;
    public float timer;

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        if (timer > spawnInterval) {
            SpawnEnemy();
            timer -= spawnInterval;
        }
    }

    private void SpawnEnemy() {
        GameObject newEnemy = objectPool.GetObject(testEnemyID);
        Enemy enemy = newEnemy.GetComponent<Enemy>();
        enemy.health = enemy.GetComponent<Health>();
        enemy.damageDealer = enemy.GetComponent<DamageDealer>();
        
        newEnemy.transform.position = Random.insideUnitCircle.normalized * enemySpawnDistance;
        enemyController.AddEnemy(newEnemy.GetComponent<Enemy>());
    }
}
