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
        buttonSound.onClick.AddListener (ButtonSound);
        buttonMusic.onClick.AddListener (ButtonMusic);
        buttonMainMenu.onClick.AddListener (ButtonMainMenu);
        buttonContinue.onClick.AddListener (ButtonContinue);
        buttonRestart.onClick.AddListener (ButtonRestart);
        Init ();
    }
    private void Init () {
        if (SoundController.Instance == null) {
            return;
        }
        AnimateMusic ();
        AnimateSound ();
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
        if (SoundController.Instance == null) {
            Debug.Log ("WTFFFF");
            return;
        }
        StartCoroutine (DelayUseOption (AnimateSound));

    }
    private void ButtonMusic () {
        Delegate?.OnMusicInteract ();
        if (SoundController.Instance == null) {
            return;
        }
        Debug.Log ($"{SoundController.Instance.UseAudio}");
        StartCoroutine (DelayUseOption (AnimateMusic));
    }
    private void AnimateMusic () {
        var image = buttonMusic.GetComponent<Image> ();
        var imageChild = buttonMusic.GetComponentsInChildren<Image> ();
        if (LeanTween.isTweening (image.gameObject)) {
            LeanTween.cancel (image.gameObject);
        }
        float currentAlpha = image.color.a;
        float destinateAlpha = SoundController.Instance.UseAudio?1: .1f;
        Debug.Log ($"{SoundController.Instance.UseAudio}");
        LeanTween.value (image.gameObject, currentAlpha, destinateAlpha, .15f).setOnUpdate ((float value) => {
            image.color = new Color (image.color.r, image.color.g, image.color.b, value);
            foreach (var child in imageChild) {
                child.color = new Color (child.color.r, child.color.g, child.color.b, value);
            }

        }).setIgnoreTimeScale (true);
    }
    public void AnimateSound () {
        var image = buttonSound.GetComponent<Image> ();
        var imageChild = buttonSound.GetComponentsInChildren<Image> ();
        if (LeanTween.isTweening (image.gameObject)) {
            LeanTween.cancel (image.gameObject);
        }
        float currentAlpha = image.color.a;
        float destinateAlpha = SoundController.Instance.UseSoundFX?1: .1f;
        LeanTween.value (image.gameObject, currentAlpha, destinateAlpha, .15f).setOnUpdate ((float value) => {
            image.color = new Color (image.color.r, image.color.g, image.color.b, value);
            foreach (var child in imageChild) {
                child.color = new Color (child.color.r, child.color.g, child.color.b, value);
            }
        }).setIgnoreTimeScale (true);
    }
    private IEnumerator DelayUseOption (System.Action callback) {

        yield return new WaitForEndOfFrame ();
        yield return new WaitForEndOfFrame ();
        yield return new WaitForEndOfFrame ();
        yield return new WaitForEndOfFrame ();
        yield return new WaitForEndOfFrame ();

        callback ();

    }
    protected override LTDescr AnimateShow (System.Action callback) {
        return canvasGroup.LeanAlpha (1, 1f).setOnComplete (callback).setIgnoreTimeScale (true);
    }
    protected override LTDescr AnimateHide (System.Action callback) {
        return canvasGroup.LeanAlpha (0, 1f).setOnComplete (callback).setIgnoreTimeScale (true);
    }
}

public interface IPausePanelDelegate {
    void OnBackToMenu ();
    void OnContinues ();
    void OnRestart ();
    void OnSoundInteract ();
    void OnMusicInteract ();
}