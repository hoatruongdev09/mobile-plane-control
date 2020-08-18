using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
public class TutorPanelView : UiView {
    public Button.ButtonClickedEvent OnOkClickedEvent { get { return buttonOK.onClick; } set { buttonOK.onClick = value; } }
    public VideoPlayer videoPlayer;
    public RawImage videoRenderTarget;
    public Button buttonOK;
    public Text textTutorial;
    public string VideoPath { get; set; }
    public string TextTutorial { get { return textTutorial.text; } set { textTutorial.text = value; } }
    private void Start () {
        InitializeVideoPlayer ();
        buttonOK.onClick.AddListener (ButtonOK);
    }
    private void InitializeVideoPlayer () {
        videoPlayer.loopPointReached += OnVideoPlayerFinished;
        videoPlayer.started += OnVideoStartPlayed;
    }
    public override void Show () {
        gameObject.SetActive (true);
        AnimateShow (() => {
            LoadVideoTutorial (VideoPath);
        });
    }
    public override void Hide () {
        AnimateHide (() => {
            videoPlayer.Stop ();
            gameObject.SetActive (false);
        });
    }
    private void ButtonOK () {
        Hide ();
    }
    public void LoadVideoTutorial (string videoPath) {
        var clip = LoadVideo (videoPath);
        ShowClip (clip);
    }
    private void ShowClip (VideoClip clip) {
        StartCoroutine (PlayTutorialCoroutine (clip));
    }
    public void SetTextTutorial (string text) {
        textTutorial.text = text;
    }
    private IEnumerator PlayTutorialCoroutine (VideoClip clip) {
        PrepareVideoPlayer (clip);
        yield return new WaitUntil (() => videoPlayer.isPrepared);
        videoRenderTarget.texture = videoPlayer.texture;
        videoRenderTarget.color = new Color (1, 1, 1, 1);
        videoPlayer.isLooping = true;
        videoPlayer.Play ();
    }

    private VideoClip LoadVideo (string videoPath) {
        Debug.Log ($"video path: {videoPath}");
        VideoClip clip = Resources.Load<VideoClip> ($"tutorial-video/{videoPath}");
        Debug.Log ($"video null: {clip == null}");
        return clip;
    }
    private void PrepareVideoPlayer (VideoClip video) {
        videoPlayer.clip = video;
        videoPlayer.waitForFirstFrame = true;
        videoPlayer.Prepare ();
    }
    private void OnVideoStartPlayed (VideoPlayer source) {
        Debug.Log ("start played");
    }

    private void OnVideoPlayerFinished (VideoPlayer source) {
        Debug.Log ("finish played");
    }
}