using SOEvents;
using UnityEngine;

public class EarthHealth : MonoBehaviour
{
    [SerializeField] GameObjectFloatSOEvent enemyReachedEarthEvent;
    [SerializeField] EarthSimulation earthSimulation;

    private void OnTriggerEnter2D(Collider2D other) {
        enemyReachedEarthEvent.Invoke(other.gameObject, 0);
        earthSimulation.TakeDamage(other.GetComponent<DamageDealer>().damageValue);
    }
    
}
