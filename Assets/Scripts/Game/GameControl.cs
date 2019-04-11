using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {
    public static GameControl Instance { get; private set; }

    public bool isJustContinue;
    public bool isGameOver;

    public string[] planeTags = new string[] { "plane_red", "plane_yellow", "heli_blue" };

    public Color bluePlaneColor = new Color32 (10, 189, 227, 255);
    public Color redPlaneColor = new Color32 (238, 82, 83, 255);
    public Color yellowPlaneColor = new Color32 (255, 249, 76, 255);
    public Color defaultLineColor = new Color32 (255, 255, 255, 255);

    private ScoreManager scoreManager;
    private void Start () {
        Instance = this;
        scoreManager = GetComponent<ScoreManager> ();
        isJustContinue = false;
        isGameOver = false;
    }

    public void OnGameOver () {
        if (!isGameOver) {
            StartCoroutine (DelayGameOver ());
        }
    }
    public void OnContinue () {
        StartCoroutine (ContinueDelay (3f));
        InGameUIControl.Instance.OnReset ();
        Time.timeScale = 1;
    }
    public void OnReset () {
        scoreManager.OnReset ();
        InGameUIControl.Instance.OnReset ();
        SpawnManager.Instance.OnReset ();
        isJustContinue = false;
        isGameOver = false;
        Time.timeScale = 1;
    }
    public void FastForwardTime () {
        if (isGameOver) {
            return;
        }
        if (Time.timeScale != 1) {
            Time.timeScale = 1;
        } else {
            Time.timeScale = 1.5f;
        }
    }

    private IEnumerator DelayGameOver () {
        isGameOver = true;
        yield return new WaitForSecondsRealtime (3f);
        scoreManager.OnGameOver ();
        InGameUIControl.Instance.GameOver ();
    }
    public string GetPlaneTag () {
        return planeTags[Random.Range (0, planeTags.Length)];
    }
    public string[] GetPlaneInfo (string planeTag) {
        return planeTag.Split ('_');
    }
    public Color GetColor (string planeTag) {
        switch (planeTag) {
            case "red":
                return redPlaneColor;
            case "yellow":
                return yellowPlaneColor;
            case "blue":
                return bluePlaneColor;
            default:
                return new Color (1, 1, 1, 1);
        }
    }
    private IEnumerator ContinueDelay (float time) {
        isJustContinue = true;
        isGameOver = false;
        yield return new WaitForSecondsRealtime (time);
        isJustContinue = false;
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
}