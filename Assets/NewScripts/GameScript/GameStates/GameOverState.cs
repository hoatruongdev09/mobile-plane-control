using System;
using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;

public class GameOverState : GameState {
    private UiManager uIManager;
    private List<PlaneControl> collidedPlanes;
    private GameController controller;
    public GameOverState (GameStateManager stateManager) : base (stateManager) {
        uIManager = stateManager.GameController.uiManager;
        controller = stateManager.GameController;
    }
    public override void Enter (object options) {
        collidedPlanes = (List<PlaneControl>) options.GetType ().GetProperty ("collidedPlanes").GetValue (options, null);
        Enter ();
    }
    public override void Enter () {
        foreach (var plane in collidedPlanes) {
            plane.HighlightCrash ();
        }
        EndGameEffect ();
        controller.StartCoroutine (DelayToShowGameOverPanel (3, () => {
            uIManager.ChangePanel (uIManager.viewGameOverPanel);
        }));
    }
    public override void Exit () {

    }
    private void EndGameEffect () {
        var currentTimeSpeed = Time.timeScale;
        LeanTween.value (controller.gameObject, currentTimeSpeed, 0, .2f).setOnUpdate ((float value) => {
            Time.timeScale = Mathf.Clamp (value, 0, Mathf.Infinity);
        }).setEaseInSine ().setIgnoreTimeScale (true);
    }
    private IEnumerator DelayToShowGameOverPanel (float time, Action callback) {
        yield return new WaitForSecondsRealtime (time);
        callback ();
    }
}