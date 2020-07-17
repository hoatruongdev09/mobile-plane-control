using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour {
    public OnScoreChanges onCurrentPlanesChanges { get; set; }
    public OnScoreChanges onBestScoreChanges { get; set; }
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
    private int currentLandedPlanes;
    private int bestScore;
    public delegate void OnScoreChanges (int value);
    public void AddLandedPlane (int number) {
        currentLandedPlanes += number;
        onCurrentPlanesChanges?.Invoke (currentLandedPlanes);
    }
    public void ChangeBestScore (int number) {
        bestScore = number;
        onBestScoreChanges?.Invoke (bestScore);
    }
}