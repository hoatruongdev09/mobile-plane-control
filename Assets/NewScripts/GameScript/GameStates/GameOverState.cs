using System;
using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;

public class GameOverState : GameState, IGameOverPanelDelegate {
    private UiManager uIManager;
    private List<PlaneControl> collidedPlanes;
    private GameController controller;
    private UiManager uiManager;
    private ScoreController scoreController;
    private LevelDataInfo levelInfo;
    public GameOverState (GameStateManager stateManager) : base (stateManager) {
        uIManager = stateManager.GameController.uiManager;
        controller = stateManager.GameController;
        uIManager = controller.uiManager;
        scoreController = controller.scoreManager;
    }
    public override void Enter (object options) {
        try {
            levelInfo = (LevelDataInfo) options.GetType ().GetProperty ("info").GetValue (options);
        } catch (Exception e) {
            Debug.LogError (e);
        }
        try {
            collidedPlanes = (List<PlaneControl>) options.GetType ().GetProperty ("collidedPlanes").GetValue (options, null);
        } catch (Exception e) {
            Debug.LogError (e);
        }
        Enter ();
    }
    public override void Enter () {
        foreach (var plane in collidedPlanes) {
            plane.HighlightCrash ();
        }
        ShowScoreSumUp ();
        SaveScore ();
        EndGameEffect ();
        controller.StartCoroutine (DelayToShowGameOverPanel (3, () => {
            uIManager.ChangePanel (uIManager.viewGameOverPanel);
            uIManager.viewGameOverPanel.Delegate = this;
        }));
    }
    public override void Exit () {

    }
    private void SaveScore () {
        var currentScoreInfo = scoreController.GetScoreInfo ();
        var savedScore = new LevelScoreInfo ();
        savedScore.bestLandedScore = currentScoreInfo.BestScore;
        savedScore.bestFireExtinguished = currentScoreInfo.BestFireExtinguished;
        string jsonData = JsonUtility.ToJson (savedScore);
        PlayerPrefs.SetString (levelInfo.id, jsonData);
        Debug.Log ($"saved data: {PlayerPrefs.GetString(levelInfo.id)}");
    }
    private void ShowScoreSumUp () {
        uIManager.viewGameOverPanel.textBestScore.text = $"{scoreController.BestScore}";
        uIManager.viewGameOverPanel.textLandedPlanes.text = $"{scoreController.CurrentLandedPlanes}";
    }
    private void EndGameEffect () {
        var currentTimeSpeed = Time.timeScale;
        LeanTween.value (controller.gameObject, currentTimeSpeed, 0, .1f).setOnUpdate ((float value) => {
            Time.timeScale = Mathf.Clamp (value, 0, Mathf.Infinity);
        }).setEaseInSine ().setIgnoreTimeScale (true);
    }
    private IEnumerator DelayToShowGameOverPanel (float time, Action callback) {
        yield return new WaitForSecondsRealtime (time);
        callback ();
    }

    public void OnWatchAd () {

    }

    public void OnViewAchievement () {

    }

    public void OnViewLeaderboard () {

    }

    public void OnBackToMenu () {
        uIManager.ShowFader (() => {
            controller.BackToMenu ();
        });
    }

    public void OnReset () {
        uIManager.ShowFader (() => {
            controller.RestartGame ();
        });
    }
}