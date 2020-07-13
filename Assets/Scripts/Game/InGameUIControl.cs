using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class InGameUIControl : MonoBehaviour {

    public static InGameUIControl Instance { get; private set; }

    public GameObject panel_game;
    public GameObject panel_pause;
    public GameObject panel_gameOver;
    public GameObject panel_tutor;
    public Text txt_bestScore;
    public Text txt_landedPlane;
    public Text[] txt_specialScore;
    public Text[] txt_bestSpecialScore;
    public Text txt_gameoverLandedPlane;
    public Text txt_gameoverBestScore;
    public Text txt_fastforwardTime;
    public Text txt_currentSpeed;
    public Text txt_fuel;
    public Image img_fader;
    public bool hasTutor;
    GameObject canvas_worldSpace;
    private ScoreManager scoreManager;
    string stringPrefix = "";
    int multiplier = 1;
    List<string> randNum = new List<string> ();
    int count = 0;
    private GameObject textClone;
    private void Awake () {
        scoreManager = GetComponent<ScoreManager> ();
    }
    private void Start () {
        Instance = this;
        canvas_worldSpace = GameObject.Find ("Canvas_WorldSpace");
        panel_game.SetActive (true);
        panel_pause.SetActive (false);
        panel_gameOver.SetActive (false);
        if (hasTutor) {
            panel_tutor.SetActive (true);
            SpawnManager.Instance.isShowTutor = true;
        } else {
            SpawnManager.Instance.isShowTutor = false;
            panel_tutor?.SetActive (false);
        }
        StartCoroutine (DelayInit ());
    }
    public void UpdateText_landedPlane (int text) {
        // txt_landedPlane.text = "Landed plane:	" + string.Format ("{0:0000}", text);
        DoTextAnimation (text, txt_landedPlane);
    }
    public void UpdateText_SpecialScore (int index, int text) {
        DoTextAnimation (text, txt_specialScore[index]);
    }
    public void UpdateText_BestSpecialScore (int index, int text) {
        if (index < txt_bestSpecialScore.Length)
            DoTextAnimation (text, txt_bestSpecialScore[index]);
    }
    public void Button_Continue () {

#if UNITY_EDITOR
        GameControl.Instance.OnContinue ();
#else
        AdsManager.Instance.ShowRewardVideo ();
#endif
    }
    public void Button_FastForward () {
        GameControl.Instance.FastForwardTime ();
        txt_fastforwardTime.text = "x" + Time.timeScale;
        Animator txt_fastforwardTime_anim = txt_fastforwardTime.GetComponent<Animator> ();
        txt_fastforwardTime_anim.ResetTrigger ("flash");
        txt_fastforwardTime_anim.SetTrigger ("flash");
        if (Time.timeScale == 1) {
            txt_currentSpeed.text = "";
        } else {
            txt_currentSpeed.text = "x" + Time.timeScale;
        }
    }
    public void Button_Pause () {
        panel_pause.SetActive (true);
        panel_game.SetActive (false);
        Time.timeScale = 0;
    }
    public void Button_Restart () {
        GameControl.Instance.OnReset ();
    }
    public void Button_Unpause () {
        panel_pause.SetActive (false);
        panel_game.SetActive (true);
        Time.timeScale = 1;
    }

    public void Button_ShowAchievements () {
        //stuff
        Debug.Log ("Show achievement");
    }
    public void Button_ShowLeaderboard () {
        //stuff
        Debug.Log ("show leaderboard");
    }
    public void Button_Tutor_OK () {
        SpawnManager.Instance.isShowTutor = false;
        panel_tutor.SetActive (false);
    }
    public void Button_LevelMenu () {
        img_fader.GetComponent<Animator> ().SetTrigger ("open");
        StartCoroutine (DelayOpenLevelMenu ());
    }
    public void ShowTextFuel (string value, Vector3 pos) {
        if (int.Parse (value) == 0)
            return;
        textClone = Instantiate (txt_fuel.gameObject, pos, Quaternion.identity);
        textClone.transform.parent = canvas_worldSpace.transform;
        textClone.GetComponent<Text> ().text = value;
        if (int.Parse (value) < 5) {
            textClone.GetComponent<Text> ().color = new Color32 (236, 83, 87, 0);
            textClone.transform.GetChild (0).GetComponent<Image> ().color = new Color32 (236, 83, 87, 0);
        }
        textClone.GetComponent<Animator> ().SetTrigger ("count");
        StartCoroutine (DestroyAfter (textClone, 1f));
    }
    public void GameOver () {
        panel_game.SetActive (false);
        panel_gameOver.SetActive (true);
        txt_gameoverBestScore.text = string.Format ("{0:0000}", scoreManager.GetHighestScore ());
        txt_gameoverLandedPlane.text = string.Format ("{0:0000}", scoreManager.GetLandedPlane ());

    }
    public void OnReset () {
        panel_game.SetActive (true);
        panel_pause.SetActive (false);
        panel_gameOver.SetActive (false);
        // txt_bestScore.text = string.Format ("Best scores: {0:0000}", scoreManager.GetHighestScore ());
        DoTextAnimation (scoreManager.GetHighestScore (), txt_bestScore);
        int[] specialScores = ScoreManager.Instance.GetSpecialScore ();
        for (int i = 0; i < specialScores.Length; i++) {
            Debug.Log ("special score: " + specialScores[i]);
            UpdateText_BestSpecialScore (i, specialScores[i]);
        }
    }

    private IEnumerator DelayOpenLevelMenu () {
        scoreManager.SaveCurrentScore ();
        yield return new WaitForSecondsRealtime (1);
        PlayerPrefs.SetInt ("open_level", 1);
        SceneManager.LoadScene ("MainMenu2");
    }
    private IEnumerator DelayInit () {
        yield return new WaitUntil (() => SaveLoadManager.Instance != null);
        yield return new WaitUntil (() => scoreManager != null);
        // txt_bestScore.text = string.Format ("Best scores: {0:0000}", scoreManager.GetHighestScore ());
        ResetFlip ();
        DoTextAnimation (scoreManager.GetHighestScore (), txt_bestScore);
        int[] specialScores = ScoreManager.Instance.GetSpecialScore ();
        for (int i = 0; i < specialScores.Length; i++) {
            Debug.Log ("special score: " + specialScores[i]);
            UpdateText_BestSpecialScore (i, specialScores[i]);
        }
    }
    private void DoTextAnimation (int number, Text textObject) {

        // LeanTween.cancel (this.gameObject);
        if (number >= multiplier * 10) {
            Debug.Log (number + ">=" + multiplier * 10);
            stringPrefix = "{" + count + "}";
            count++;
            multiplier = multiplier * 10;
            randNum.Add ("");
            DoTextAnimation (number, textObject);
        } else {
            LeanTween.value (this.gameObject, 0, 1, 1.0f).setOnUpdate ((float val) => {
                if (number % 10 != 0) {
                    textObject.text = string.Format ("{0}{1}", number / 10, Random.Range (0, 10));
                } else {
                    for (int i = 0; i < randNum.Count; i++) {
                        randNum[i] = Random.Range (0, 10).ToString ();
                    }
                    stringPrefix = string.Join ("", randNum.ToArray ());
                    textObject.text = stringPrefix;
                }

            }).setOnComplete (onComplete => {
                textObject.text = number.ToString ();
            });
        }
    }
    private void ResetFlip () {
        stringPrefix = "";
        multiplier = 1;
        randNum = null;
        randNum = new List<string> ();
        count = 0;
        randNum.Add ("");
    }
    IEnumerator DestroyAfter (GameObject what, float duration) {
        yield return new WaitForSeconds (duration);
        Destroy (what);
    }
}