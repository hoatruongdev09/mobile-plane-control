using System;
using System.Collections;
using System.Collections.Generic;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class GameServiceController : MonoBehaviour
{
    public static GameServiceController Instance { get; private set; }

    private void Awake()
    {
        var controller = FindObjectOfType<GameServiceController>();
        if (controller != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        if (Instance == null)
        {
            Instance = this;
        }
    }
    // private void Start () {
    //     // Initialize ();
    // }

    private void Initialize()
    {
#if UNITY_ANDROID
        // PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder ().RequestIdToken ().Build ();
        // PlayGamesPlatform.InitializeInstance (config);
        // PlayGamesPlatform.DebugLogEnabled = true;
        // PlayGamesPlatform.Activate ();
#endif
#if UNITY_IOS

#endif
    }

    public void Authenticate()
    {
        if (Social.localUser.authenticated) { return; }
        Social.localUser.Authenticate((success, info) =>
        {
            Debug.Log($"Sign in :{success} : {info}");
        });
        // #if UNITY_ANDROID
        //         PlayGamesPlatform.Instance.Authenticate (SignInInteractivity.CanPromptAlways, (result) => {
        //             Debug.Log ($"android sign in result: {result.ToString()}");
        //         });

        // #endif
        // #if UNITY_IOS
        //         Social.localUser.Authenticate ((action) => {
        //             Debug.Log ($"login stats: {action}");
        //         });
        // #endif

    }

    public void UnlockAchievement(string id, Action<bool> callback)
    {
        Social.ReportProgress(id, 100f, (success) =>
        {
            callback(success);
            Debug.Log($"unlocked achievement {id} | ${success}");
        });
    }
    public void IncrementingAchievement(string id, int value, Action<bool> callback)
    {
#if UNITY_ANDROID
        // PlayGamesPlatform.Instance.IncrementAchievement(id, value, (success) =>
        // {
        //     callback(success);
        //     Debug.Log($"increase achievement: ${id} | ${success}");
        // });
#endif
#if UNITY_IOS
        Social.ReportProgress (id, value, (success) => {
            callback (success);
            Debug.Log ($"increase achievement: ${id} | ${success}");
        });
#endif
    }
    public void PostScoreToLeaderboard(string id, long value, Action<bool> callback)
    {
        Social.ReportScore(value, id, (success) =>
        {
            callback(success);
            Debug.Log($"posted score to leaderboard: {id} | {success}");
        });
    }
    public void ShowLeaderboardUI(Action<bool> callback = null)
    {
        if (!Social.localUser.authenticated)
        {
            Debug.Log("user not signed in");
            if (callback != null) callback(false);
            return;
        }
        if (callback != null) callback(true);
        Social.ShowLeaderboardUI();
    }
    public void ShowAchievemenUI(Action<bool> callback = null)
    {
        if (!Social.localUser.authenticated)
        {
            Debug.Log("user not signed in");
            if (callback != null) callback(false);
            return;
        }
        if (callback != null) callback(true);
        Social.ShowAchievementsUI();
    }
}