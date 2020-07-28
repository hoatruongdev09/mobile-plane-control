using System;
using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;
public class GameStartedState : GameState, IAirportDelegate, IGamePanelViewDelegate {
    public bool HasEnemy { get; private set; }
    public bool HasFire { get; private set; }
    public bool HasCloud { get; private set; }
    public bool HasTornado { get; private set; }
    public bool HasFuel { get; private set; }
    private PathDrawer pathDrawer;
    private Camera mainCamera;
    private PlaneControl detectedPlane;
    private Airport detectedAirport;
    private GameController gameControl;
    private AirportManager airportManager;
    private List<PlaneControl> collidedPlanes;
    private SpawnController spawnController;
    private float currentTimeSpeed = 1;
    private bool isChangingToGameOver = false;
    private LevelDifficultData difficultData;
    protected FlagCounter counter;
    protected FlagTimer timer;

    public GameStartedState (GameStateManager stateManager) : base (stateManager) {
        gameControl = stateManager.GameController;
        pathDrawer = gameControl.pathDrawer;
        mainCamera = gameControl.mainCamera;
        spawnController = gameControl.spawnController;
        airportManager = gameControl.airportManager;
        collidedPlanes = new List<PlaneControl> ();
        timer = new FlagTimer ();
        counter = new FlagCounter ();
    }
    public override void Enter (object options) {
        var flags = options.GetType ().GetProperty ("flags").GetValue (options);
        HasFire = (bool) flags?.GetType ().GetProperty ("hasFire").GetValue (flags);
        HasEnemy = (bool) flags?.GetType ().GetProperty ("hasEnemy").GetValue (flags);
        HasTornado = (bool) flags?.GetType ().GetProperty ("hasTornado").GetValue (flags);
        HasCloud = (bool) flags.GetType ().GetProperty ("hasCloud").GetValue (flags);
        HasFuel = (bool) flags?.GetType ().GetProperty ("hasFuel").GetValue (flags);
        difficultData = (LevelDifficultData) options.GetType ().GetProperty ("difficult").GetValue (options);
        OnDifficultDataInitialized (difficultData);
        Enter ();
    }
    public override void Enter () {
        gameControl.onPlaneCollided += OnPlaneCollided;
        gameControl.onPlaneLanded += OnPlaneLanded;
        gameControl.uiManager.viewGamePanel.Delegate = this;
        StartAnimate ();
    }
    public override void Update () {
        MouseInput ();
        SpawnPlaneJob ();
        SpawnTornadoJob ();
        SpawnFireJob ();
        SpawnCloud ();
    }
    public override void Exit () {
        gameControl.uiManager.Delegate = null;
        gameControl.onPlaneCollided -= OnPlaneCollided;
    }
    protected virtual void StartAnimate () {
        var lastTimeSpeed = Time.timeScale;
        LeanTween.value (gameControl.gameObject, lastTimeSpeed, currentTimeSpeed, .5f).setOnUpdate ((float value) => {
            Time.timeScale = value;
        }).setIgnoreTimeScale (true);
    }
    protected virtual void OnPlaneCollided (PlaneControl plane) {
        AddCollidedPlane (plane);
        gameControl.StartCoroutine (DelayToEndGame (0.1f, () => {
            stateManager.StateMachine.ChangeState (stateManager.OverState, new { collidedPlanes = this.collidedPlanes });
        }));

    }
    private void OnPlaneLanded () {
        counter.CurrentPlaneCount--;
    }
    private void AddCollidedPlane (PlaneControl plane) {
        if (collidedPlanes.Contains (plane)) { return; }
        collidedPlanes.Add (plane);
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
            DrawPath (detectedPlane?.Path, mainCamera.ScreenToWorldPoint (Input.mousePosition), OnMaxPointReach);
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
    private void DrawPath (NewScript.Path path, Vector3 position, Action maxPointReached = null) {
        if (path == null) { return; }
        pathDrawer.DrawPath (path, position, maxPointReached);
    }
    private void OnMaxPointReach () {
        detectedPlane.Deselect ();
        detectedPlane = null;
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
        // Debug.Log ($"current: {Time.timeScale} next speed: {nextTimeSpeed}");
        if (nextTimeSpeed > gameControl.MaxTimeSpeed) {
            nextTimeSpeed = 1;
        }
        AnimateFastForward (nextTimeSpeed);
    }
    private void AnimateFastForward (float nextSpeed) {
        float startSpeed = Time.timeScale;
        LeanTween.value (gameControl.gameObject, startSpeed, nextSpeed, .2f).setOnUpdate ((float value) => {
            Time.timeScale = value;
        }).setOnComplete (() => {
            currentTimeSpeed = Time.timeScale;
        }).setIgnoreTimeScale (true);
    }
    public void OnPauseClick () {
        stateManager.StateMachine.ChangeState (stateManager.PauseState);
    }
    private IEnumerator DelayToEndGame (float time, Action callback) {
        if (!isChangingToGameOver) {
            isChangingToGameOver = true;
            yield return new WaitForSecondsRealtime (time);
            callback ();
        }
    }
    private void OnDifficultDataInitialized (LevelDifficultData difficultData) {
        timer.currentPlaneSpawnTiming = difficultData.planeCreateInterval / 2;
        timer.currentCloudSpawnTiming = difficultData.cloudCreateInterval / 2;
        timer.currentFireSpawnTiming = difficultData.createFireInterval / 2;
        timer.currentTornadoSpawnTiming = difficultData.createTornadoInterval / 2;
    }
    protected void SpawnPlaneJob () {
        if (counter.CurrentPlaneCount > difficultData.maxPlaneInTime) { return; }
        timer.currentPlaneSpawnTiming += Time.deltaTime;
        if (timer.currentPlaneSpawnTiming >= difficultData.planeCreateInterval) {
            var randomAirport = airportManager.RandomAirport ();
            try {
                spawnController.CreateAPlaneForAirport (randomAirport, HasFire, HasFuel && GetChance (difficultData.lowFuelChance), difficultData.fuelTimeRangeMin, difficultData.fuelTimeRangeMax);
                counter.CurrentPlaneCount++;
                timer.currentPlaneSpawnTiming = 0;
            } catch (Exception e) {
                // Debug.Log (e);
            }
        }
    }
    protected void SpawnTornadoJob () {
        if (!HasTornado) { return; }
        if (counter.CurrentTornadoCount > difficultData.maxTornadoInTime) { return; }
        timer.currentTornadoSpawnTiming += Time.deltaTime;
        if (timer.currentTornadoSpawnTiming >= difficultData.createTornadoInterval) {
            Debug.Log ("Create tornado");
            counter.CurrentTornadoCount++;
            timer.currentTornadoSpawnTiming = 0;
        }

    }
    protected void SpawnFireJob () {
        if (!HasFire) { return; }
        if (counter.CurrentFireCount > difficultData.maxFireInTime) { return; }
        timer.currentFireSpawnTiming += Time.deltaTime;
        if (timer.currentFireSpawnTiming >= difficultData.createFireInterval) {
            Debug.Log ("Create fire");
            counter.CurrentFireCount++;
            timer.currentFireSpawnTiming = 0;
        }
    }
    protected void SpawnCloud () {
        if (!HasCloud) { return; }
        if (counter.CurrentCloudCount > difficultData.maxCloudInTime) { return; }
        timer.currentCloudSpawnTiming += Time.deltaTime;
        if (timer.currentCloudSpawnTiming >= difficultData.cloudCreateInterval) {
            Debug.Log ("Create cloud");
            counter.CurrentCloudCount++;
            timer.currentCloudSpawnTiming = 0;
        }
    }
    private bool GetChance (float input) {
        return UnityEngine.Random.Range (0, 101) <= input;
    }

}

public class FlagCounter {
    public float CurrentPlaneCount {
        get { return currentPlaneCount; }
        set { currentPlaneCount = Mathf.Clamp (value, 0, Mathf.Infinity); }
    }
    public float CurrentCloudCount {
        get { return currentCloudCount; }
        set { currentCloudCount = Mathf.Clamp (value, 0, Mathf.Infinity); }
    }
    public float CurrentFireCount {
        get { return currentFireCount; }
        set { currentFireCount = Mathf.Clamp (value, 0, Mathf.Infinity); }
    }
    public float CurrentTornadoCount {
        get { return currentTornadoCount; }
        set { currentCloudCount = Mathf.Clamp (value, 0, Mathf.Infinity); }
    }

    private float currentPlaneCount;
    private float currentCloudCount;
    private float currentFireCount;
    private float currentTornadoCount;
    public FlagCounter () {
        Reset ();
    }
    public void Reset () {
        CurrentPlaneCount = 0;
        CurrentCloudCount = 0;
        CurrentTornadoCount = 0;
        CurrentFireCount = 0;
    }

}
public class FlagTimer {
    public float currentPlaneSpawnTiming;
    public float currentCloudSpawnTiming;
    public float currentEnemySpawnTiming;
    public float currentTornadoSpawnTiming;
    public float currentFireSpawnTiming;
    public FlagTimer () {
        Reset ();
    }
    public void Reset () {
        currentPlaneSpawnTiming = 0;
        currentCloudSpawnTiming = 0;
        currentEnemySpawnTiming = 0;
        currentTornadoSpawnTiming = 0;
        currentFireSpawnTiming = 0;
    }
}