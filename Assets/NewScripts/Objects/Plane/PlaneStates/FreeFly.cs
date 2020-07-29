using UnityEngine;

public class FreeFly : PlaneState, IPlaneBehavior, ITriggerCheckerDelegate, ICollisionCheckerDelegate {
    private Transform transform;
    private PlaneControl controller;
    private float freeFlyAngle;
    private Vector3 freeFlyDirect;
    private bool enterWithEffect;
    public FreeFly (PlaneStateManager stateManager) : base (stateManager) {
        controller = stateManager.Controller;
        transform = stateManager.Controller.transform;
    }
    public override void Enter (object options) {
        enterWithEffect = (bool) options.GetType ().GetProperty ("effect").GetValue (options);
        controller.IsStun = (bool) options.GetType ().GetProperty ("stun").GetValue (options);
        freeFlyDirect = ((Vector3) options.GetType ().GetProperty ("direct").GetValue (options)).normalized;
        // Debug.Log ($"freeFlyDirect: {freeFlyDirect}");
        Enter ();
    }
    public override void Enter () {
        controller.PlaneBehaviorDelegate = this;
        controller.TriggerCheckerDelegate = this;
        controller.CollisionCheckerDelegate = this;
        controller.Path.DeactivateEndPoint (true);
        controller.Path.Clear ();
        if (!enterWithEffect) {
            freeFlyAngle = transform.rotation.eulerAngles.z;
            freeFlyDirect = transform.up;
            // Debug.Log ($"init free: {freeFlyAngle} | {freeFlyDirect}");
        }
        if (controller.IsStun) {
            controller.IsReadyToLand = false;
        }
    }
    public override void Exit () {
        enterWithEffect = false;
        controller.IsStun = false;
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
        // transform.Translate (freeFlyDirect * StateManager.Controller.MoveSpeed * Time.smoothDeltaTime);
        transform.position += freeFlyDirect * StateManager.Controller.MoveSpeed * Time.smoothDeltaTime;
        if (controller.IsStun) {
            transform.Rotate (0, 0, controller.DisableRotateSpeed * Time.smoothDeltaTime, Space.Self);
        } else {
            transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 0, freeFlyAngle), Time.smoothDeltaTime);
        }
    }
    private void StunEffect () {
        if (!controller.IsStun) { return; }
        freeFlyAngle += 50 * Time.smoothDeltaTime;
    }
    private float CalculateFlyRotation (Vector2 direct) {
        return Mathf.Atan2 (direct.y, direct.x) * Mathf.Rad2Deg - 90;
    }
    private void ReflectDirection (Vector2 normal) {
        Vector2 reflectVector = Vector2.Reflect (freeFlyDirect, normal);
        Debug.Log ($"calculate reflect: {transform.up} {normal} {reflectVector}");
        freeFlyAngle = CalculateFlyRotation (reflectVector);
        freeFlyDirect = reflectVector.normalized;
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