using UnityEngine;

public class FreeFly : PlaneState, IPlaneBehavior {
    private Transform transform;
    private PlaneControl controller;
    public FreeFly (PlaneStateManager stateManager) : base (stateManager) {
        controller = stateManager.Controller;
        transform = stateManager.Controller.transform;
    }

    public override void Enter () {
        controller.PlaneBehaviorDelegate = this;
        controller.Path.ActiveEndPoint (true);
    }
    public override void Exit () {
        controller.PlaneBehaviorDelegate = null;
    }

    public void OnLanding () {

    }

    public void OnSelect (bool action) {
        if (action) {
            StateManager.Machine.ChangeState (StateManager.StateFollowPath);
        }
    }

    public override void Update () {
        FlyForward ();
    }
    private void FlyForward () {
        transform.Translate (Vector2.up * StateManager.Controller.MoveSpeed * Time.smoothDeltaTime);
    }
}