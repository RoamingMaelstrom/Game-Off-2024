using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    [SerializeField] Rigidbody2D target;
    [SerializeField] float speedGainPerSecond = 1f;
    [SerializeField] float homeDelay = 1f;
    [SerializeField] float homeStrength = 1f;

    private List<Rigidbody2D> inRange = new();

    private float timer = 0;


    private void OnEnable() {
        timer = 0;
        inRange = new();
    }

    private void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        if (timer > homeDelay) body.velocity = body.velocity.normalized * (body.velocity.magnitude + (speedGainPerSecond * Time.fixedDeltaTime));

        if (inRange.Count == 0) {
            target = null;
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

        target = sNearest;



        if (target != null && timer > homeDelay) {
            Vector2 cDir = body.velocity.normalized;
            Vector2 tDir = (target.transform.position - transform.position).normalized;
            body.velocity = Vector2.Lerp(cDir, tDir, homeStrength * Time.fixedDeltaTime) * body.velocity.magnitude;
            body.transform.rotation = Quaternion.Euler(0, 0, WeaponMath.Math.VectorToRotation(body.velocity));
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        inRange.Add(other.attachedRigidbody);
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (inRange.Contains(other.attachedRigidbody)) inRange.Remove(other.attachedRigidbody);
    }


    // if target goes out of range, or is behind direction of travel, choose new target
    // Maybe vary homeStrength depending on distance from target
}
