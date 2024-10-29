using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolInfoGroupObject", menuName = "GliderFramework/PoolInfoGroupObject", order = 11)]
public class PoolInfoGroup : ScriptableObject
{
    public int startPoolID;
    public string basePoolName;
    public List<GameObject> objectPrefabList;
    public int maxPoolSize;
    public PoolDynamicOptions dynamicOptions = new();
}
