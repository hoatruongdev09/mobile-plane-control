using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {
    public LevelScoreInfo SavedScore {
        get { return savedScore; }
        private set { savedScore = value; }
    }
    public OnScoreChanges onCurrentPlanesChanges { get; set; }
    public OnScoreChanges onBestScoreChanges { get; set; }
    public OnScoreChanges onCurrentFireExtinguishedChanges { get; set; }
    public OnScoreChanges onBestFireExtinguishedChange { get; set; }
    public int CurrentLandedPlanes {
        get { return currentLandedPlanes; }
        set {
            currentLandedPlanes = value;
            onCurrentPlanesChanges.Invoke (currentLandedPlanes);
            if (currentLandedPlanes >= savedScore.bestLandedScore) {
                BestScore = currentLandedPlanes;
            }
        }
    }
    public int BestScore {
        get { return savedScore.bestLandedScore; }
        set {
            savedScore.bestLandedScore = value;
            onBestScoreChanges?.Invoke (savedScore.bestLandedScore);
        }
    }
    public int CurrentFireExtinguished {
        get { return currentFireExtinguished; }
        set {
            currentFireExtinguished = value;
            onCurrentFireExtinguishedChanges?.Invoke (currentFireExtinguished);
            if (currentFireExtinguished > savedScore.bestFireExtinguished) {
                BestFireExtinguished = currentFireExtinguished;
            }
        }
    }
    public int BestFireExtinguished {
        get { return savedScore.bestFireExtinguished; }
        set {
            savedScore.bestFireExtinguished = value;
            onBestFireExtinguishedChange?.Invoke (savedScore.bestFireExtinguished);
        }
    }
    public int MostPlaneInScreen {
        get { return savedScore.mostAircraftsOnScreen; }
        set { savedScore.mostAircraftsOnScreen = value; }
    }
    private int currentFireExtinguished;
    // private int bestFireExtinguished;
    private int currentLandedPlanes;
    // private int bestScore;
    // private int mostPlaneInScreen;
    private LevelScoreInfo savedScore;
    public delegate void OnScoreChanges (int value);
    public void LoadSavedScore (string level) {
        if (PlayerPrefs.HasKey (level)) {
            var jsonData = PlayerPrefs.GetString (level);
            savedScore = JsonUtility.FromJson<LevelScoreInfo> (jsonData);
        } else {
            savedScore = new LevelScoreInfo ();
            savedScore.crashes = new CrashInfo[] { };
        }
    }
    public void SavePlayerScore (string key) {
        var jsonData = JsonUtility.ToJson (savedScore);
        PlayerPrefs.SetString (key, jsonData);
    }
    public void AddLandedPlane (int number) {
        CurrentLandedPlanes += number;
        if (PlayerSection.Instance != null) {
            PlayerSection.Instance.PlayerData.totalPlaneLanded++;
        }
    }
    public void AddFireExtinguished (int number) {
        CurrentFireExtinguished += number;
    }
    public CurrentLevelScoreInfo GetScoreInfo () {
        return new CurrentLevelScoreInfo () {
            CurrentFireExtinguished = this.CurrentFireExtinguished,
                BestFireExtinguished = this.BestFireExtinguished,
                CurrentLandedPlanes = this.CurrentLandedPlanes,
                BestScore = this.BestScore,
                MostPlaneOnScreen = this.MostPlaneInScreen
        };
    }
}

[System.Serializable]
public class CurrentLevelScoreInfo {
    public int CurrentFireExtinguished { get; set; }
    public int BestFireExtinguished { get; set; }
    public int CurrentLandedPlanes { get; set; }
    public int MostPlaneOnScreen { get; set; }
    public int BestScore { get; set; }
}

[System.Serializable]
public class LevelScoreInfo {
    public int playTime;
    public int bestLandedScore;
    public int bestFireExtinguished;
    public int averageScore;
    public int mostRecentScore;
    public int mostAircraftsOnScreen;
    public int totalAircaftLanded;
    public CrashInfo[] crashes;

    public CrashInfo GetMostCrashInfo () {
        if (crashes == null) { return null; }
        if (crashes.Length == 0) {
            return null;
        }
        var mostCrash = crashes[0];
        foreach (var crash in crashes) {
            if (crash.count > mostCrash.count) {
                mostCrash = crash;
            }
        }
        return mostCrash;
    }
}

[System.Serializable]
public class CrashInfo {
    public string plane;
    public int count;
}