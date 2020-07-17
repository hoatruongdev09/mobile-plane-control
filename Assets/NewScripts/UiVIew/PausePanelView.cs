using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PausePanelView : UiView {
    public IPausePanelDelegate Delegate { get; set; }
    public Button buttonMainMenu;
    public Button buttonContinue;
    public Button buttonRestart;

    public Button buttonSound;
    public Button buttonMusic;

    private void Start () {
        buttonMainMenu.onClick.AddListener (ButtonMainMenu);
        buttonContinue.onClick.AddListener (ButtonContinue);
        buttonRestart.onClick.AddListener (ButtonRestart);
        buttonSound.onClick.AddListener (ButtonSound);
        buttonMusic.onClick.AddListener (ButtonMusic);
    }

    private void ButtonMainMenu () {
        Delegate?.OnBackToMenu ();
    }
    private void ButtonContinue () {
        Delegate?.OnContinues ();
    }
    private void ButtonRestart () {
        Delegate?.OnRestart ();
    }
    private void ButtonSound () {
        Delegate?.OnSoundInteract ();
    }
    private void ButtonMusic () {
        Delegate?.OnMusicInteract ();
    }

    protected override void AnimateShow (System.Action callback) {
        canvasGroup.LeanAlpha (1, 1f).setOnComplete (callback).setIgnoreTimeScale (true);
    }
    protected override void AnimateHide (System.Action callback) {
        canvasGroup.LeanAlpha (0, 1f).setOnComplete (callback).setIgnoreTimeScale (true);
    }
}

public interface IPausePanelDelegate {
    void OnBackToMenu ();
    void OnContinues ();
    void OnRestart ();
    void OnSoundInteract ();
    void OnMusicInteract ();
}