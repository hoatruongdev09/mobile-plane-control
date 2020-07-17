using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameController : MonoBehaviour {
    public static GameController Instance { get; private set; }
    public int MaxTimeSpeed { get { return maxTimeSpeed; } }
    public bool IsPause { get; set; }
    public PathDrawer pathDrawer;
    public AirportManager airportManager;
    public ScoreController scoreManager;
    public UiManager uiManager;
    public Camera mainCamera;
    [SerializeField] private int maxTimeSpeed = 5;
    private StateMachine stateMachine;
    private GameStateManager stateManager;
    private void Awake () {
        if (Instance == null) {
            Instance = this;
        }
    }

    private void Start () {

        mainCamera = Camera.main;

        stateMachine = new StateMachine ();
        stateManager = new GameStateManager (this, stateMachine);

        stateMachine.Start (stateManager.StartedState);

        scoreManager.onCurrentPlanesChanges += UpdateLandedPlanesUI;
        scoreManager.onCurrentPlanesChanges += UpdateBestScore;
    }

    private void Update () {
        stateMachine.CurrentState.Update ();
    }
    public void OnPlaneLanding (PlaneControl plane) {
        plane.Delete ();
        scoreManager.AddLandedPlane (1);
    }

    private void UpdateLandedPlanesUI (int value) {
        uiManager.viewGamePanel.SetCurrentTextLanded (value);
    }
    private void UpdateBestScore (int value) {
        uiManager.viewGamePanel.SetHighScore (value);
    }

}