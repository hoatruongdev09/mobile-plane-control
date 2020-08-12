using System;
using UnityEngine;

public class Crashing : PlaneState {
    private PlaneControl controller;
    private Transform transform;
    private bool crashed;
    private float randomRotation;

    private float countingRandom = 0;
    private float crashingSpeed = 0;
    public Crashing (PlaneStateManager stateManager) : base (stateManager) {
        controller = stateManager.Controller;
        transform = controller.transform;
    }
    public override void Enter () {
        controller.detector.enabled = false;
        controller.bodyCollider.gameObject.SetActive (false);
        controller.path.DeactivateEndPoint (true);
        controller.Path.Clear ();
        randomRotation = UnityEngine.Random.Range (-60, 60) + transform.rotation.eulerAngles.z;
        StartCrashing ();
    }
    public override void Update () {
        CrashFlying ();
    }
    private void CrashFlying () {
        if (crashed) { return; }
        if (countingRandom >= 1f) {
            randomRotation = UnityEngine.Random.Range (-60, 60) + transform.rotation.eulerAngles.z;
            countingRandom = 0;
        } else {
            countingRandom += Time.deltaTime;
        }
        transform.Translate (Vector2.up * (controller.MoveSpeed + crashingSpeed) * Time.smoothDeltaTime);
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 0, randomRotation), 10 * Time.smoothDeltaTime);
    }
    private void StartCrashing () {
        AnimateCrashing ().setOnComplete (() => {
            crashed = true;
            controller.onPlaneCrash?.Invoke (controller);
        }).setEaseInBack ().setOnUpdate ((Vector3 value) => {
            crashingSpeed += Time.deltaTime;
        });
    }
    private LTDescr AnimateCrashing () {
        return transform.LeanScale (Vector3.one * 0.3f, 3f);
    }
}