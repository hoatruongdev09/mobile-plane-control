using System;
using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;
public class GameStartedState : GameState, IAirportDelegate, IGamePanelViewDelegate {
    public bool hasEnemy;
    public bool hasFire;
    public bool hasCloud;
    public bool hasTornado;
    public bool hasFuel;
    private PathDrawer pathDrawer;
    private Camera mainCamera;
    private PlaneControl detectedPlane;
    private Airport detectedAirport;
    private GameController gameControl;
    private AirportManager airportManager;
    private List<PlaneControl> collidedPlanes;
    private SpawnController spawnController;
    private ScoreController scoreManager;
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
        scoreManager = gameControl.scoreManager;
        collidedPlanes = new List<PlaneControl> ();
        timer = new FlagTimer ();
        counter = new FlagCounter ();
    }
    public override void Enter (object options) {
        try {
            var flags = options.GetType ().GetProperty ("flags").GetValue (options);
            hasFire = (bool) flags?.GetType ().GetProperty ("hasFire").GetValue (flags);
            hasEnemy = (bool) flags?.GetType ().GetProperty ("hasEnemy").GetValue (flags);
            hasTornado = (bool) flags?.GetType ().GetProperty ("hasTornado").GetValue (flags);
            hasCloud = (bool) flags.GetType ().GetProperty ("hasCloud").GetValue (flags);
            hasFuel = (bool) flags?.GetType ().GetProperty ("hasFuel").GetValue (flags);
        } catch (Exception e) {
            Debug.LogError (e);
        }
        try {
            difficultData = (LevelDifficultData) options.GetType ().GetProperty ("difficult").GetValue (options);
        } catch (Exception e) {
            Debug.LogError (e);
            difficultData = new LevelDifficultData ();
        }
        OnDifficultDataInitialized (difficultData);
        Enter ();
    }
    public override void Enter () {
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
    private void AddCollidedPlane (PlaneControl plane) {
        if (collidedPlanes.Contains (plane)) { return; }
        collidedPlanes.Add (plane);
    }
    private void MouseInput () {
        if (Input.GetMouseButtonDown (0)) {
            DetectPath ();
        }
        if (Input.GetMouseButtonUp (0)) {
            if (detectedPlane && !detectedPlane.IsStun) {
                detectedPlane.Deselect ();
            }
            detectedPlane = null;
        }
        if (Input.GetMouseButton (0) && detectedPlane && !detectedPlane.IsStun) {
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
        Debug.Log ("max point reached");
        detectedPlane.Deselect ();
        detectedPlane = null;
    }
    public void OnPlaneLanded (PlaneControl plane) {
        plane.Delete ();
        scoreManager.AddLandedPlane (1);
        counter.CurrentPlaneCount--;
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
            PlaneControl plane = null;
            try {
                plane = spawnController.CreateAPlaneForAirport (randomAirport, hasFire, hasFuel && GetChance (difficultData.lowFuelChance), difficultData.fuelTimeRangeMin, difficultData.fuelTimeRangeMax);
                plane.onPlaneLanded += this.OnPlaneLanded;
                plane.onCollidedWithPlane += this.OnPlaneCollided;
                timer.currentPlaneSpawnTiming = 0;
                counter.CurrentPlaneCount++;
            } catch (Exception e) {
                Debug.Log (e);
            }
        }
    }
    protected void SpawnTornadoJob () {
        if (!hasTornado) { return; }
        if (counter.CurrentTornadoCount > difficultData.maxTornadoInTime) { return; }
        timer.currentTornadoSpawnTiming += Time.deltaTime;
        if (timer.currentTornadoSpawnTiming >= difficultData.createTornadoInterval) {
            Debug.Log ("Create tornado");
            var tornado = spawnController.CreateTornado (15, MapManager.Instance.GetRandomPosition ());
            tornado.onTornadoDie += OnTornadoDisappear;
            counter.CurrentTornadoCount++;
            timer.currentTornadoSpawnTiming = 0;
        }

    }

    private void OnTornadoDisappear (Tornado tornado) {
        counter.CurrentTornadoCount--;
        tornado.DestroySelf ();
    }

    protected void SpawnFireJob () {
        if (!hasFire) { return; }
        if (counter.CurrentFireCount > difficultData.maxFireInTime) { return; }
        timer.currentFireSpawnTiming += Time.deltaTime;
        if (timer.currentFireSpawnTiming >= difficultData.createFireInterval) {
            Debug.Log ("Create fire");
            var fire = spawnController.CreateFireForest (110, MapManager.Instance.GetRandomPosition ());
            fire.onFireCooledDown += OnFireCoolDowned;
            counter.CurrentFireCount++;
            timer.currentFireSpawnTiming = 0;
        }
    }

    private void OnFireCoolDowned (FireForest fire) {
        counter.CurrentFireCount--;
        fire.DestorySelf ();
    }

    protected void SpawnCloud () {
        if (!hasCloud) { return; }
        if (counter.CurrentCloudCount > difficultData.maxCloudInTime) { return; }
        timer.currentCloudSpawnTiming += Time.deltaTime;
        if (timer.currentCloudSpawnTiming >= difficultData.cloudCreateInterval) {
            Debug.Log ("Create cloud");
            var cloud = spawnController.CreateCloud (MapManager.Instance.GetRandomPosition ());
            cloud.LifeTime = UnityEngine.Random.Range (15, 20);
            cloud.onDisappear += OnCloudDisappear;
            counter.CurrentCloudCount++;
            timer.currentCloudSpawnTiming = 0;
        }
    }

    private void OnCloudDisappear (Cloud cloud) {
        cloud.DestroySelf ();
        counter.CurrentCloudCount--;
    }

    private bool GetChance (float input) {
        return UnityEngine.Random.Range (0, 101) <= input;
        // return true;
    }

}

public class FlagCounter {
    public float CurrentPlaneCount {
        get { return currentPlaneCount; }
        set { currentPlaneCount = Mathf.Clamp (value, 0, Mathf.Infinity); Debug.Log ($"current plane count: {currentPlaneCount}"); }
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