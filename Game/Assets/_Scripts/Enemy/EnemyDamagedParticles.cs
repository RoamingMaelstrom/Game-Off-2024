using SOEvents;
using UnityEngine;

public class EnemyDamagedParticles : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent enemyDamagedEvent;
    [SerializeField] GameObject particleSystemPrefab;
    [SerializeField] int particleSystemCount = 12;
    [SerializeField] float minEmissionTime = 0.04f;
    [SerializeField] float maxEmissionTime = 0.1f;

    private ParticleSystem[] particleSystems;
    private float[] systemTimers;
    private int currentParticleSystemIndex;

    private void Awake() {
        enemyDamagedEvent.AddListener(AssignParticleSystem);
    }

    private void AssignParticleSystem(GameObject enemy, float arg1)
    {
        ParticleSystem particles = particleSystems[currentParticleSystemIndex];
        particles.gameObject.transform.position = enemy.transform.position;
        particles.Play();
        systemTimers[currentParticleSystemIndex] = Random.Range(minEmissionTime, maxEmissionTime);
        currentParticleSystemIndex ++;
        currentParticleSystemIndex %= particleSystemCount;
    }

    private void Start() {
        particleSystems = new ParticleSystem[particleSystemCount];
        systemTimers = new float[particleSystemCount];
        for (int i = 0; i < particleSystemCount; i++)
        {
            GameObject systemObject = Instantiate(particleSystemPrefab, Vector2.one * 10000, Quaternion.identity, transform);
            particleSystems[i] = systemObject.GetComponent<ParticleSystem>();
        }
    }

    private void FixedUpdate() {
        for (int i = 0; i < particleSystemCount; i++)
        {
            if (systemTimers[i] <= 0) particleSystems[i].Stop();
            systemTimers[i] -= Time.fixedDeltaTime;
        }
    }
}
