using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GamePanelView : UiView {
    public IGamePanelViewDelegate Delegate { get; set; }

    [SerializeField] public Text textCurrentFireExtinguished;
    [SerializeField] public Text textBestFireExtinguished;
    [SerializeField] public Text textCurrentLandedPlane;
    [SerializeField] public Text textHighScore;
    [SerializeField] public Button buttonPause;
    [SerializeField] public Button buttonFastForward;
    [SerializeField] public Text textSpeedIndicate;
    public RectTransform fireScoreHolder;
    private void OnEnable () {
        UpdateTextSpeedIndicate ();
    }
    public void Start () {
        buttonPause.onClick.AddListener (ButtonPauseClick);
        buttonFastForward.onClick.AddListener (ButtonFastForwardClick);
    }
    public void SetCurrentTextLanded (int number) {
        textCurrentLandedPlane.text = $"Landed: {number}";
    }
    public void SetHighScore (int number) {
        textHighScore.text = $"Best score: {number}";
    }
    public void SetCurrentFireExtinguished (int number) {
        textCurrentFireExtinguished.text = $"{number}";
    }
    public void SetBestFireExtinguished (int number) {
        textBestFireExtinguished.text = $"{number}";
    }
    private void ButtonPauseClick () {
        // Debug.Log ($"pause: {Delegate == null}");
        Delegate?.OnPauseClick ();
    }
    private void ButtonFastForwardClick () {
        // Debug.Log ("fast forward");
        Delegate?.OnFastForward ();
        UpdateTextSpeedIndicate ();
    }
    public void UpdateTextSpeedIndicate () {
        LeanTween.value (gameObject, 0, 1, 1f).setOnUpdate ((float value) => {
            textSpeedIndicate.text = $"speed: x{string.Format ("{0:0.##}", Time.timeScale)}";
        }).setIgnoreTimeScale (true).setOnComplete (() => {
            if (Time.timeScale == 1) {
                textSpeedIndicate.text = $"speed: normal";
            }
        });
    }
    public void ShowFireScoreHolder (bool action) {
        fireScoreHolder.gameObject.SetActive (action);
    }

}
public interface IGamePanelViewDelegate {
    void OnPauseClick ();
    void OnFastForward ();
}