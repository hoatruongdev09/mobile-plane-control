using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;

public class PlaneControl : MonoBehaviour, ITriggerCheckerDelegate, ICollisionCheckerDelegate {
    public OnLanded onPlaneLanded { get; set; }
    public OnCollidedWithPlane onCollidedWithPlane { get; set; }
    public IPlaneBehavior PlaneBehaviorDelegate { get; set; }
    public ITriggerCheckerDelegate TriggerCheckerDelegate { get; set; }
    public ICollisionCheckerDelegate CollisionCheckerDelegate { get; set; }
    public delegate void OnLanded (PlaneControl plane);
    public delegate void OnCollidedWithPlane (PlaneControl plane);
    public bool IsSelected {
        get { return isSelect; }
        set {
            OnSelect (value);
        }
    }
    public bool IsReadyToLand {
        get { return isReadyToLand; }
        set {
            isReadyToLand = value;
            if (isReadyToLand) {
                OnReadyToLand ();
            } else {
                OnUnReadyToLand ();
            }
        }
    }
    public bool IsEnterMap {
        get { return isEnterMap; }
        set { isEnterMap = value; }
    }
    public bool IsStun { get; set; }
    public List<IPlaneComponent> Components {
        get { return components; }
        set { components = value; }
    }
    public NewScript.Path Path { get { return path; } }
    public enum PlaneType { helicopter, air_plane }

    [Header ("Properties")]
    public PlaneType planeType;
    public Color baseColor;
    public float MoveSpeed = 10;
    public float TurnSpeed = 1;
    public float MeetDistance = .1f;
    public float DisableRotateSpeed = 360;

    public string PlaneTag;
    public SpriteRenderer[] graphics;
    public SpriteRenderer warningIndicate;
    public Collider2D detector;
    public ColliderChecker detectorChecker;
    public ColliderChecker bodyCollider;
    public NewScript.Path path;
    private List<IPlaneComponent> components;
    private PlaneStateManager stateManager;
    private StateMachine stateMachine;
    private bool isReadyToLand = false;
    private bool isSelect = false;
    private bool isEnterMap = true;
    private void Start () {
        Init ();
        stateMachine = new StateMachine ();
        stateManager = new PlaneStateManager (this, stateMachine);
        stateMachine.Start (stateManager.StateFreeFly);

    }
    private void Init () {
        components = new List<IPlaneComponent> ();

        path.Controller = this;

        bodyCollider.CollisionCheckerDelegate = this;
        bodyCollider.TriggerCheckerDelegate = this;
        bodyCollider.OwnedInfo = this;

        detectorChecker.CollisionCheckerDelegate = this;
        detectorChecker.TriggerCheckerDelegate = this;
        detectorChecker.OwnedInfo = this;
    }
    private void Update () {
        stateMachine.CurrentState?.Update ();
    }
    public void SetColor (Color color) {
        baseColor = color;
        foreach (var graphic in graphics) {
            graphic.color = color;
        }
    }
    public void OnOutOfFuel () {
        if (stateMachine.CurrentState == stateManager.StateLanding) {
            return;
        }
        stateMachine.ChangeState (stateManager.StateCrashing);
    }
    private void OnReadyToLand () {
        foreach (var graphic in graphics) {
            var startColor = graphic.color;
            LeanTween.value (graphic.gameObject, startColor, Color.white, .3f).setIgnoreTimeScale (true);
        }
    }
    private void OnUnReadyToLand () {
        foreach (var graphic in graphics) {
            var startColor = graphic.color;
            LeanTween.value (graphic.gameObject, startColor, baseColor, .3f).setIgnoreTimeScale (true);
        }
    }
    public void HighlightCrash () {
        AnimateHighlightCrash ();
    }
    private LTDescr AnimateHighlightCrash () {
        return LeanTween.value (gameObject, 1, 0, .25f).setOnUpdate ((float value) => {
            foreach (var graphic in graphics) {
                graphic.color = new Color (graphic.color.r, graphic.color.g, graphic.color.b, value);
            }
        }).setIgnoreTimeScale (true).setLoopPingPong (5);
    }
    public void Select () {
        IsSelected = true;
    }
    public void Deselect () {
        IsSelected = false;
    }
    public void SetLanding (Airport airport) {
        if (airport.PlaneTag != PlaneTag) {
            return;
        }
        var landingPath = airport.GetLandingPoint ();
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
            path.DeactivateEndPoint (isSelect);
        }
        if (isSelect) {
            IsReadyToLand = false;
        }
    }
    private void ActiveWarningIndicator (bool action) {
        var targetAlpha = action ? 1 : 0;
        var startAlpha = warningIndicate.color.a;
        LeanTween.value (warningIndicate.gameObject, startAlpha, targetAlpha, .1f).setOnUpdate ((float value) => {
            warningIndicate.color = new Color (warningIndicate.color.r, warningIndicate.color.g, warningIndicate.color.b, value);
        });
    }
    public void OnCheckerTriggerEnter2D (ColliderChecker checker, Collider2D other) {
        if (checker == detectorChecker) {
            ActiveWarningIndicator (true);
        }
        if (checker == bodyCollider) {
            if (other.tag == "hurricane") {
                var reflectDirect = transform.position - other.transform.position;
                stateMachine.ChangeState (stateManager.StateFreeFly, new { effect = true, stun = true, direct = reflectDirect });
            }
        }
        TriggerCheckerDelegate?.OnCheckerTriggerEnter2D (checker, other);
    }

    public void OnCheckerTriggerStay2D (ColliderChecker checker, Collider2D other) {
        if (checker == detectorChecker) {
            warningIndicate.transform.Rotate (0, 0, 360 * Time.unscaledDeltaTime);
        }
        if (checker == bodyCollider) {
            if (other.tag == "hurricane") {
                var reflectDirect = transform.position - other.transform.position;
                stateMachine.ChangeState (stateManager.StateFreeFly, new { effect = true, stun = true, direct = reflectDirect });
            }
        }
        TriggerCheckerDelegate?.OnCheckerTriggerStay2D (checker, other);
    }

    public void OnCheckerTriggerExit2D (ColliderChecker checker, Collider2D other) {
        if (checker == detectorChecker) {
            ActiveWarningIndicator (false);
        }
        TriggerCheckerDelegate?.OnCheckerTriggerExit2D (checker, other);
    }

    public void OnCheckerCollisionEnter2D (ColliderChecker checker, Collision2D other) {
        // Debug.Log ($"collision: {checker.name}");
        if (checker == bodyCollider) {
            ColliderChecker otherChecker = other.gameObject.GetComponent<ColliderChecker> ();
            if (otherChecker && otherChecker.OwnedInfo.GetType () == typeof (PlaneControl)) {
                onCollidedWithPlane?.Invoke (this);
            }
        }
        CollisionCheckerDelegate?.OnCheckerCollisionEnter2D (checker, other);
    }

    public void OnCheckerCollisionExit2D (ColliderChecker checker, Collision2D other) {
        CollisionCheckerDelegate?.OnCheckerCollisionExit2D (checker, other);
    }

    public void OnCheckerCollisionStay2D (ColliderChecker checker, Collision2D other) {
        CollisionCheckerDelegate?.OnCheckerCollisionStay2D (checker, other);
    }

}
public interface IPlaneBehavior {
    void OnSelect (bool action);
}