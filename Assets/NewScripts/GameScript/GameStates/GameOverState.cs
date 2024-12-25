using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NewScript;
using UnityEngine;

public class GameOverState : GameState, IGameOverPanelDelegate
{
    private UiManager uIManager;
    private List<PlaneControl> collidedPlanes;
    private GameController controller;
    private UiManager uiManager;
    private ScoreController scoreController;
    private LevelDataInfo levelInfo;
    private LevelScoreInfo savedLevelScore;
    private AdsController adsController;
    private bool watchToEndAd = false;
    private bool loadedAd = true;
    private bool delayShowAd = false;
    private bool isContinuePlayed = false;
    private bool isAdClosed = false;
    public GameOverState(GameStateManager stateManager) : base(stateManager)
    {
        uIManager = stateManager.GameController.uiManager;
        controller = stateManager.GameController;
        uIManager = controller.uiManager;
        scoreController = controller.scoreManager;

    }
    public override void Enter(object options)
    {
        try
        {
            levelInfo = (LevelDataInfo)options.GetType().GetProperty("info").GetValue(options);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        try
        {
            collidedPlanes = (List<PlaneControl>)options.GetType().GetProperty("collidedPlanes").GetValue(options, null);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        Enter();
    }
    public override void Enter()
    {
        InitializeAdControl();
        foreach (var plane in collidedPlanes)
        {
            plane.HighlightCrash();
        }
        LoadScore();
        ShowScoreSumUp();
        SaveScore();
        EndGameEffect();
        if (delayShowAd)
        {
            // adsController.ShowRewardAd();
        }
        controller.StartCoroutine(DelayToShowGameOverPanel(3, () =>
        {
            uIManager.ChangePanel(uIManager.viewGameOverPanel);
            uIManager.viewGameOverPanel.Delegate = this;
        }));
    }

    public override void Exit()
    {
        uIManager.ChangePanel(uIManager.viewGamePanel);
    }
    private void InitializeAdControl()
    {
        adsController = controller.adsController;
        if (adsController == null)
        {
            controller.uiManager.viewGameOverPanel.DisableWatchAdButton();
            return;
        }

        // adsController.RewardAdDelegate = this;
        // if (!adsController.RewardAdLoaded())
        // {
        //     adsController.RequestRewardAd();
        // }
    }
    private void LoadScore()
    {
        savedLevelScore = scoreController.SavedScore;
    }
    private void SaveScore()
    {
        var currentScoreInfo = scoreController.GetScoreInfo();
        var listCrashInfo = savedLevelScore.crashes.ToList();
        foreach (var planeCollided in collidedPlanes)
        {
            var crash = listCrashInfo.SingleOrDefault(info => info.plane == planeCollided.name);
            if (crash != null)
            {
                crash.count++;
            }
            else
            {
                listCrashInfo.Add(new CrashInfo() { plane = planeCollided.name.Replace("(Clone)", ""), count = 1 });
            }
        }
        savedLevelScore.crashes = listCrashInfo.ToArray();
        savedLevelScore.playTime += 1;
        savedLevelScore.totalAircaftLanded += currentScoreInfo.CurrentLandedPlanes;
        savedLevelScore.averageScore = savedLevelScore.totalAircaftLanded / savedLevelScore.playTime;
        savedLevelScore.mostRecentScore = currentScoreInfo.CurrentLandedPlanes;
        savedLevelScore.bestLandedScore = currentScoreInfo.BestScore;
        savedLevelScore.bestFireExtinguished = currentScoreInfo.BestFireExtinguished;
        savedLevelScore.mostAircraftsOnScreen = currentScoreInfo.MostPlaneOnScreen;
        string jsonData = JsonUtility.ToJson(savedLevelScore);
        PlayerPrefs.SetString(levelInfo.id, jsonData);
        Debug.Log($"saved data: {PlayerPrefs.GetString(levelInfo.id)}");
    }
    private void ShowScoreSumUp()
    {
        uIManager.viewGameOverPanel.textBestScore.text = $"{scoreController.BestScore}";
        uIManager.viewGameOverPanel.textLandedPlanes.text = $"{scoreController.CurrentLandedPlanes}";
    }
    private void EndGameEffect()
    {
        var currentTimeSpeed = Time.timeScale;
        LeanTween.value(controller.gameObject, currentTimeSpeed, 0, .01f).setOnUpdate((float value) =>
        {
            Time.timeScale = Mathf.Clamp(value, 0, Mathf.Infinity);
        }).setEaseInSine().setIgnoreTimeScale(true);
    }
    private IEnumerator DelayToShowGameOverPanel(float time, Action callback)
    {
        yield return new WaitForSecondsRealtime(time);
        callback();
    }

    public void OnWatchAd()
    {
        if (isContinuePlayed)
        {
            return;
        }
        if (!loadedAd)
        {
            delayShowAd = true;
            ContinuePlaying();
            return;
        }
        // if (adsController.ShowRewardAd())
        // {
        //     Debug.Log("OK UNITY");
        //     this.controller.StartCoroutine(DelayContinuePlaying());
        // }
        // else
        // {
        //     Debug.Log("FAIL UNITY");
        // }

        // adsController.ShowRewardAd ();

    }
    private void ContinuePlaying()
    {
        isContinuePlayed = true;
        stateManager.StateMachine.ChangeState(stateManager.StartedState, new
        {
            continueOver = true,
            crashedPlanes = collidedPlanes
        });
        controller.uiManager.viewGameOverPanel.DisableWatchAdButton();
    }
    private IEnumerator DelayContinuePlaying()
    {
        yield return new WaitUntil(() => isAdClosed == true);
        if (watchToEndAd && !delayShowAd)
        {
            ContinuePlaying();
        }
    }
    public void OnViewAchievement()
    {
        GameServiceController.Instance?.ShowAchievemenUI((success) =>
        {
            if (!success)
            {
                GameServiceController.Instance?.Authenticate();
            }
        });
    }

    public void OnViewLeaderboard()
    {
        GameServiceController.Instance?.ShowLeaderboardUI((success) =>
        {
            if (!success)
            {
                GameServiceController.Instance?.Authenticate();
            }
        });
    }

    public void OnBackToMenu()
    {
        uIManager.ShowFader(() =>
        {
            controller.BackToMenu();
        });
    }

    public void OnReset()
    {
        uIManager.ShowFader(() =>
        {
            controller.RestartGame();
        });
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("ad closed");
        isAdClosed = true;
    }

    // public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    // {
    //     loadedAd = false;
    //     Debug.LogError($"Failed to load ad: {args.Message}");
    // }

    // public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    // {
    //     Debug.LogError($"Failed to show ad: {args.Message}");
    // }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        loadedAd = true;
        Debug.Log($"Ad loaded: {args.ToString()}");
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log($"Ad opening: {args.ToString()}");
    }

    // public void HandleUserEarnedReward(object sender, Reward args)
    // {
    //     Debug.Log($"Ad rewarded: {args.ToString()}");
    //     watchToEndAd = true;
    // }
}