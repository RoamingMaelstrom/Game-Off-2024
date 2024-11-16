using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] List<Enemy> enemies = new();

    public void AddEnemy(Enemy enemy) {
        enemies.Add(enemy);
        enemy.body.velocity = CalculateVelocity(enemy);
    }

    private Vector2 CalculateVelocity(Enemy enemy) {
        Vector2 direction = - enemy.transform.position.normalized;
        return direction * 5f;
    }

    public Vector2 GetRandomEnemyPosition() {
        if (enemies.Count == 0) return Random.insideUnitCircle.normalized * 1000;
        return enemies[Random.Range(0, enemies.Count)].transform.position;
    }
}
