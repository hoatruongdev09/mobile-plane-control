using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;
public class GameStartedState : GameState, IAirportDelegate, IGamePanelViewDelegate {
    private PathDrawer pathDrawer;
    private Camera mainCamera;
    private PlaneControl detectedPlane;
    private Airport detectedAirport;
    private GameController gameControl;
    public GameStartedState (GameStateManager stateManager) : base (stateManager) {
        pathDrawer = stateManager.GameController.pathDrawer;
        mainCamera = stateManager.GameController.mainCamera;
        gameControl = stateManager.GameController;
    }
    public override void Enter () {
        Time.timeScale = 1;
        stateManager.GameController.uiManager.viewGamePanel.Delegate = this;
    }
    public override void Update () {
        MouseInput ();
    }
    public override void Exit () {
        stateManager.GameController.uiManager.Delegate = null;
    }
    private void MouseInput () {
        if (Input.GetMouseButtonDown (0)) {
            DetectPath ();
        }
        if (Input.GetMouseButtonUp (0)) {
            if (detectedPlane) {
                detectedPlane.Deselect ();
            }
            detectedPlane = null;
        }
        if (Input.GetMouseButton (0)) {
            DrawPath (detectedPlane?.Path, mainCamera.ScreenToWorldPoint (Input.mousePosition));
            DetectAirport ();
        }
    }
    private void DetectPath () {
        var detectedPlane = pathDrawer.DetectPlane (mainCamera.ScreenToWorldPoint (Input.mousePosition));
        var detectedEndPoint = pathDrawer.DetectEndPoint (mainCamera.ScreenToWorldPoint (Input.mousePosition));
        if (detectedPlane) {
            this.detectedPlane = detectedPlane;
            this.detectedPlane.Select ();
            this.detectedPlane.Path.Clear ();
        } else if (detectedEndPoint) {
            this.detectedPlane = detectedEndPoint.Path.Controller;
            this.detectedPlane.Select ();
        }

    }
    private void DetectAirport () {
        if (detectedPlane == null) { return; }
        var airport = pathDrawer.DetectAirport (mainCamera.ScreenToWorldPoint (Input.mousePosition));
        if (airport == null) {
            detectedAirport?.ClearPoints ();
            detectedAirport = null;
            return;
        }
        detectedAirport = airport;
        detectedAirport.Delegate = this;
        airport.Record (mainCamera.ScreenToWorldPoint (Input.mousePosition), detectedPlane.Path);
    }
    private void DrawPath (NewScript.Path path, Vector3 position) {
        if (path == null) { return; }
        pathDrawer.DrawPath (path, position);
    }

    public void OnAddLandingPlane () {
        detectedPlane.SetLanding (detectedAirport);
        detectedPlane.Deselect ();
        detectedPlane = null;
        detectedAirport.ClearPoints ();
        detectedAirport = null;
    }

    public void OnFastForward () {
        float nextTimeSpeed = Mathf.FloorToInt (Time.timeScale + 1);
        Debug.Log ($"current: {Time.timeScale} next speed: {nextTimeSpeed}");
        if (nextTimeSpeed > gameControl.MaxTimeSpeed) {
            nextTimeSpeed = 1;
        }
        AnimateFastForward (nextTimeSpeed);
    }
    private void AnimateFastForward (float nextSpeed) {
        float startSpeed = Time.timeScale;
        LeanTween.value (gameControl.gameObject, startSpeed, nextSpeed, 1f).setOnUpdate ((float value) => {
            Time.timeScale = value;
        }).setOnComplete (() => {
            Debug.Log ($"current speed: {Time.timeScale}");
        });
    }
    public void OnPauseClick () {
        Debug.Log ("pause");
        stateManager.StateMachine.ChangeState (stateManager.PauseState);
    }

}