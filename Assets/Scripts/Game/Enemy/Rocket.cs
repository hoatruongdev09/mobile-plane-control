using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Bullet {
    public float rotateSpeed;

    public bool targetTracking = false;

    protected override void Move () {
        transform.Translate (Vector2.up * moveSpeed * Time.deltaTime);
        liveTime -= Time.deltaTime;
        if (liveTime <= 0) {
            Destroy (gameObject);
        }
        if (target != null && targetTracking)
            transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 0, CalculationAngle ((target.transform.position - transform.position).normalized)), rotateSpeed * Time.deltaTime);
    }
    private float CalculationAngle (Vector3 direction) {
        return -Mathf.Atan2 (direction.x, direction.y) * Mathf.Rad2Deg;
    }
    protected override void CollidedAction () {
        SpawnManager.Instance.SpawnRandomInAirBlowFX (transform.position);
    }
}