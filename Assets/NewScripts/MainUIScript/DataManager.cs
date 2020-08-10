using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour {
    public static DataManager Instance { get; private set; }
    public List<LevelDataModel> LevelData { get { return levelsData; } }

    [SerializeField] private List<LevelDataModel> levelsData;
    private void Awake () {
        var dataManager = FindObjectOfType<DataManager> ();
        if (dataManager.gameObject != this.gameObject) {
            Destroy (this.gameObject);
        } else {
            DontDestroyOnLoad (this.gameObject);
        }
        if (Instance == null) {
            Instance = this;
        }
    }
    public LevelScoreInfo LoadLevelScoreInfo (string levelId) {
        if (!PlayerPrefs.HasKey (levelId)) {
            var info = new LevelScoreInfo ();
            return info;
        }
        var data = JsonUtility.FromJson<LevelScoreInfo> (PlayerPrefs.GetString (levelId));
        return data;
    }
    public LevelDataModel GetLevelDataByID (string id) {
        foreach (var levelData in levelsData) {
            if (levelData.info.id == id) { return levelData; }
        }
        return null;
    }
    public PlayerData LoadPlayerData () {
        PlayerData playerData;
        if (!PlayerPrefs.HasKey ("player-data")) {
            playerData = new PlayerData ();
            playerData.savedVersion = Application.version;
            playerData.totalPlaneLanded = 0;
            playerData.unlockedLevel = new string[] { "Island" };
            playerData.lastPlayedLevelID = "Island 2";
            var saveJsonData = JsonUtility.ToJson (playerData);
            PlayerPrefs.SetString ("player-data", saveJsonData);
            return playerData;
        }
        var jsonData = PlayerPrefs.GetString ("player-data");
        Debug.Log ($"data: {jsonData}");
        playerData = JsonUtility.FromJson<PlayerData> (jsonData);
        return playerData;
    }
    public void LoadLevelData () {
        levelsData.Clear ();
        TextAsset[] allData = Resources.LoadAll<TextAsset> ("LevelData");
        foreach (var data in allData) {
            var dataModel = JsonUtility.FromJson<LevelDataModel> (data.text);
            levelsData.Add (dataModel);
        }
    }
}