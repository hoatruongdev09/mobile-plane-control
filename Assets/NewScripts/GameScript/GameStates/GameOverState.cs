using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NewScript;
using UnityEngine;

public class GameOverState : GameState, IGameOverPanelDelegate {
    private UiManager uIManager;
    private List<PlaneControl> collidedPlanes;
    private GameController controller;
    private UiManager uiManager;
    private ScoreController scoreController;
    private LevelDataInfo levelInfo;
    private LevelScoreInfo savedLevelScore;
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
        LoadScore ();
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
    private void LoadScore () {
        savedLevelScore = scoreController.SavedScore;
    }
    private void SaveScore () {
        var currentScoreInfo = scoreController.GetScoreInfo ();
        var listCrashInfo = savedLevelScore.crashes.ToList ();
        foreach (var planeCollided in collidedPlanes) {
            var crash = listCrashInfo.SingleOrDefault (info => info.plane == planeCollided.name);
            if (crash != null) {
                crash.count++;
            } else {
                listCrashInfo.Add (new CrashInfo () { plane = planeCollided.name.Replace ("(Clone)", ""), count = 1 });
            }
        }
        savedLevelScore.crashes = listCrashInfo.ToArray ();
        savedLevelScore.playTime += 1;
        savedLevelScore.totalAircaftLanded += currentScoreInfo.CurrentLandedPlanes;
        savedLevelScore.averageScore = savedLevelScore.totalAircaftLanded / savedLevelScore.playTime;
        savedLevelScore.mostRecentScore = currentScoreInfo.CurrentLandedPlanes;
        savedLevelScore.bestLandedScore = currentScoreInfo.BestScore;
        savedLevelScore.bestFireExtinguished = currentScoreInfo.BestFireExtinguished;
        savedLevelScore.mostAircraftsOnScreen = currentScoreInfo.MostPlaneOnScreen;
        string jsonData = JsonUtility.ToJson (savedLevelScore);
        PlayerPrefs.SetString (levelInfo.id, jsonData);
        Debug.Log ($"saved data: {PlayerPrefs.GetString(levelInfo.id)}");
    }
    private void ShowScoreSumUp () {
        uIManager.viewGameOverPanel.textBestScore.text = $"{scoreController.BestScore}";
        uIManager.viewGameOverPanel.textLandedPlanes.text = $"{scoreController.CurrentLandedPlanes}";
    }
    private void EndGameEffect () {
        var currentTimeSpeed = Time.timeScale;
        LeanTween.value (controller.gameObject, currentTimeSpeed, 0, .01f).setOnUpdate ((float value) => {
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