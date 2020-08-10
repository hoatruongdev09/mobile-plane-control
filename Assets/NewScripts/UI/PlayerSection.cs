using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSection : MonoBehaviour {
    public static PlayerSection Instance { get; set; }
    public string LastChooseLevelID {
        get { return PlayerData.lastPlayedLevelID; }
        set { PlayerData.lastPlayedLevelID = value; }
    }
    public PlayerData PlayerData {
        get { return playerData; }
        set { playerData = value; }
    }

    [SerializeField] private PlayerData playerData;
    private void Awake () {
        var playerSection = FindObjectOfType<PlayerSection> ();
        if (playerSection.gameObject != this.gameObject) {
            Destroy (this.gameObject);
        } else {
            DontDestroyOnLoad (this.gameObject);
        }
        if (Instance == null) {
            Instance = this;
        }
    }
    public void SaveSection () {
        var jsonData = JsonUtility.ToJson (PlayerData);
        PlayerPrefs.SetString ("player-data", jsonData);
    }
}