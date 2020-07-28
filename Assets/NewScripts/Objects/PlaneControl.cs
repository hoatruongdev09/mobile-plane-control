using System.Collections;
using System.Collections.Generic;
using NewScript;
using UnityEngine;

public class PlaneControl : MonoBehaviour, ITriggerCheckerDelegate, ICollisionCheckerDelegate {
    public IPlaneBehavior PlaneBehaviorDelegate { get; set; }
    public ITriggerCheckerDelegate TriggerCheckerDelegate { get; set; }
    public ICollisionCheckerDelegate CollisionCheckerDelegate { get; set; }
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
    [SerializeField] private NewScript.Path path;
    private PlaneStateManager stateManager;
    private StateMachine stateMachine;
    private bool isReadyToLand = false;
    private bool isSelect = false;
    private bool isEnterMap = true;
    private void Start () {
        path.Controller = this;
        stateMachine = new StateMachine ();
        stateManager = new PlaneStateManager (this, stateMachine);
        stateMachine.Start (stateManager.StateFreeFly);
        Init ();

    }
    private void Init () {

        bodyCollider.CollisionCheckerDelegate = this;
        bodyCollider.TriggerCheckerDelegate = this;
        bodyCollider.OwnedInfo = this;

        detectorChecker.CollisionCheckerDelegate = this;
        detectorChecker.TriggerCheckerDelegate = this;
        detectorChecker.OwnedInfo = this;
    }
    private void Update () {
        stateMachine.CurrentState.Update ();
    }
    public void SetColor (Color color) {
        baseColor = color;
        foreach (var graphic in graphics) {
            graphic.color = color;
        }
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
            path.ActiveEndPoint (isSelect);
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
        TriggerCheckerDelegate?.OnCheckerTriggerEnter2D (checker, other);
    }

    public void OnCheckerTriggerStay2D (ColliderChecker checker, Collider2D other) {
        if (checker == detectorChecker) {
            warningIndicate.transform.Rotate (0, 0, 360 * Time.unscaledDeltaTime);
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
                GameController.Instance?.OnPlaneCollide (this);
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