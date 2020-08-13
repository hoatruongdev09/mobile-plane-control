using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatView : UiView {
    public HighScoreView highScoreView;
    public Text textAverageScore;
    public Text textMostRecentScore;
    public Text textMostAircraftsOnScreen;
    public Text textTotalAircraftLanded;
    public Image imgMostLikelyCrash;
    public Text textMostLikelyCrash;
    public Button buttonAchievement;
    public Button buttonLeaderboard;

    public void Start () {
        buttonAchievement.onClick.AddListener (ButtonAchievement);
        buttonLeaderboard.onClick.AddListener (ButtonLeaderboard);
    }
    public void SetStat (LevelScoreInfo info) {
        Debug.Log ($"into : {info == null}");
        textAverageScore.text = info.averageScore.ToString ();
        textMostRecentScore.text = info.mostRecentScore.ToString ();
        textMostAircraftsOnScreen.text = info.mostAircraftsOnScreen.ToString ();
        textTotalAircraftLanded.text = info.totalAircaftLanded.ToString ();
        CrashInfo mostCrash = info.GetMostCrashInfo ();
        if (mostCrash == null) {
            Debug.Log ("Wtf");
            imgMostLikelyCrash.gameObject.SetActive (false);
            textMostLikelyCrash.gameObject.SetActive (true);
        } else {
            string planeString = mostCrash.plane.Replace ("(Clone)", "");
            Debug.Log ($"plane string {planeString}");
            var plane = Resources.Load<PlaneControl> ($"Planes/{planeString}");
            Debug.Log ($"sprite: {plane == null}");
            imgMostLikelyCrash.sprite = plane.graphics[0].sprite;
            imgMostLikelyCrash.preserveAspect = true;
            imgMostLikelyCrash.gameObject.SetActive (true);
            textMostLikelyCrash.gameObject.SetActive (false);
        }
        if (info.bestLandedScore > 0) {
            highScoreView.textHighScore.text = info.bestLandedScore.ToString ();
            highScoreView.Show ();
        } else {
            highScoreView.Hide ();
        }
    }
    private void ButtonAchievement () {
        GameServiceController.Instance?.ShowAchievemenUI ((success) => {
            if (!success) {
                GameServiceController.Instance?.Authenticate ();
            }
        });
    }
    private void ButtonLeaderboard () {
        GameServiceController.Instance?.ShowLeaderboardUI ((success) => {
            if (!success) {
                GameServiceController.Instance?.Authenticate ();
            }
        });
    }
}