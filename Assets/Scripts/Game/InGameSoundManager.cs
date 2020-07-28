using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSoundManager : MonoBehaviour {
    public static InGameSoundManager Instance { get; private set; }

    [Header ("SOUND EFFECT")]
    public AudioClip ac_crashSound;
    public AudioClip ac_planeSelectSound;
    public AudioClip ac_planeDeselectSound;
    public AudioClip ac_landedSound;
    public AudioClip ac_buttonSound;
    public AudioClip ac_warningSound;
    [Header ("MUSIC")]
    public AudioClip[] ac_Musics;

    private AudioSource audioSource;
    [SerializeField]
    private bool playSound;
    [SerializeField]
    private bool playMusic;
    private bool delayCrashSound;
    private bool delayWarningSound;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake () {

    }
    private void Start () {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy (gameObject);
        }
        audioSource = GetComponent<AudioSource> ();
        playSound = PlayerPrefs.GetInt ("sound", 1) == 1 ? true : false;
        playMusic = PlayerPrefs.GetInt ("music", 1) == 1 ? true : false;
        PlayMusic (ac_Musics[0]);
    }
    public void PlayMusic (AudioClip clip) {
        if (!playMusic) { return; }
        audioSource.clip = clip;
        audioSource.Play ();
        audioSource.loop = true;
    }
    public void PlayASound (AudioClip clip) {
        if (playSound)
            audioSource.PlayOneShot (clip);
    }
    public void PlayWarningSound () {
        if (playSound) {
            if (!delayWarningSound) {
                audioSource.PlayOneShot (ac_warningSound);
                StartCoroutine (DelayWarningSound (0.001f));
            }
        }
    }
    public void PlayCrashSound () {
        if (playSound) {
            if (!delayCrashSound) {
                audioSource.PlayOneShot (ac_crashSound);
                StartCoroutine (DelayCrashSound (0.001f));
            }
        }
    }
    public bool GetPlaySound () {
        return playSound;
    }
    public bool GetPlayMusic () {
        return playMusic;
    }

    public void SetPlaySound (bool active) {
        PlayerPrefs.SetInt ("sound", active ? 1 : 0);
        playSound = active;
        Debug.Log ("playsound: " + active);
    }
    public void SetPlayMusic (bool active) {
        PlayerPrefs.SetInt ("music", active ? 1 : 0);
        playMusic = active;
        Debug.Log ("playmusic: " + active);
        CheckMusicStatus ();
    }
    private IEnumerator DelayWarningSound (float time) {
        delayWarningSound = true;
        yield return new WaitForSecondsRealtime (time);
        delayWarningSound = false;
    }
    private IEnumerator DelayCrashSound (float time) {
        delayCrashSound = true;
        yield return new WaitForSecondsRealtime (time);
        delayCrashSound = false;
    }
    private void CheckMusicStatus () {
        if (playMusic) {
            if (!audioSource.isPlaying) {
                audioSource.PlayOneShot (ac_Musics[Random.Range (0, ac_Musics.Length)]);
            }
        } else {
            if (audioSource.isPlaying) {
                audioSource.Stop ();
            }

        }
        Debug.Log (GlobalShadow.Instance.shadowColor);
    }
}