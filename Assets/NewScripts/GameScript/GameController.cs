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
        stateMachine.Start (stateManager.InitState, new { level = "levelDataSample", difficult = "levelDifficultSample" });
    }

    private void Update () {
        stateMachine.CurrentState.Update ();
    }

    public void RestartGame () {
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
    }
    public void BackToMenu () {

    }
}