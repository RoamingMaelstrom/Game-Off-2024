using UnityEngine;

[CreateAssetMenu(fileName = "WorldInfo", menuName = "Jam/WorldInfo", order = 1)]
public class WorldInfo : ScriptableObject {
    [SerializeField] float baseWorldRadius = 150f;
    [HideInInspector]
    public float gameWorldRadius;
    public float earthRadius = 30f;

    public void CallOnMainSceneLoad() => gameWorldRadius = baseWorldRadius;
}