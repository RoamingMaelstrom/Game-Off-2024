using SOEvents;
using UnityEngine;

public class CameraMapUpgrader : MonoBehaviour
{
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;
    [SerializeField] CameraPositionLogic cameraPositionLogic;
    [SerializeField] Camera minimapCamera;
    [SerializeField] LayerMask uniqueMinimapLayerMask;
    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.CAMERA_LOGIC;

    private void Awake() {
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects)
        {
            switch (effect.effectType)
            {
                case EffectType.MAP_SIZE: cameraPositionLogic.arenaSize *= 1f + (effect.value / 100f); break;
                case EffectType.MINIMAP_ZOOM: minimapCamera.orthographicSize *= 1f + (effect.value / 100f); break;
                case EffectType.UNLOCK_ICONS: minimapCamera.cullingMask = uniqueMinimapLayerMask; break;
                default: break;
            }
        }
    }
}
