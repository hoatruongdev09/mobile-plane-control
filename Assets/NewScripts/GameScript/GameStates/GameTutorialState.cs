using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameTutorialState : GameState {
    private UiManager uiManager;
    private object gamePlayData;
    private TutorialData tutorialData;
    public GameTutorialState (GameStateManager stateManager) : base (stateManager) {
        uiManager = stateManager.GameController.uiManager;
    }

    public override void Enter (object options) {
        gamePlayData = options.GetType ().GetProperty ("gamePlayData").GetValue (options);
        tutorialData = (TutorialData) options.GetType ().GetProperty ("tutorialData").GetValue (options);
        Enter ();
    }
    public override void Enter () {
        uiManager.viewTutorPanel.TextTutorial = tutorialData.textTutorial;
        uiManager.viewTutorPanel.VideoPath = tutorialData.videoPath;
        uiManager.viewTutorPanel.OnOkClickedEvent.AddListener (OnSkipTutorial);
        uiManager.ChangePanel (uiManager.viewTutorPanel);
    }

    private void OnSkipTutorial () {
        stateManager.StateMachine.ChangeState (stateManager.startedState, gamePlayData);
    }
}