using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainUiManager : MonoBehaviour, IMainTitlePanelDelegate {
    public MainTitlePanel mainTitlePanel;
    public Image imgFader;
    private void Start () {
        mainTitlePanel.Delegate = this;
    }
    public void ShowFader (Action callback) {
        imgFader.gameObject.SetActive (true);
        LeanTween.value (imgFader.gameObject, 0, 1, 1f).setOnUpdate ((float value) => {
            imgFader.color = new Color (imgFader.color.r, imgFader.color.g, imgFader.color.b, value);
        }).setOnComplete (() => {
            if (callback != null) { callback (); }
        }).setIgnoreTimeScale (true);;
    }
    public void HideFader (Action callback) {
        LeanTween.value (imgFader.gameObject, 1, 0, 1f).setOnUpdate ((float value) => {
            imgFader.color = new Color (imgFader.color.r, imgFader.color.g, imgFader.color.b, value);
        }).setOnComplete (() => {
            if (callback != null) { callback (); }
            imgFader.gameObject.SetActive (false);
        }).setIgnoreTimeScale (true);
    }

    public void OnChooseLevel (int id) {
        var level = DataManager.Instance.LevelData[id];
        PlayerSection.Instance.LastChooseLevelID = level.info.id;
    }

    public void OnPlayClick () {
        ShowFader (() => {
            MainGameManager.Instance.StartGame ();
        });
    }

    public void OnConfirmQuit () {
        ShowFader (() => {
            MainGameManager.Instance.Quit ();
        });
    }

    public void OnChooseLevel (string levelId) {
        var level = DataManager.Instance.GetLevelDataByID (levelId);
        PlayerSection.Instance.LastChooseLevelID = level.info.id;
    }
}