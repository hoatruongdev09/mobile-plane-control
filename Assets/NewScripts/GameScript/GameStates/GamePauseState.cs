using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;

public class GamePauseState : GameState, IPausePanelDelegate {
    private bool isPause = false;
    private bool isAnimatePause = false;
    private UiManager uiManager;
    public GamePauseState (GameStateManager stateManager) : base (stateManager) {
        uiManager = stateManager.GameController.uiManager;
    }

    public override void Enter () {
        stateManager.GameController.uiManager.viewPausePanel.Delegate = this;
        AnimatePause ();
    }

    public void OnBackToMenu () {
        uiManager.ShowFader (() => {
            GameController.Instance?.BackToMenu ();
        });
    }

    public void OnContinues () {
        uiManager.ClosePanel (uiManager.viewPausePanel, () => { });
        AnimateUnpause ();
    }
    public void OnMusicInteract () { }

    public void OnPauseClick () {

    }
    public void OnRestart () {
        uiManager.ShowFader (() => {
            GameController.Instance?.RestartGame ();
        });
    }

    public void OnSoundInteract () { }

    private void AnimatePause () {
        if (isAnimatePause) { return; }
        isAnimatePause = true;
        LeanTween.value (stateManager.GameController.gameObject, 1, 0, .5f).setOnUpdate ((float value) => {
            Time.timeScale = value;
        }).setOnComplete (() => {
            isPause = true;
            isAnimatePause = false;
        }).setIgnoreTimeScale (true);
        uiManager.OpenPanel (uiManager.viewPausePanel);
    }
    private void AnimateUnpause () {
        if (isAnimatePause) { return; }
        isAnimatePause = true;
        LeanTween.value (stateManager.GameController.gameObject, 0, 1, 1f).setOnUpdate ((float value) => {
            Time.timeScale = value;
        }).setOnComplete (() => {
            isPause = false;
            isAnimatePause = false;
            stateManager.StateMachine.ChangeState (stateManager.StartedState);
        }).setIgnoreTimeScale (true);
    }
}