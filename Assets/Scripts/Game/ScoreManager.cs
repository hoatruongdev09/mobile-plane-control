using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance { get; private set; }

    [SerializeField]
    private int landedPlane;
    public ScoreInfo[] specialScore;

    private LevelData levelData;
    private void Start () {
        OnReset ();
    }
    public void OnReset () {
        landedPlane = 0;
        foreach (ScoreInfo si in specialScore) {
            si.score = 0;
        }
        Instance = this;
        StartCoroutine (AfterInit ());
    }
    public void AddScore () {
        landedPlane++;
        InGameUIControl.Instance.UpdateText_landedPlane (landedPlane);
    }
    public void OnGameOver () {
        CheckHighestScore ();
    }
    public void CheckHighestScore () {
        if (landedPlane > levelData.highScore) {
            levelData.highScore = landedPlane;
        }
        for (int i = 0; i < levelData.specialScore.Length; i++) {
            if (specialScore[i].score > levelData.specialScore[i].score) {
                levelData.specialScore[i].score = specialScore[i].score;
            }
        }
        SaveCurrentScore ();
    }
    public int GetHighestScore () {
        return levelData.highScore;
    }
    public int GetLandedPlane () {
        return landedPlane;
    }
    public void ScoreSpecial (string name, int score) {
        foreach (ScoreInfo si in specialScore) {
            if (si.scorename == name) {
                si.score += score;
                InGameUIControl.Instance.UpdateText_SpecialScore (levelData.IndexOf (si), si.score);
                return;
            }
        }
        Debug.Log ("no ScoreInfo for: " + name);
    }
    public int[] GetSpecialScore () {
        List<int> scores = new List<int> ();
        foreach (ScoreInfo si in levelData.specialScore) {
            scores.Add (si.score);
        }
        return scores.ToArray ();
    }
    public void SaveCurrentScore () {
        SaveLoadManager.Instance.AddTotalLandedPlane (landedPlane);
        SaveLoadManager.Instance.SaveUserData ();
    }
    private IEnumerator AfterInit () {
        yield return new WaitUntil (() => InGameUIControl.Instance != null);
        InGameUIControl.Instance.UpdateText_landedPlane (0);
        yield return new WaitUntil (() => SaveLoadManager.Instance != null);
        string sceneName = SceneManager.GetActiveScene ().name;
        levelData = SaveLoadManager.Instance.GetLevelData (sceneName);
        if (levelData == null) {
            Debug.Log ("We dont have any data for this scene");
            levelData = new LevelData (sceneName, 0, specialScore);
            SaveLoadManager.Instance.AddLevelData (levelData);
        } else {
            specialScore = new ScoreInfo[levelData.specialScore.Length];
            for (int i = 0; i < specialScore.Length; i++) {
                specialScore[i] = new ScoreInfo (levelData.specialScore[i].scorename, 0);
            }
        }
    }

}