using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UiManager : MonoBehaviour {
    public IUiManagerDelegate Delegate { get; set; }
    public UiView CurrentView { get; set; }
    public GamePanelView viewGamePanel;
    public PausePanelView viewPausePanel;
    public GameOverPanelView viewGameOverPanel;
    public TutorPanelView viewTutorPanel;
    public Image imageFader;
    private void Start () { }
    public void ChangePanel (UiView nextPanel) {
        if (CurrentView == null) {
            CurrentView = nextPanel;
            CurrentView.Show ();
            return;
        }
        CurrentView.Hide (() => {
            CurrentView = nextPanel;
            CurrentView.Show ();
        });

    }
    public void ChangePanel (UiView nextPanel, System.Action callback) {
        if (CurrentView == null) {
            CurrentView = nextPanel;
            CurrentView.Show ();
            return;
        }
        CurrentView.Hide (() => {
            CurrentView = nextPanel;
            CurrentView.Show ();
            callback ();
        });
    }
    public void OpenPanel (UiView panel, System.Action callback) {
        panel.Show (callback);
    }
    public void ClosePanel (UiView panel, System.Action callback) {
        panel.Hide (callback);
    }
    public void OpenPanel (UiView panel) {
        panel.Show (() => { });
    }
    public void ClosePanel (UiView panel) {
        panel.Hide (() => { });
    }
    public void ShowFader (Action callback = null) {
        LeanTween.value (imageFader.gameObject, 0, 1, 1f).setOnUpdate ((float value) => {
            imageFader.color = new Color (imageFader.color.r, imageFader.color.g, imageFader.color.b, value);
        }).setIgnoreTimeScale (true).setOnComplete (() => {
            if (callback != null) {
                callback ();
            }
        });
    }
    public void HideFader (Action callback = null) {
        LeanTween.value (imageFader.gameObject, 1, 0, 1f).setOnUpdate ((float value) => {
            imageFader.color = new Color (imageFader.color.r, imageFader.color.g, imageFader.color.b, value);
        }).setIgnoreTimeScale (true).setOnComplete (() => {
            if (callback != null) {
                callback ();
            }
        });
    }
}
public interface IUiManagerDelegate {

}