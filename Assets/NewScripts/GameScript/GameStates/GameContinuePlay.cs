using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NewScript;
using UnityEngine;

public class GameContinuePlay : GameStartedState {
    private float effectTime = 0;
    private List<PlaneControl> crashPlanes;
    public GameContinuePlay (GameStateManager stateManager) : base (stateManager) {
        crashPlanes = new List<PlaneControl> ();
    }

    public override void Enter (object options) {
        crashPlanes = (List<PlaneControl>) options.GetType ().GetProperty ("crashPlanes").GetValue (options);
    }
    public override void Enter () {
        ClearCrashedPlanes (crashPlanes);
        base.Enter ();

        effectTime = 15;
        stateManager.GameController.StartCoroutine (DelayChangeToNormal (15));
    }
    public override void Update () {
        base.Update ();
        effectTime -= Time.unscaledDeltaTime;
    }
    public override void Exit () {
        base.Exit ();
    }
    protected override void OnPlaneCollided (PlaneControl plane) {
        plane.BlowUp ();
    }
    protected override void OnPlaneDangerWaning (PlaneControl plane) {

    }
    private IEnumerator DelayChangeToNormal (float time) {
        yield return new WaitForSecondsRealtime (time);
        stateManager.StateMachine.ChangeState (stateManager.StartedState);
    }
    public override void OnPauseClick () {

    }
    private void ClearCrashedPlanes (List<PlaneControl> crashPlane) {
        foreach (var plane in crashPlane) {
            plane.BlowUp ();
        }
    }
}