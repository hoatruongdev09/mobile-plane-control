public class PlaneState : State {
    public PlaneStateManager StateManager { get; private set; }
    public PlaneState (PlaneStateManager stateManager) {
        StateManager = stateManager;
    }
    public virtual void Enter () {

    }

    public virtual void Enter (object options) {
        Enter ();
    }

    public virtual void Update () {

    }

    public virtual void Exit () {

    }

    public virtual void Exit (object options) {
        Exit ();
    }

    public virtual void Reset () {

    }

}