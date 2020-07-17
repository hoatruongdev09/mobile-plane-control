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
        CurrentView.Hide (() => {
            CurrentView = nextPanel;
            CurrentView.Show ();
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
}
public interface IUiManagerDelegate {

}