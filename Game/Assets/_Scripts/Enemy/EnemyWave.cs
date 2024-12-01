using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWave", menuName = "Jam/EnemyWave", order = 4)]
public class EnemyWave : ScriptableObject {
    public EnemySpawnInfo[] spawnInfoArray;
    public float duration = 60f;
}




[System.Serializable]
public class EnemySpawnInfo
{
    public EnemyProfile profile;
    public float startSpawnRate = 1f;
    public float endSpawnRate = 1f;
}