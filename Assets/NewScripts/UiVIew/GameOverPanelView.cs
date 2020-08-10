using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverPanelView : UiView {
    public IGameOverPanelDelegate Delegate { get; set; }
    public Text textLandedPlanes;
    public Text textBestScore;
    public Button buttonWatchAdToContinues;
    public Button buttonAchievement;
    public Button buttonLeaderBoard;
    public Button buttonMainMenu;
    public Button buttonReset;

    private void Start () {
        buttonWatchAdToContinues.onClick.AddListener (ButtonWatchAd);
        buttonAchievement.onClick.AddListener (ButtonAchievement);
        buttonLeaderBoard.onClick.AddListener (ButtonLeaderboard);
        buttonMainMenu.onClick.AddListener (ButtonMainMenu);
        buttonReset.onClick.AddListener (ButtonReset);
    }
    public void ButtonWatchAd () {
        Delegate?.OnWatchAd ();
    }
    public void ButtonAchievement () {
        Delegate?.OnViewAchievement ();
    }
    public void ButtonLeaderboard () {
        Delegate?.OnViewLeaderboard ();
    }
    public void ButtonMainMenu () {
        Delegate?.OnBackToMenu ();
    }
    public void ButtonReset () {
        Delegate?.OnReset ();
    }
    protected override LTDescr AnimateShow (System.Action callback) {
        return canvasGroup.LeanAlpha (1, 1f).setOnComplete (callback).setIgnoreTimeScale (true);
    }
    protected override LTDescr AnimateHide (System.Action callback) {
        return canvasGroup.LeanAlpha (0, 1f).setOnComplete (callback).setIgnoreTimeScale (true);
    }

}
public interface IGameOverPanelDelegate {
    void OnWatchAd ();
    void OnViewAchievement ();
    void OnViewLeaderboard ();
    void OnBackToMenu ();
    void OnReset ();
}