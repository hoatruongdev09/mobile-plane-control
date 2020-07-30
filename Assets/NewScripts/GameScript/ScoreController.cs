using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {
    public OnScoreChanges onCurrentPlanesChanges { get; set; }
    public OnScoreChanges onBestScoreChanges { get; set; }
    public OnScoreChanges onCurrentFireExtinguishedChanges { get; set; }
    public OnScoreChanges onBestFireExtinguishedChange { get; set; }
    public int CurrentLandedPlanes {
        get { return currentLandedPlanes; }
        set {
            currentLandedPlanes = value;
            onCurrentPlanesChanges.Invoke (currentLandedPlanes);
            if (currentLandedPlanes >= bestScore) {
                BestScore = currentLandedPlanes;
            }
        }
    }
    public int BestScore {
        get { return bestScore; }
        set {
            bestScore = value;
            onBestScoreChanges?.Invoke (bestScore);
        }
    }
    public int CurrentFireExtinguished {
        get { return currentFireExtinguished; }
        set {
            currentFireExtinguished = value;
            onCurrentFireExtinguishedChanges?.Invoke (currentFireExtinguished);
            if (currentFireExtinguished > bestFireExtinguished) {
                BestFireExtinguished = currentFireExtinguished;
            }
        }
    }
    public int BestFireExtinguished {
        get { return bestFireExtinguished; }
        set {
            bestFireExtinguished = value;
            onBestFireExtinguishedChange?.Invoke (bestFireExtinguished);
        }
    }
    private int currentFireExtinguished;
    private int bestFireExtinguished;
    private int currentLandedPlanes;
    private int bestScore;
    public delegate void OnScoreChanges (int value);
    public void AddLandedPlane (int number) {
        CurrentLandedPlanes += number;
    }
    public void AddFireExtinguished (int number) {
        CurrentFireExtinguished += number;
    }
    public void ChangeBestScore (int number) {
        bestScore = number;
        onBestScoreChanges?.Invoke (bestScore);
    }
    public CurrentLevelScoreInfo GetScoreInfo () {
        return new CurrentLevelScoreInfo () {
            CurrentFireExtinguished = currentFireExtinguished,
                BestFireExtinguished = bestFireExtinguished,
                CurrentLandedPlanes = currentLandedPlanes,
                BestScore = bestScore
        };
    }
}
public class CurrentLevelScoreInfo {
    public int CurrentFireExtinguished { get; set; }
    public int BestFireExtinguished { get; set; }
    public int CurrentLandedPlanes { get; set; }
    public int BestScore { get; set; }
}
public class LevelScoreInfo {
    public int bestLandedScore;
    public int bestFireExtinguished;
}