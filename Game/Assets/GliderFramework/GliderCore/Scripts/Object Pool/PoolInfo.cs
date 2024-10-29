using UnityEngine;

[CreateAssetMenu(fileName = "PoolInfoObject", menuName = "GliderFramework/PoolInfoObject", order = 10)]
public class PoolInfo : ScriptableObject
{
    public int poolID;
    public string poolName;
    public GameObject objectPrefab;
    public int maxPoolSize;
    public PoolDynamicOptions dynamicOptions = new();
}
