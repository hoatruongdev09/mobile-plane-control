using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;

public class PlaneControl : MonoBehaviour, ITriggerCheckerDelegate, ICollisionCheckerDelegate {
    public IPlaneBehavior PlaneBehaviorDelegate { get; set; }
    public bool IsSelected {
        get { return isSelect; }
        set {
            OnSelect (value);
        }
    }
    public bool IsReadyToLand {
        get { return isReadyToLand; }
        set { isReadyToLand = value; }
    }
    public NewScript.Path Path { get { return path; } }

    [Header ("Properties")]
    public float MoveSpeed = 10;
    public float TurnSpeed = 1;
    public float MeetDistance = .1f;
    public float DisableRotateSpeed = 360;
    public string PlaneTag;
    public SpriteRenderer graphic;
    public Collider2D detector;
    public ColliderChecker bodyCollider;
    [SerializeField] private NewScript.Path path;
    private PlaneStateManager stateManager;
    private StateMachine stateMachine;
    private bool isReadyToLand = false;
    private bool isSelect = false;
    private void Start () {
        path.Controller = this;
        stateMachine = new StateMachine ();
        stateManager = new PlaneStateManager (this, stateMachine);
        stateMachine.Start (stateManager.StateFollowPath);
        bodyCollider.CollisionCheckerDelegate = this;
        bodyCollider.TriggerCheckerDelegate = this;
        bodyCollider.OwnedInfo = this;
    }
    private void Update () {
        stateMachine.CurrentState.Update ();
    }
    public void Select () {
        IsSelected = true;
    }
    public void Deselect () {
        IsSelected = false;
    }
    public void SetLanding (Airport airport) {
        if (airport.planeTag != PlaneTag) {
            return;
        }
        var landingPath = airport.GetLandindPoint ();
        foreach (Vector3 point in landingPath) {
            path.AddPoint (point);
        }
        IsReadyToLand = true;
    }
    public void SetLanding () {
        IsReadyToLand = true;
    }
    public void ChangeToFollowPath () {
        stateMachine.ChangeState (stateManager.StateFollowPath);
    }
    public void Delete () {
        Destroy (gameObject);
    }

    private void OnSelect (bool value) {
        isSelect = value;
        PlaneBehaviorDelegate?.OnSelect (isSelect);
        if (!IsReadyToLand) {
            path.ActiveEndPoint (isSelect);
        }
        if (isSelect) {
            IsReadyToLand = false;
        }
    }

    public void OnTriggerEnter2D (ColliderChecker checker, Collider2D other) {

    }

    public void OnTriggerStay2D (ColliderChecker checker, Collider2D other) {

    }

    public void OnTriggerExit2D (ColliderChecker checker, Collider2D other) {

    }

    public void OnCollisionEnter2D (ColliderChecker checker, Collision2D other) {
        if (checker == bodyCollider) {
            ColliderChecker otherChecker = other.gameObject.GetComponent<ColliderChecker> ();
            if (otherChecker && otherChecker.OwnedInfo.GetType () == typeof (PlaneControl)) {
                Debug.Log ("collided");
            }
        }
    }

    public void OnCollisionExit2D (ColliderChecker checker, Collision2D other) {

    }

    public void OnCollisionStay2D (ColliderChecker checker, Collision2D other) {

    }
}
public interface IPlaneBehavior {
    void OnSelect (bool action);
}