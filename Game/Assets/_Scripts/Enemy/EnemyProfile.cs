using UnityEngine;

[CreateAssetMenu(fileName = "EnemyProfile", menuName = "Jam/EnemyProfile", order = 2)]
public class EnemyProfile : ScriptableObject {
    public int enemyID;
    public string enemyName;
    public Sprite sprite;
    public float health = 100f;
    public float damage = 10f; 
    public float speed = 10f;
    public Vector2 scale = Vector2.one;

    public bool hasWeapon = false;
    public float fireDistance = 10f;
    public float fireRate = 0.5f;
    public int munitionID;
}

