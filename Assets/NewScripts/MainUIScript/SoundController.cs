using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour {
    public static SoundController Instance { get; private set; }
    public OnOptionInteract onAudioChange { get; set; }
    public OnOptionInteract onSoundFXChange { get; set; }
    public bool UseAudio {
        get { return useAudio; }
        set {
            useAudio = value;
            onAudioChange?.Invoke (useAudio);
        }
    }
    public bool UseSoundFX {
        get { return useSoundFX; }
        set {
            useSoundFX = value;
            onSoundFXChange?.Invoke (useSoundFX);
        }
    }
    public AudioClip buttonPress;
    public AudioClip[] musics;
    public AudioClip planeSelect;
    public AudioClip planeLanded;
    public AudioClip planeWarning;
    public AudioClip planeCrash;
    public AudioSource musicChannel;
    public AudioSource sfxChannel;

    private bool useAudio;
    private bool useSoundFX;
    public delegate void OnOptionInteract (bool action);
    private bool delayPlayWarningSound = false;
    private bool delayCrashSound = false;
    private void Awake () {
        var soundControll = FindObjectOfType<SoundController> ();
        if (soundControll.gameObject != this.gameObject) {
            Destroy (this.gameObject);
        } else {
            DontDestroyOnLoad (this.gameObject);
        }
        if (Instance == null) {
            Instance = this;
        }
    }
    private void Start () {
        onAudioChange += OnAudioChange;
    }
    public void PlayMusic (int id, bool loop) {
        if (!useAudio) { return; }
        try {
            EasyInMusic ().setOnComplete (() => {
                musicChannel.clip = musics[id];
                musicChannel.loop = loop;
                musicChannel.Play ();
                EasyOutMusic ();
            });
        } catch (Exception e) {
            Debug.LogError (e);
        }
    }
    public void PlaySFX (AudioClip sfx) {
        if (!useSoundFX) { return; }
        sfxChannel.PlayOneShot (sfx);
    }
    public void ButtonPress () {
        PlaySFX (buttonPress);
    }
    public void AssignButtonSound () {
        Button[] buttons = Resources.FindObjectsOfTypeAll<Button> ();
        foreach (var button in buttons) {
            button.onClick.AddListener (ButtonPress);
        }

    }
    public void PlaneWarning () {
        if (delayPlayWarningSound) { return; }
        delayPlayWarningSound = true;
        PlaySFX (planeWarning);
        SetTimeOut (() => {
            delayPlayWarningSound = false;
        }, .01f, true);
    }
    public void PlaneCrash () {
        if (delayCrashSound) { return; }
        delayCrashSound = true;
        PlaySFX (planeCrash);
        SetTimeOut (() => {
            delayCrashSound = false;
        }, .01f, true);
    }
    private void OnAudioChange (bool action) {
        if (!action) {
            EasyInMusic ().setOnComplete (() => {
                musicChannel.Pause ();
            });
        } else {
            EasyOutMusic ().setOnComplete (() => {
                musicChannel.UnPause ();
            });
        }
    }
    private LTDescr EasyInMusic () {
        return LeanTween.value (gameObject, 1, 0, .4f).setOnUpdate ((float value) => {
            musicChannel.volume = value;
        }).setIgnoreTimeScale (true);
    }
    private LTDescr EasyOutMusic () {
        return LeanTween.value (gameObject, 0, 1, .4f).setOnUpdate ((float value) => {
            musicChannel.volume = value;
        }).setIgnoreTimeScale (true);
    }

    public void SetTimeOut (System.Action callback, float time, bool realTime) {
        StartCoroutine (TimeOut (callback, time, realTime));
    }
    private IEnumerator TimeOut (Action callback, float time, bool realTime) {
        if (realTime) {
            yield return new WaitForSecondsRealtime (time);
        } else {
            yield return new WaitForSeconds (time);
        }
        callback ();
    }
}