using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Plane : MonoBehaviour {

    public State currentState;
    // public Sprite initSprite;
    // public Sprite hasWayPointSprite;
    public Color initColor;
    public LayerMask mask;
    public SpriteRenderer item;
    public bool hasItem;
    public SpriteRenderer warner;
    public WaterDrop waterDrop;
    private float warnDistance = 11;
    private SpriteRenderer planeSprite;
    private Line line;
    private bool isCollided;
    public float flashTime = 0.2f;
    private bool warnedSound;
    private void Start () {
        line = GetComponent<Line> ();
        warner.gameObject.SetActive (false);
        isCollided = false;
        Init ();
    }
    private void Update () {
        if (isCollided) {
            if (flashTime <= 0) {
                warner.enabled = !warner.enabled;
                planeSprite.enabled = !planeSprite.enabled;
                if (hasItem) {
                    item.enabled = !item.enabled;
                }
                flashTime = .2f;
            } else {
                flashTime -= Time.unscaledDeltaTime;
            }
        }
        if (hasItem) {
            item.transform.Rotate (0, 0, 2 * 360 * Time.deltaTime, Space.World);
        }
        line.FollowPath ();
        if (!line.GetIsLanding ())
            CheckCloseOtherPlane ();
    }
    private void CheckCloseOtherPlane () {
        if (line.GetIsLanding ()) {
            warner.gameObject.SetActive (false);
            return;
        }
        Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, warnDistance, mask);
        //Debug.Log ("other plane: " + cols.Length);
        if (cols.Length > 1) {
            warner.gameObject.SetActive (true);
            warner.transform.Rotate (0, 0, 360 * Time.deltaTime);
            if (!warnedSound) {
                warnedSound = true;
                InGameSoundManager.Instance?.PlayWarningSound ();
            }
        } else {
            warner.gameObject.SetActive (false);
            warnedSound = false;
        }
    }

    public virtual void Init () {
        //initColor = GameControl.Instance.GetColor (line.planeTag);
        planeSprite = GetComponent<SpriteRenderer> ();
        SwitchToInit ();
    }

    public virtual void SwitchToInit () {
        currentState = State.Init;
        //planeSprite.sprite = initSprite;
        planeSprite.color = initColor;
        if (hasItem) {
            item.color = initColor;
        }
    }
    public virtual void SwitchToHasWayPoint () {
        currentState = State.HasWayPoint;
        //planeSprite.sprite = hasWayPointSprite;
        planeSprite.color = new Color (1, 1, 1, 1);
        if (hasItem) {
            item.color = new Color (1, 1, 1, 1);
        }
    }
    private void OnCollisionEnter2D (Collision2D other) {
        if (other.gameObject.tag == "barrier") {
            if (!line.isDisable) {
                Vector2 reflectDirect = Vector2.Reflect (transform.up, other.GetContact (0).normal);
                Debug.Log ("reflect direct: " + reflectDirect);
                line.SetLookDirect (reflectDirect.normalized);
            } else {
                Vector2 reflectDirect = Vector2.Reflect (line.GetFreeLookDirect (), other.GetContact (0).normal);
                Debug.Log ("reflect direct: " + reflectDirect);
                line.SetLookDirect (reflectDirect.normalized);
            }
        }
        if (other.gameObject.tag == "plane" || other.gameObject.tag == "obstacle" || other.gameObject.tag == "enemy" && this.tag != "enemy") {
            if (!GameControl.Instance.isJustContinue) {
                Debug.Log ("Gameover motherfather");
                TriggerGameOver ();
            } else {
                Debug.Log ("blow effect required");
                InGameSoundManager.Instance?.PlayCrashSound ();
                SpawnManager.Instance.SpawnBlowEffect (transform.position);
                Destroy (gameObject);
            }
        }
    }
    private void OnCollisionStay2D (Collision2D other) {
        if (other.gameObject.tag == "plane" || other.gameObject.tag == "obstacle" || other.gameObject.tag == "enemy" && this.tag != "enemy") {
            if (!GameControl.Instance.isJustContinue) {
                Debug.Log ("Gameover motherfather");
                TriggerGameOver ();
            } else {
                Debug.Log ("blow effect required");
                InGameSoundManager.Instance?.PlayCrashSound ();
                SpawnManager.Instance.SpawnBlowEffect (transform.position);
                Destroy (gameObject);
            }
        }
    }
    public void TriggerGameOver () {
        isCollided = true;
        Time.timeScale = 0;
        InGameSoundManager.Instance?.PlayCrashSound ();
        GameControl.Instance.OnGameOver ();
        StartCoroutine (DestroyAfter (3f));
    }
    private void OnTriggerEnter2D (Collider2D other) {
        if (other.tag == "forestfire") {
            waterDrop.DropWater (other.GetComponent<ForestFire> ());
        }
    }
    private void OnTriggerStay2D (Collider2D other) {
        if (other.tag == "hurricane" && !line.GetIsLanding ()) {
            line.isDisable = true;
            SwitchToInit ();
            Vector2 reflectDirect = transform.position - other.transform.position;
            line.SetLookDirect (reflectDirect.normalized);
            line.ClearAll ();
        }
        if (other.tag == "cloud") {
            Debug.Log ("SLOWW");
            line.dynamicMoveSpeed = line.moveSpeed / 2;
        }
    }
    private void OnTriggerExit2D (Collider2D other) {
        if (other.tag == "barrier") {
            if (GetComponent<CircleCollider2D> ().isTrigger)
                GetComponent<CircleCollider2D> ().isTrigger = false;
        }
        if (other.tag == "cloud") {
            line.dynamicMoveSpeed = line.moveSpeed;
        }
    }
    private IEnumerator DestroyAfter (float time) {
        yield return new WaitForSecondsRealtime (time - 1);
        SpawnManager.Instance.SpawnBlowEffect (transform.position);
        yield return new WaitForSecondsRealtime (1);
        Destroy (gameObject);
    }

    public enum State { Init, HasWayPoint }
}