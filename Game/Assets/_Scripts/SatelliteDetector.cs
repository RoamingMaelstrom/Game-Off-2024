using System.Collections.Generic;
using UnityEngine;

public class SatelliteDetector : MonoBehaviour
{
    [SerializeField] Weapon parentWeapon;
    public float range = 25f;
    [SerializeField] List<Rigidbody2D> inRange = new();

    private Rigidbody2D nearest;

    private void FixedUpdate() {
        transform.localPosition = parentWeapon.transform.localPosition;
        transform.localScale = Vector3.one * range;
        if (inRange.Count == 0) {
            nearest = null;
            parentWeapon.SetTarget(null);
            return;
        }

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

        nearest = sNearest;
        parentWeapon.SetTarget(nearest);

        if (nearest != null) parentWeapon.TryFire();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        inRange.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (inRange.Contains(other.attachedRigidbody)) inRange.Remove(other.attachedRigidbody);
    }
}
