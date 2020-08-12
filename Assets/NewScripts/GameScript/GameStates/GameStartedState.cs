using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private UiManager uIControl;
    private float currentTimeSpeed = 1;
    private bool isChangingToGameOver = false;
    private LevelDifficultData difficultData;
    protected FlagCounter counter;
    protected FlagTimer timer;
    private LevelDataInfo levelInfo;
    private object enterStateInfo;
    private bool isContinue;
    public GameStartedState (GameStateManager stateManager) : base (stateManager) {
        gameControl = stateManager.GameController;
        pathDrawer = gameControl.pathDrawer;
        mainCamera = gameControl.mainCamera;
        spawnController = gameControl.spawnController;
        airportManager = gameControl.airportManager;
        scoreManager = gameControl.scoreManager;
        uIControl = gameControl.uiManager;
        collidedPlanes = new List<PlaneControl> ();
        timer = new FlagTimer ();
        counter = new FlagCounter ();
    }
    public override void Enter (object options) {
        var continueOver = options.GetType ().GetProperty ("continueOver");
        Debug.Log ($"continue over: {continueOver == null}");
        if (continueOver != null) {
            EnterContinue (options);
        } else {
            EnterNewGame (options);
        }
    }
    public override void Enter () {
        gameControl.uiManager.viewGamePanel.Delegate = this;

        scoreManager.onBestScoreChanges += gameControl.uiManager.viewGamePanel.SetHighScore;
        scoreManager.onCurrentPlanesChanges += gameControl.uiManager.viewGamePanel.SetCurrentTextLanded;
        scoreManager.onBestFireExtinguishedChange += gameControl.uiManager.viewGamePanel.SetCurrentFireExtinguished;
        scoreManager.onCurrentFireExtinguishedChanges += gameControl.uiManager.viewGamePanel.SetBestFireExtinguished;

        StartAnimate ();
    }
    public override void Update () {
        MouseInput ();
        SpawnPlaneJob ();
        SpawnTornadoJob ();
        SpawnFireJob ();
        SpawnCloud ();
        TimeJob ();
    }

    public override void Exit () {
        isContinue = false;
        gameControl.uiManager.Delegate = null;

        scoreManager.onBestScoreChanges -= gameControl.uiManager.viewGamePanel.SetHighScore;
        scoreManager.onCurrentPlanesChanges -= gameControl.uiManager.viewGamePanel.SetCurrentTextLanded;
        scoreManager.onBestFireExtinguishedChange -= gameControl.uiManager.viewGamePanel.SetCurrentFireExtinguished;
        scoreManager.onCurrentFireExtinguishedChanges -= gameControl.uiManager.viewGamePanel.SetBestFireExtinguished;

    }
    private void EnterNewGame (object options) {
        enterStateInfo = options;
        LoadLevelInfo (options);
        LoadGameFlags (options);
        LoadDifficultData (options);
        Enter ();
    }
    private void EnterContinue (object options) {
        isContinue = true;
        List<PlaneControl> crashedPlane = (List<PlaneControl>) options.GetType ().GetProperty ("crashedPlanes").GetValue (options);
        ClearCrashedPlane (crashedPlane);
        Enter ();
        BackToNormal (15);
    }
    private void BackToNormal (float time) {
        float startTime = time;
        uIControl.viewGamePanel.textTextCrashAcceptTime.gameObject.SetActive (true);
        LeanTween.value (gameControl.gameObject, startTime, 0, time).setOnUpdate ((float value) => {
            uIControl.viewGamePanel.textTextCrashAcceptTime.text = $"Crash accept time: {Mathf.RoundToInt(value)}";
        }).setOnComplete (() => {
            isContinue = false;
            uIControl.viewGamePanel.textTextCrashAcceptTime.gameObject.SetActive (false);
        });
    }
    private void LoadLevelInfo (object options) {
        try {
            levelInfo = (LevelDataInfo) options.GetType ().GetProperty ("info").GetValue (options);
        } catch (Exception e) {
            Debug.LogError (e);
        }
    }
    private void LoadGameFlags (object options) {
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
    }
    private void LoadDifficultData (object options) {
        try {
            difficultData = (LevelDifficultData) options.GetType ().GetProperty ("difficult").GetValue (options);
        } catch (Exception e) {
            Debug.LogError (e);
            difficultData = new LevelDifficultData ();
        }
        OnDifficultDataInitialized (difficultData);
    }

    protected virtual void StartAnimate () {
        var lastTimeSpeed = Time.timeScale;
        LeanTween.value (gameControl.gameObject, lastTimeSpeed, currentTimeSpeed, .5f).setOnUpdate ((float value) => {
            Time.timeScale = value;
        }).setIgnoreTimeScale (true);
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
    private void ClearCrashedPlane (List<PlaneControl> crashedPlane) {
        foreach (var plane in crashedPlane) {
            plane.BlowUp ();
            spawnController.CreateBlowEffect (plane.transform.position, spawnController.inAirBlowEffectPrefab);
            SoundController.Instance?.PlaneCrash ();
        }
        foreach (var plane in collidedPlanes) {
            plane.BlowUp ();
            spawnController.CreateBlowEffect (plane.transform.position, spawnController.inAirBlowEffectPrefab);
            SoundController.Instance?.PlaneCrash ();
        }
        collidedPlanes.Clear ();
    }
    protected virtual void OnPlaneCollided (PlaneControl plane) {
        SoundController.Instance?.PlaneCrash ();
        Debug.Log ($"is continue: {isContinue}");
        if (isContinue) {
            var fxObject = spawnController.CreateBlowEffect (plane.transform.position, spawnController.inAirBlowEffectPrefab);
            plane.BlowUp ();
            return;
        }
        AddCollidedPlane (plane);
        gameControl.StartCoroutine (DelayToEndGame (() => {
            stateManager.StateMachine.ChangeState (stateManager.OverState, new {
                collidedPlanes = this.collidedPlanes,
                    info = this.levelInfo

            });
            isChangingToGameOver = false;
        }));

    }
    protected void OnPlaneCrashed (PlaneControl plane) {
        SoundController.Instance?.PlaneCrash ();
        if (isContinue) {
            spawnController.CreateBlowEffect (plane.transform.position, spawnController.crashBlowEffectPrefab);
            plane.BlowUp ();
            return;
        }
        AddCollidedPlane (plane);
        gameControl.StartCoroutine (DelayToEndGame (() => {
            stateManager.StateMachine.ChangeState (stateManager.OverState, new {
                collidedPlanes = this.collidedPlanes,
                    info = this.levelInfo
            });
        }));
    }
    protected virtual void OnPlaneSelect (PlaneControl plane, bool action) {
        SoundController.Instance?.PlaySFX (SoundController.Instance?.planeSelect);
    }
    protected virtual void OnPlaneDangerWaning (PlaneControl plane) {
        if (isContinue) {
            return;
        }
        SoundController.Instance?.PlaneWarning ();
        plane.ActiveWarningIndicator (true);
    }
    protected virtual void OnPlaneLanded (PlaneControl plane) {
        SoundController.Instance?.PlaySFX (SoundController.Instance.planeLanded);
        plane.Delete ();
        scoreManager.AddLandedPlane (1);
        counter.CurrentPlaneCount--;
        CheckIfLevelWillUnlockByPlane ();
    }
    private void CheckIfLevelWillUnlockByPlane () {
        Debug.Log ($"{PlayerSection.Instance == null} | {DataManager.Instance == null}");
        if (PlayerSection.Instance == null) { return; }
        if (DataManager.Instance == null) { return; }
        foreach (var levelData in DataManager.Instance.LevelData) {
            Debug.Log ($"check level data id: {levelData.info.id}");
            if (levelData.info.unlockType == (int) LevelDataInfo.UnlockType.landed && levelData.info.unlock <= PlayerSection.Instance.PlayerData.totalPlaneLanded) {
                if (PlayerSection.Instance.PlayerData.unlockedLevel.Contains (levelData.info.id)) {
                    Debug.Log ($"contain level data id: {levelData.info.id}");
                    continue;
                }

                var unlockedLevel = PlayerSection.Instance.PlayerData.unlockedLevel.ToList ();
                unlockedLevel.Add (levelData.info.id);
                PlayerSection.Instance.PlayerData.unlockedLevel = unlockedLevel.ToArray ();
                Debug.Log ($"UNLOCK LEVEL: {levelData.info.id}");
                uIControl.viewGamePanel.ShowAnnouncer ($"Level <b>{levelData.info.name}</b> unlocked");
            }
        }
        PlayerSection.Instance.SaveSection ();
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
    public virtual void OnPauseClick () {
        if (isContinue) {
            return;
        }
        stateManager.StateMachine.ChangeState (stateManager.PauseState);
    }
    private IEnumerator DelayToEndGame (Action callback) {
        if (!isChangingToGameOver) {
            isChangingToGameOver = true;
            yield return new WaitForEndOfFrame ();
            callback ();
        }
    }
    private void OnDifficultDataInitialized (LevelDifficultData difficultData) {
        timer.currentPlaneSpawnTiming = difficultData.planeCreateInterval / 8;
        timer.currentCloudSpawnTiming = difficultData.cloudCreateInterval / 8;
        timer.currentFireSpawnTiming = difficultData.createFireInterval / 8;
        timer.currentTornadoSpawnTiming = difficultData.createTornadoInterval / 8;
    }
    protected void SpawnPlaneJob () {
        if (counter.CurrentPlaneCount > difficultData.maxPlaneInTime || difficultData.maxPlaneInTime == -1) { return; }
        if (counter.CurrentPlaneCount > scoreManager.MostPlaneInScreen) {
            scoreManager.MostPlaneInScreen = (int) counter.CurrentPlaneCount;
        }
        timer.currentPlaneSpawnTiming += Time.deltaTime;
        if (timer.currentPlaneSpawnTiming >= difficultData.planeCreateInterval) {
            try {
                CreatePlane ();
                timer.currentPlaneSpawnTiming = 0;
            } catch (Exception e) {
                Debug.Log (e);
            }
        }
    }
    private void CreatePlane () {
        try {
            var randomAirport = airportManager.RandomAirport ();
            PlaneControl plane = null;
            plane = spawnController.CreateAPlaneForAirport (randomAirport, hasFire && GetChance (difficultData.waterChance), hasFuel && GetChance (difficultData.lowFuelChance), difficultData.fuelTimeRangeMin, difficultData.fuelTimeRangeMax);
            plane.onPlaneLanded += this.OnPlaneLanded;
            plane.onCollidedWithPlane += this.OnPlaneCollided;
            plane.onPlaneCrash += this.OnPlaneCrashed;
            plane.onShowWarning += this.OnPlaneDangerWaning;
            plane.onPlaneSelect += this.OnPlaneSelect;
        } catch (Exception e) {
            throw e;
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
            fire.onFireExtinguished += OnFireExtinguished;
            counter.CurrentFireCount++;
            timer.currentFireSpawnTiming = 0;
        }
    }

    private void OnFireExtinguished (FireForest fire) {
        counter.CurrentFireCount--;
        scoreManager.AddFireExtinguished (1);
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
    /// <summary>
    /// tạo độ khó cho game, 
    /// giảm thời gian các giới hạn của bộ đếm giờ
    /// </summary>
    private void TimeJob () {
        difficultData.planeCreateInterval = Mathf.Clamp (difficultData.planeCreateInterval - 0.01f * Time.deltaTime, 1, Mathf.Infinity);
        difficultData.cloudCreateInterval = Mathf.Clamp (difficultData.cloudCreateInterval - 0.01f * Time.deltaTime, 1, Mathf.Infinity);
        difficultData.createFireInterval = Mathf.Clamp (difficultData.createFireInterval - 0.01f * Time.deltaTime, 1, Mathf.Infinity);
        difficultData.createTornadoInterval = Mathf.Clamp (difficultData.createTornadoInterval - .01f * Time.deltaTime, 1, Mathf.Infinity);
        difficultData.enemyCreateInterval = Mathf.Clamp (difficultData.enemyCreateInterval - 0.01f * Time.deltaTime, 1, Mathf.Infinity);
    }

    private bool GetChance (float input) {
        return UnityEngine.Random.Range (0, 101) <= input;
        // return true;
    }

}

public class FlagCounter {
    public int CurrentPlaneCount {
        get { return currentPlaneCount; }
        set { currentPlaneCount = value; Debug.Log ($"current plane count: {currentPlaneCount}"); }
    }
    public int CurrentCloudCount {
        get { return currentCloudCount; }
        set { currentCloudCount = value; }
    }
    public int CurrentFireCount {
        get { return currentFireCount; }
        set { currentFireCount = value; }
    }
    public int CurrentTornadoCount {
        get { return currentTornadoCount; }
        set { currentCloudCount = value; }
    }

    private int currentPlaneCount;
    private int currentCloudCount;
    private int currentFireCount;
    private int currentTornadoCount;
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