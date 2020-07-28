using UnityEngine;

public class GameState : State {

    protected GameStateManager stateManager;
    public GameState (GameStateManager stateManager) {
        this.stateManager = stateManager;
    }

    public virtual void Enter () {

    }

    public virtual void Enter (object options) {
        Enter ();
    }

    public virtual void Exit () {

    }

    public virtual void Exit (object options) {
        Exit ();
    }

    public virtual void Reset () {

    }

    public virtual void Update () {

    }
}