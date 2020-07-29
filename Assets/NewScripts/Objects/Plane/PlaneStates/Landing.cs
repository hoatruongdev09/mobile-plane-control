using System;
using UnityEngine;

public class Landing : FolllowPath {
    private bool isHiding = false;
    public Landing (PlaneStateManager stateManager) : base (stateManager) {

    }
    public override void Enter () {
        base.Enter ();
        StartLanding ();
    }
    protected override void CheckStartLanding () {

    }
    protected override void CalculateDirect () {
        if (currentPath.CurrentPointCount <= 2) {
            AnimateHide ();
        }
        try {
            nextPosition = currentPath.GetNextPoint (0);
        } catch (Exception e) {

        }
        moveDirect = nextPosition - transform.position;
        if (moveDirect.sqrMagnitude <= Mathf.Pow (StateManager.Controller.MeetDistance, 2)) {
            RemoveTrace ();
        }
    }
    private void StartLanding () {
        controller.detector.enabled = false;
        controller.bodyCollider.gameObject.SetActive (false);
        AnimateLanding ();
    }
    private void AnimateHide () {
        if (isHiding) { return; }
        isHiding = true;
        LeanTween.value (transform.gameObject, 1, 0, 2f).setOnUpdate ((float value) => {
            foreach (var graphic in controller.graphics) {
                graphic.color = new Color (graphic.color.r, graphic.color.g, graphic.color.b, value);
            }
        }).setOnComplete (() => {
            this.controller.onPlaneLanded?.Invoke (this.controller);
        });
    }
    private void AnimateLanding () {
        transform.LeanScale (Vector3.one * 0.5f, 1f);
        var startSpeed = controller.MoveSpeed;
        var endSpeed = controller.MoveSpeed * .75f;
        LeanTween.value (transform.gameObject, startSpeed, endSpeed, 2f).setOnUpdate ((float value) => {
            controller.MoveSpeed = value;
        });
        Color lineColor = controller.Path.lineVisual.startColor;
        LeanTween.value (transform.gameObject, 1, 0, .5f).setOnUpdate ((float value) => {
            lineColor.a = value;
            controller.Path.lineVisual.endColor = controller.Path.lineVisual.startColor = lineColor;
        });
    }

}