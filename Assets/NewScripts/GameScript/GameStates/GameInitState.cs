using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GameInitState : GameState {
    private UiManager uiManager;
    private GameController controller;
    private SpawnController spawnController;
    private MapGraphicController mapGraphicController;
    private AirportManager airportManager;
    private bool levelLoaded = false;

    private bool hasEnemy;
    private bool hasFire;
    private bool hasCloud;
    private bool hasTornado;
    private bool hasFuel;
    private LevelDifficultData levelDifficultData;
    private LevelDataInfo levelInfo;
    private ScoreController scoreController;
    public GameInitState (GameStateManager stateManager) : base (stateManager) {
        uiManager = stateManager.GameController.uiManager;
        controller = stateManager.GameController;
        spawnController = stateManager.GameController.spawnController;
        mapGraphicController = stateManager.GameController.mapGraphicController;
        airportManager = stateManager.GameController.airportManager;
        scoreController = controller.scoreManager;
    }
    public override void Enter (object options) {
        string levelData = (string) options.GetType ().GetProperty ("level").GetValue (options);
        string difficult = (string) options.GetType ().GetProperty ("difficult").GetValue (options);

        scoreController.onBestFireExtinguishedChange += uiManager.viewGamePanel.SetBestFireExtinguished;
        scoreController.onBestScoreChanges += uiManager.viewGamePanel.SetHighScore;

        LoadLevelData (levelData);
        LoadDifficultData (difficult);
        Enter ();
        InitSound ();
    }

    public override void Enter () {
        controller.StartCoroutine (DelayStartGame (() => {
            stateManager.StateMachine.ChangeState (stateManager.StartedState, new {
                flags = new {
                        hasEnemy = hasEnemy,
                            hasFire = hasFire,
                            hasCloud = hasCloud,
                            hasTornado = hasTornado,
                            hasFuel = hasFuel,
                    },
                    difficult = levelDifficultData,
                    info = levelInfo
            });
        }));
    }

    public override void Exit () {
        scoreController.onBestFireExtinguishedChange -= uiManager.viewGamePanel.SetBestFireExtinguished;
        scoreController.onBestScoreChanges -= uiManager.viewGamePanel.SetHighScore;
        uiManager.HideFader ();
    }
    private void LoadDifficultData (string difficultData) {
        string filePath = $"LevelDifficult/{difficultData}";
        TextAsset textFile = Resources.Load<TextAsset> (filePath);
        Debug.Log ($"difficult data: {textFile.text}");
        this.levelDifficultData = JsonUtility.FromJson<LevelDifficultData> (textFile.text);
    }
    private void LoadLevelData (string levelData) {
        string filePath = $"LevelData/{levelData}";
        Debug.Log ($"file path: {filePath}");
        TextAsset textFile = Resources.Load<TextAsset> (filePath);
        Debug.Log ($"level data: {textFile.text}");
        LevelDataModel levelDataModel = JsonUtility.FromJson<LevelDataModel> (textFile.text);
        airportManager.Airports = CreateAirports (levelDataModel.airport);
        spawnController.ListPlanes = LoadPlanes (levelDataModel.planeId);
        LoadMapBackground (levelDataModel.levelBackground);
        LoadSceneBackground (levelDataModel.sceneBackground);
        LoadGameFlags (levelDataModel);
        levelInfo = levelDataModel.info;
        LoadSavedScore (levelInfo.id);
        levelLoaded = true;
    }
    private void LoadSavedScore (string levelID) {
        scoreController.LoadSavedScore (levelID);
        uiManager.viewGamePanel.SetBestFireExtinguished (scoreController.BestFireExtinguished);
        uiManager.viewGamePanel.SetHighScore (scoreController.BestScore);
    }
    private void LoadGameFlags (LevelDataModel model) {
        hasEnemy = model.enemies;
        hasFire = model.fire;
        hasCloud = model.cloud;
        hasTornado = model.tornado;
        hasFuel = model.fuel;

        uiManager.viewGamePanel.ShowFireScoreHolder (hasFire);

    }
    private List<PlaneControl> LoadPlanes (string planeFolder) {
        var planes = Resources.LoadAll<PlaneControl> ($"Planes/{planeFolder}");
        return planes.ToList ();
    }
    private List<PlaneControl> LoadPlanes (int[] planes) {
        List<PlaneControl> planeControllers = new List<PlaneControl> ();
        for (int i = 0; i < planes.Length; i++) {
            PlaneControl plane = Resources.Load<PlaneControl> ($"Planes/plane{planes[i]}");
            if (plane) {
                planeControllers.Add (plane);
            }
        }
        return planeControllers;
    }

    private void LoadSceneBackground (MapImageModel data) {
        Sprite sprite = Resources.Load<Sprite> ($"MapBackground/{data.background}");
        mapGraphicController.SetSceneBackground (sprite);
        mapGraphicController.sceneBackground.transform.position = data.position.ToVector3 ();
        mapGraphicController.sceneBackground.transform.localScale = data.scale.ToVector3 ();

    }
    private void LoadMapBackground (MapImageModel data) {
        Sprite sprite = Resources.Load<Sprite> ($"MapBackground/{data.background}");
        mapGraphicController.SetMapBackground (sprite);
        mapGraphicController.mapBackground.transform.position = data.position.ToVector3 ();
        mapGraphicController.mapBackground.transform.localScale = data.scale.ToVector3 ();
    }
    private void LoadLevelBackground (string mapLink) {
        Sprite sprite = Resources.Load<Sprite> ($"MapBackground/{mapLink}");
        mapGraphicController.SetSceneBackground (sprite);
    }
    private void LoadMapBackground (string mapLink) {
        Sprite sprite = Resources.Load<Sprite> ($"MapBackground/{mapLink}");
        mapGraphicController.SetMapBackground (sprite);
    }
    private List<Airport> CreateAirports (AirportDataModel[] airportData) {
        List<Airport> listAirports = new List<Airport> ();
        foreach (var model in airportData) {
            Airport airport;
            if (model.type == "long") {
                airport = spawnController.CreateAirport ();
            } else {
                airport = spawnController.CreateHelipad ();
            }
            airport.name = model.name;
            airport.transform.position = model.position.ToVector3 ();
            airport.transform.localScale = model.scale.ToVector3 ();
            airport.transform.rotation = Quaternion.Euler (0, 0, model.rotation);
            airport.PlaneTag = model.planeTag;
            Color color;
            ColorUtility.TryParseHtmlString (model.color, out color);
            airport.SetColorHighlight (color);
            airport.Dehighlight ();
            listAirports.Add (airport);
        }
        return listAirports;
    }
    private IEnumerator DelayStartGame (Action callback) {
        yield return new WaitUntil (() => levelLoaded == true);
        Debug.Log ("Wtf");
        callback ();
    }
    private IEnumerator DelayStartGame (float time, Action callback) {
        yield return new WaitForSecondsRealtime (time);
        callback ();
    }
    private void InitSound () {
        SoundController.Instance?.AssignButtonSound ();
    }
}