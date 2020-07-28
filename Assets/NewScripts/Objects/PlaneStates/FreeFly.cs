using UnityEngine;

public class FreeFly : PlaneState, IPlaneBehavior, ITriggerCheckerDelegate, ICollisionCheckerDelegate {
    private Transform transform;
    private PlaneControl controller;
    private float freeFlyAngle;
    public FreeFly (PlaneStateManager stateManager) : base (stateManager) {
        controller = stateManager.Controller;
        transform = stateManager.Controller.transform;
    }

    public override void Enter () {
        controller.PlaneBehaviorDelegate = this;
        controller.TriggerCheckerDelegate = this;
        controller.CollisionCheckerDelegate = this;
        controller.Path.ActiveEndPoint (true);
        freeFlyAngle = transform.rotation.eulerAngles.z;
        Debug.Log ($"init free fly angle: {freeFlyAngle}");
    }
    public override void Exit () {
        controller.PlaneBehaviorDelegate = null;
        controller.TriggerCheckerDelegate = null;
        controller.CollisionCheckerDelegate = null;
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
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 0, freeFlyAngle), 2 * Time.smoothDeltaTime);
    }
    private float CalculateFlyRotation (Vector2 direct) {
        return Mathf.Atan2 (direct.y, direct.x) * Mathf.Rad2Deg - 90;
    }
    private void ReflectDirection (Vector2 normal) {
        Vector2 reflectVector = Vector2.Reflect (transform.up, normal);
        Debug.Log ($"calculate reflect: {transform.up} {normal} {reflectVector}");
        freeFlyAngle = CalculateFlyRotation (reflectVector);
    }
    public void OnCheckerTriggerEnter2D (ColliderChecker checker, Collider2D other) {

    }

    public void OnCheckerTriggerExit2D (ColliderChecker checker, Collider2D other) {
        if (checker == controller.bodyCollider && other.tag == "mapBorder") {
            controller.IsEnterMap = false;
            Debug.Log ("plane enterer map");
        }
    }

    public void OnCheckerTriggerStay2D (ColliderChecker checker, Collider2D other) {

    }

    public void OnCheckerCollisionEnter2D (ColliderChecker checker, Collision2D other) {
        if (checker == controller.bodyCollider && other.gameObject.tag == "barrier") {
            if (controller.IsEnterMap) { return; }
            Debug.Log ($"wtf change direction  {controller.IsEnterMap}");
            ReflectDirection (other.contacts[0].normal);
        }
    }

    public void OnCheckerCollisionExit2D (ColliderChecker checker, Collision2D other) {

    }

    public void OnCheckerCollisionStay2D (ColliderChecker checker, Collision2D other) {

    }
}