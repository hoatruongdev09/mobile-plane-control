using System;
using UnityEngine;

public class Crashing : PlaneState {
    private PlaneControl plane;
    private Transform transform;
    private bool crashed;
    private float randomRotation;
    public Crashing (PlaneStateManager stateManager) : base (stateManager) {
        plane = stateManager.Controller;
        transform = plane.transform;
    }
    public override void Enter () {
        plane.detector.enabled = false;
        plane.bodyCollider.gameObject.SetActive (false);
        randomRotation = UnityEngine.Random.Range (-15, 15) + transform.rotation.eulerAngles.z;
        StartCrashing ();
    }
    public override void Update () {
        CrashFlying ();
    }
    private void CrashFlying () {
        if (crashed) { return; }
        transform.Translate (Vector2.up * plane.MoveSpeed * Time.smoothDeltaTime);
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 0, randomRotation), 2 * Time.smoothDeltaTime);
        // transform.Rotate (0, 0, plane.DisableRotateSpeed * Time.smoothDeltaTime);
    }
    private void StartCrashing () {
        AnimateCrashing ().setOnComplete (() => {
            crashed = true;
            plane.onCollidedWithPlane?.Invoke (plane);
        }).setEaseInBack ();
    }
    private LTDescr AnimateCrashing () {
        return transform.LeanScale (Vector3.one * 0.3f, 2f);
    }
}