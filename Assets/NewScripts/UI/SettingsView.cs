using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingsView : UiView {
    public Button buttonMusic;
    public Button buttonSoundFX;
    public Button buttonVibrate;

    private void Start () {
        buttonMusic.onClick.AddListener (ButtonMusic);
        buttonSoundFX.onClick.AddListener (ButtonSoundFX);
        buttonVibrate.onClick.AddListener (ButtonVibrate);
    }
    private void ButtonMusic () {
        SoundController.Instance.UseAudio = !SoundController.Instance.UseAudio;
        if (SoundController.Instance.UseAudio) {
            AnimateOn (buttonMusic);
        } else {
            AnimateOff (buttonMusic);
        }
    }
    private void ButtonSoundFX () {
        SoundController.Instance.UseSoundFX = !SoundController.Instance.UseSoundFX;
        if (SoundController.Instance.UseSoundFX) {
            AnimateOn (buttonSoundFX);
        } else {
            AnimateOff (buttonSoundFX);
        }
    }
    private void ButtonVibrate () {

    }

    private void AnimateOn (Button button) {
        if (LeanTween.isTweening (button.gameObject)) {
            LeanTween.cancel (button.gameObject);
        }
        var color = new Color32 (26, 188, 156, 255);
        var image = button.GetComponent<Image> ();
        var text = button.GetComponentInChildren<Text> ();
        var currentColor = image.color;
        LeanTween.value (button.gameObject, currentColor, color, .3f).setOnUpdate ((Color value) => {
            image.color = value;
        }).setIgnoreTimeScale (true);
        LeanTween.value (text.gameObject, 1, 0, .15f).setOnUpdate ((float value) => {
            text.color = new Color (text.color.r, text.color.g, text.color.b, value);
        }).setIgnoreTimeScale (true).setOnComplete (() => {
            text.text = "ON";
        });
        LeanTween.value (text.gameObject, 0, 1, .15f).setOnUpdate ((float value) => {
            text.color = new Color (text.color.r, text.color.g, text.color.b, value);
        }).setIgnoreTimeScale (true).setDelay (.15f);
    }
    private void AnimateOff (Button button) {
        if (LeanTween.isTweening (button.gameObject)) {
            LeanTween.cancel (button.gameObject);
        }
        var color = new Color32 (211, 84, 0, 255);
        var image = button.GetComponent<Image> ();
        var text = button.GetComponentInChildren<Text> ();
        var currentColor = image.color;
        LeanTween.value (button.gameObject, currentColor, color, .3f).setOnUpdate ((Color value) => {
            image.color = value;
        }).setIgnoreTimeScale (true);
        LeanTween.value (text.gameObject, 1, 0, .15f).setOnUpdate ((float value) => {
            text.color = new Color (text.color.r, text.color.g, text.color.b, value);
        }).setIgnoreTimeScale (true).setOnComplete (() => {
            text.text = "OFF";
        });
        LeanTween.value (text.gameObject, 0, 1, .15f).setOnUpdate ((float value) => {
            text.color = new Color (text.color.r, text.color.g, text.color.b, value);
        }).setIgnoreTimeScale (true).setDelay (.15f);
    }
}