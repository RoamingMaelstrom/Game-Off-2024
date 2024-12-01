using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int enemyID;
    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;
    public Health health;
    public DamageDealer damageDealer;
    public Weapon weapon;
    public float speed;
}
