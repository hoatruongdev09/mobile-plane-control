using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour {
    public static GameController Instance { get; private set; }
    public OnPlaneCollided onPlaneCollided { get; set; }
    public int MaxTimeSpeed { get { return maxTimeSpeed; } }
    public bool IsPause { get; set; }
    public PathDrawer pathDrawer;
    public AirportManager airportManager;
    public ScoreController scoreManager;
    public SpawnController spawnController;
    public AdsController adsController;
    public MapGraphicController mapGraphicController;
    public UiManager uiManager;
    public Camera mainCamera;
    [SerializeField] private int maxTimeSpeed = 5;
    private StateMachine stateMachine;
    private GameStateManager stateManager;
    public delegate void OnPlaneCollided (PlaneControl plane);
    public delegate void OnPlaneLanded ();
    private void Awake () {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start () {
        mainCamera = Camera.main;
        stateMachine = new StateMachine ();
        stateManager = new GameStateManager (this, stateMachine);
        StartCoroutine (InitializeCoroutine ());
    }

    private void Update () {
        stateMachine.CurrentState?.Update ();
    }

    public void RestartGame () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
    public void BackToMenu () {
        StartCoroutine (DelayBackToMenu ());
    }
    private IEnumerator DelayBackToMenu () {
        PlayerSection.Instance?.SaveSection ();
        adsController?.CloseBannerAd ();
        yield return null;
        SceneManager.LoadScene (0);
    }
    private IEnumerator InitializeCoroutine () {
        var selectedLevel = "levelDataSample";
        var selectedDifficult = "levelDifficultSample";
        if (PlayerSection.Instance) {
            selectedLevel = PlayerSection.Instance.LastChooseLevelID;
        }
        yield return new WaitForEndOfFrame ();
        stateMachine.Start (stateManager.InitState, new { level = selectedLevel, difficult = selectedDifficult });
        yield return new WaitUntil (() => CrossSceneData.Instance != null);
        yield return new WaitUntil (() => AdsController.Instance != null);
        adsController = AdsController.Instance;
        Debug.Log ($"is remove ad: {CrossSceneData.Instance.IsRemoveAd}");
        if (!CrossSceneData.Instance.IsRemoveAd) {
            adsController.RequestBannerAd ();
            adsController.ShowBannerAd ();
        }
    }
}