using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UserData {
    public float landedPlane;
    public List<LevelData> listLevelData;

    public UserData () {
        listLevelData = new List<LevelData> ();
    }
    public LevelData FindLevelByName (string lvName) {
        LevelData lvData = listLevelData.Find (x => x.buildSceneName.Equals (lvName));
        return lvData;
    }
}

[Serializable]
public class LevelData {
    public string buildSceneName;
    public int highScore;
    public ScoreInfo[] specialScore;
    public LevelData (string buildSceneName, int highScore, ScoreInfo[] specialScore) {
        this.buildSceneName = buildSceneName;
        this.highScore = highScore;
        this.specialScore = specialScore;
    }
    public int IndexOf (ScoreInfo si) {
        for (int i = 0; i < specialScore.Length; i++) {
            if (specialScore[i].scorename == si.scorename)
                return i;
        }
        return -1;
    }
}

[System.Serializable]
public class ScoreInfo {
    public string scorename;
    public int score;

    public ScoreInfo (string scorename, int score) {
        this.scorename = scorename;
        this.score = score;
    }
}