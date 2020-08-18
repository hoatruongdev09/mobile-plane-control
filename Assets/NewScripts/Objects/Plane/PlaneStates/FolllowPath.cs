using System;
using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;

public class FolllowPath : PlaneState {
    protected NewScript.Path currentPath;
    protected Transform transform;
    protected PlaneControl controller;
    protected Vector3 nextPosition;
    protected Vector3 moveDirect;
    public FolllowPath (PlaneStateManager stateManager) : base (stateManager) {
        transform = stateManager.Controller.transform;
        controller = stateManager.Controller;
    }
    public override void Enter () {
        currentPath = StateManager.Controller.Path;
    }

    public override void Exit () {

    }
    public override void Update () {
        this.FollowPath ();
    }
    protected virtual void CheckStartLanding () {
        if (currentPath.CurrentPointCount <= 5) {
            StateManager.Machine.ChangeState (StateManager.StateLanding);
        }
    }

    protected virtual void FollowPath () {
        currentPath.SetFirstIndexPosition (controller.planeAnchor.transform.position);
        CalculateDirect ();
        UpdateTransform ();
        if (controller.IsReadyToLand) {
            CheckStartLanding ();
        }
    }
    protected virtual void CalculateDirect () {
        try {
            nextPosition = currentPath.GetNextPoint (0);
        } catch (Exception e) {
            // Debug.Log ($"{e.Message}");
            if (!controller.IsSelected) {
                StateManager.Machine.ChangeState (StateManager.StateFreeFly);
            }
        }
        moveDirect = nextPosition - transform.position;
        if (moveDirect.sqrMagnitude <= Mathf.Pow (StateManager.Controller.MeetDistance, 2)) {
            RemoveTrace ();
        }
    }
    protected virtual void UpdateTransform () {
        transform.position = Vector3.MoveTowards (transform.position, nextPosition, StateManager.Controller.MoveSpeed * Time.smoothDeltaTime);
        float angle = CalculateRotation (moveDirect / 2);
        transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 0, angle), (StateManager.Controller.TurnSpeed + Mathf.Abs (angle / 60)) * Time.smoothDeltaTime);
    }
    protected virtual float CalculateRotation (Vector2 direct) {
        return Mathf.Atan2 (direct.y, direct.x) * Mathf.Rad2Deg - 90;
    }
    protected virtual void RemoveTrace () {
        for (int i = 1; i < currentPath.CurrentPointCount; i++) {
            try {
                currentPath.SetPosition (i, currentPath.GetNextPoint (i));
            } catch (Exception e) {
                // Debug.Log ($"index {i} out of bound, detail: {e.Message}");
            }
        }
        currentPath.CurrentPointCount--;
    }
}