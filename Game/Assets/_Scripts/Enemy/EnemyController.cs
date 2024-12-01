using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] Rigidbody2D probeBody;
    [SerializeField] List<Enemy> enemies = new();
    public bool trackProbe = false;

    private void FixedUpdate() {
        if (!trackProbe) return;
        foreach (var enemy in enemies)
        {
            if ((enemy.body.transform.position - probeBody.transform.position).sqrMagnitude < enemy.body.transform.position.sqrMagnitude) {
                enemy.body.velocity = Vector2.Lerp(enemy.body.velocity, CalculateVelocityToProbe(enemy, probeBody), Time.fixedDeltaTime * 0.5f);
            }
            else enemy.body.velocity = Vector2.Lerp(enemy.body.velocity, CalculateVelocityToEarth(enemy), Time.fixedDeltaTime * 0.5f);

            enemy.body.transform.rotation = Quaternion.Euler(0, 0, WeaponMath.Math.VectorToRotation(-enemy.body.velocity));
        }
    }

    public void AddEnemy(Enemy enemy) {
        enemies.Add(enemy);
        enemy.body.velocity = CalculateVelocityToEarth(enemy);
        enemy.body.transform.rotation = Quaternion.Euler(0, 0, WeaponMath.Math.VectorToRotation(-enemy.body.velocity));
    }

    private Vector2 CalculateVelocityToEarth(Enemy enemy) {
        Vector2 direction = - enemy.transform.position.normalized;
        return direction * enemy.speed;
    }

    private Vector2 CalculateVelocityToProbe(Enemy enemy, Rigidbody2D probe) {
        return (probe.transform.position - (enemy.transform.position + (Vector3)enemy.body.velocity)).normalized * enemy.speed;
    }

    public Vector2 GetRandomEnemyPosition() {
        if (enemies.Count == 0) return Random.insideUnitCircle.normalized * 1000;
        return enemies[Random.Range(0, enemies.Count)].transform.position;
    }

    public void SetVelocitiesToEarth() {
        foreach (var enemy in enemies)
        {
            enemy.body.velocity = CalculateVelocityToEarth(enemy);
            enemy.body.transform.rotation = Quaternion.Euler(0, 0, WeaponMath.Math.VectorToRotation(-enemy.body.velocity));
        }
    }
}
