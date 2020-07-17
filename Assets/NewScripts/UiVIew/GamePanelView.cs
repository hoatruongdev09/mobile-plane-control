using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GamePanelView : UiView {
    public IGamePanelViewDelegate Delegate { get; set; }

    [SerializeField] public Text textCurrentLandedPlane;
    [SerializeField] public Text textHighScore;
    [SerializeField] public Button buttonPause;
    [SerializeField] public Button buttonFastForward;
    [SerializeField] public Text textSpeedIndicate;
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
    private void ButtonPauseClick () {
        Debug.Log ($"pause: {Delegate == null}");
        Delegate?.OnPauseClick ();
    }
    private void ButtonFastForwardClick () {
        Debug.Log ("fast forward");
        Delegate?.OnFastForward ();
        UpdateTextSpeedIndicate ();
    }
    public void UpdateTextSpeedIndicate () {
        LeanTween.value (gameObject, 0, 1, 2f).setOnUpdate ((float value) => {
            textSpeedIndicate.text = $"x{Mathf.Floor(Time.timeScale)}";
        }).setIgnoreTimeScale (true).setOnComplete (() => {
            if (Time.timeScale == 1) {
                textSpeedIndicate.text = $"normal";
            }
        });
    }
}
public interface IGamePanelViewDelegate {
    void OnPauseClick ();
    void OnFastForward ();
}