using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class TutorialView : UiView {
    public RawImage videoRenderTarget;
    public VideoPlayer videoPlayer;
    public VideoClip[] tutorials;
    public string[] textTutorials;
    public Text textTutorial;
    public Button buttonPreviousTutorial;
    public Button buttonNextTutorial;
    [SerializeField] private int currentTutorial = 0;

    private void Start () {
        InitializeVideoPlayer ();
        buttonPreviousTutorial.onClick.AddListener (ButtonPreviousTutorial);
        buttonNextTutorial.onClick.AddListener (ButtonNextTutorial);
    }
    private void InitializeVideoPlayer () {
        videoPlayer.loopPointReached += OnVideoPlayerFinished;
        videoPlayer.started += OnVideoStartPlayed;
    }
    private void ButtonPreviousTutorial () {
        ChangeTutorial (-1);
    }
    private void ButtonNextTutorial () {
        ChangeTutorial (1);
    }
    private void ChangeTutorial (int direct) {
        currentTutorial += direct;
        if (currentTutorial < 0) {
            currentTutorial = textTutorials.Length - 1;
        }
        if (currentTutorial >= textTutorials.Length) {
            currentTutorial = 0;
        }
        ShowTutorial (currentTutorial);
    }
    public void ShowTutorial (int index) {
        buttonNextTutorial.gameObject.SetActive (true);
        buttonPreviousTutorial.gameObject.SetActive (true);
        PlayTutorial (tutorials[index]);
        AnimateChangeTutorial ();
    }
    public void PlayTutorial (VideoClip video) {
        videoPlayer.Stop ();
        StartCoroutine (PlayTutorialCoroutine (video));
    }
    private void AnimateChangeTutorial () {
        LeanTween.alphaText (textTutorial.rectTransform, 0, .15f);
        textTutorial.text = textTutorials[currentTutorial];
        LeanTween.alphaText (textTutorial.rectTransform, 1, .15f).setDelay (.15f);
    }
    private IEnumerator PlayTutorialCoroutine (VideoClip video) {
        PrepareVideoPlayer (video);
        yield return new WaitUntil (() => videoPlayer.isPrepared);
        videoRenderTarget.texture = videoPlayer.texture;
        videoRenderTarget.color = new Color (1, 1, 1, 1);
        videoPlayer.isLooping = true;
        videoPlayer.Play ();

    }
    private void OnVideoStartPlayed (VideoPlayer source) {
        Debug.Log ("start played");
    }

    private void OnVideoPlayerFinished (VideoPlayer source) {
        Debug.Log ("finish played");
    }
    private void PrepareVideoPlayer (VideoClip video) {
        videoPlayer.clip = video;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.Prepare ();
    }
    public override void Show () {
        gameObject.SetActive (true);
        AnimateShow (() => {
            ChangeTutorial (0);
        });
    }
    public override void Hide () {
        videoPlayer.Stop ();
        AnimateHide (() => {
            gameObject.SetActive (false);
        });
    }
}