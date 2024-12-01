using SOEvents;
using UnityEngine;

public class PrototypeLaser : MonoBehaviour
{
    [SerializeField] TechObjectDisplaySOEvent unlockTechEvent;

    [SerializeField] DamageDealer damageDealer;
    [SerializeField] LineRenderer lineRenderer;
    public float rotationSpeed = 2f;
    private float currentRotation;

    public int numUpgrades = 0;
    public int maxUpgrades = 5;

    [SerializeField] bool active = false;

    [SerializeField] string laserSfx;
    [SerializeField] LayerMask layerMask;

    int playTimer = 0;

    private readonly TechUpgradeHandler techUpgradeHandler = TechUpgradeHandler.___VICTORY___;

    private void Awake() {
        unlockTechEvent.AddListener(ProcessUnlock);
    }

    private void FixedUpdate() {
        if (!active) return;
        playTimer ++;
        if (playTimer % 250 == 0) GliderAudio.SFX.PlayAtPoint(laserSfx, Vector2.zero);
    }

    private void Update() {
        if (!active) {
            lineRenderer.positionCount = 0;
            return;
        }
        currentRotation -= rotationSpeed * Time.deltaTime / 60f;
        Vector2 target = new Vector2(Mathf.Sin(-currentRotation * Mathf.Deg2Rad), Mathf.Cos(currentRotation * Mathf.Deg2Rad)) * 1000f;

        RaycastHit2D result = Physics2D.Raycast(Vector2.zero, target.normalized, 1000f, layerMask);

        Vector2 endPoint = result.point == Vector2.zero ? target : result.point + (target.normalized / 2f);

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, endPoint);
        damageDealer.transform.position = endPoint;
    }



    private void ProcessUnlock(TechObjectDisplay tOD) {
        if (!tOD.techObject.ContainsHandler(techUpgradeHandler)) return;

        foreach (var effect in tOD.techObject.effects) {
            if (effect.effectType == EffectType.PROTOTYPE_WEAPON) {
                if (!active) {
                    active = true;
                    numUpgrades++;
                }
                else UpgradePrototype();
                return;
            }
        }
    }

    public void UpgradePrototype() {
        numUpgrades++;
        damageDealer.dotDamageValue *= 2f;
        damageDealer.dotInterval *= 0.9f;
        rotationSpeed *= 1.25f;
        if (numUpgrades > 5) rotationSpeed *= 1.25f;
    }
}
