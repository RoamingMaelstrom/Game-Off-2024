using System.Collections.Generic;
using UnityEngine;

public class SatelliteDetector : MonoBehaviour
{
    [SerializeField] Weapon parentWeapon;
    public float range = 25f;
    [SerializeField] List<Rigidbody2D> inRange = new();
    [SerializeField] bool targetSlowest = false;

    private Rigidbody2D target;

    private void FixedUpdate() {
        transform.localPosition = parentWeapon.transform.localPosition;
        transform.localScale = Vector3.one * range;
        if (inRange.Count == 0) {
            target = null;
            parentWeapon.SetTarget(null);
            return;
        }

        for (int i = inRange.Count - 1; i >= 0; i--)
        {
            if (!inRange[i].gameObject.activeInHierarchy) inRange.RemoveAt(i);
        }

        if (inRange.Count == 0) {
            target = null;
            parentWeapon.SetTarget(null);
            return;
        }

        target = targetSlowest ? GetSlowest() : GetNearest();
        parentWeapon.SetTarget(target);

        if (target != null) parentWeapon.TryFire();
    }

    private Rigidbody2D GetSlowest()
    {
        Rigidbody2D slowest = inRange[0];
        float slowestSpeedSqr = inRange[^1].velocity.sqrMagnitude;
        float tSlowestSqr;

        for (int i = inRange.Count - 2; i >= 0; i--)
        {
            tSlowestSqr = inRange[i].velocity.sqrMagnitude;
            if (tSlowestSqr < slowestSpeedSqr) {
                slowest = inRange[i];
                slowestSpeedSqr = tSlowestSqr;
            }
        }

        return slowest;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        inRange.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (inRange.Contains(other.attachedRigidbody)) inRange.Remove(other.attachedRigidbody);
    }

    private Rigidbody2D GetNearest() {
        float sDeltaSqr = (inRange[0].transform.position - transform.position).sqrMagnitude;
        float tDeltaSqr;
        Rigidbody2D sNearest = inRange[0];

        for (int i = 1; i < inRange.Count; i++)
        {
            tDeltaSqr = (inRange[i].transform.position - transform.position).sqrMagnitude;
            if (tDeltaSqr < sDeltaSqr) {
                sNearest = inRange[i];
                sDeltaSqr = tDeltaSqr;
            }
        }

        return sNearest;
    }
}
