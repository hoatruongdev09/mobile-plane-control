using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour {
    [Header ("Properties")]
    public float moveSpeed = 10;
    public float turnSpeed = 1;
    public float meetDistance = .1f;
    public float disableRotateSpeed = 360;
    public string planeTag;
    public int maxPointCount = 200;

    private GameObject endPoint;
    public Vector2 freeLookDirect;

    private LineRenderer lineRenderer;
    private int positionCount;
    private int currentIndex;
    private int finishLineIndex;
    [Header ("Status")]
    private bool isFollowPath;
    private bool readyToLanding;
    public bool canDraw = true;
    public bool isDisable;
    [SerializeField] private bool islanding;
    public float dynamicMoveSpeed;
    private Plane plane;

    private SpawnManager spawnManager;
    private Material defaultMaterial;
    private Material startMaterial;
    private void Start () {
        lineRenderer = GetComponent<LineRenderer> ();
        plane = GetComponent<Plane> ();
        dynamicMoveSpeed = moveSpeed;
        lineRenderer.positionCount = positionCount = 1;
        currentIndex = 1;
        finishLineIndex = 0;
        defaultMaterial = new Material (Shader.Find ("Sprites/Default"));
        startMaterial = lineRenderer.material;
        lineRenderer.startColor = lineRenderer.endColor = GameControl.Instance.defaultLineColor;
    }
    public void FollowPath () {
        if (isDisable) {
            readyToLanding = false;
            transform.Translate (freeLookDirect.normalized * dynamicMoveSpeed * Time.deltaTime, Space.World);
            transform.Rotate (0, 0, disableRotateSpeed * Time.deltaTime, Space.Self);
        } else {
            lineRenderer.SetPosition (0, transform.position);
            if (finishLineIndex != 0) {
                isFollowPath = currentIndex <= finishLineIndex;
                if (isFollowPath) {
                    freeLookDirect = (V2ToV3 (GetLookPosition (currentIndex)) - transform.position).normalized;
                    transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 0, CalculationAngle (freeLookDirect)), turnSpeed * Time.smoothDeltaTime);
                    transform.position = Vector3.MoveTowards (transform.position, lineRenderer.GetPosition (currentIndex), dynamicMoveSpeed * Time.smoothDeltaTime);
                    if (Vector2.Distance (transform.position, lineRenderer.GetPosition (currentIndex)) < meetDistance) {
                        currentIndex++;
                        RemoveTrace ();
                    }
                    if (currentIndex > finishLineIndex) {
                        Destroy (endPoint);
                    }
                }
            } else {
                transform.Translate (Vector2.up * dynamicMoveSpeed * Time.smoothDeltaTime);
                transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.Euler (0, 0, CalculationAngle (freeLookDirect)), turnSpeed * Time.smoothDeltaTime);
            }
        }
    }

    private void RemoveTrace () {
        for (int i = 1; i <= finishLineIndex - 1; i++) {
            lineRenderer.SetPosition (i, lineRenderer.GetPosition (i + 1));
        }
        currentIndex--;
        finishLineIndex--;
        lineRenderer.positionCount--;
        positionCount--;
    }
    private Vector2 GetLookPosition (int index) {
        return lineRenderer.GetPosition (index);
        // if (index + 2 <= finishLineIndex)
        //     return line.GetPosition (index + 2);
        // else if (index + 1 <= finishLineIndex)
        //     return line.GetPosition (index + 1);
        // else
        //     return line.GetPosition (index);
    }
    public Vector2 GetFreeLookDirect () {
        return freeLookDirect;
    }
    public void SetLookDirect (Vector2 direct) {
        freeLookDirect = direct;
    }
    public void SetSpawnManager (SpawnManager sp) {
        spawnManager = sp;
    }
    public void ClearAll () {
        positionCount = lineRenderer.positionCount = 1;
        currentIndex = 1;

        Destroy (endPoint);
    }
    public void PrepareDottedLine () {
        if (spawnManager) {
            Vector3[] pos = new Vector3[lineRenderer.positionCount];
            Debug.Log ("prepare line pos count: " + lineRenderer.positionCount);
            lineRenderer.GetPositions (pos);
        }
    }
    public void Set2EndPoints (Vector2 p1, Vector2 p2) {
        lineRenderer.SetPosition (lineRenderer.positionCount - 2, p1);
        lineRenderer.SetPosition (lineRenderer.positionCount - 1, p2);
    }
    public int GetPositionCount () {
        return positionCount;
    }
    /// <summary>
    /// Do stuff when user draw to airport
    /// </summary>
    /// <param name="active"></param>
    public void SetReadyToLand (bool active) {
        readyToLanding = active;

        plane.SwitchToHasWayPoint ();
        lineRenderer.startWidth = lineRenderer.endWidth = 0.4f;
        lineRenderer.material = defaultMaterial;
        //spawnManager = null;
    }
    public bool GetReadyToLand () {
        return readyToLanding;
    }
    public bool GetIsLanding () {
        return islanding;
    }
    public void DrawLine (Vector2 mousePos) {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint (mousePos);
        //line.startColor = line.endColor = colorTag;
        readyToLanding = false;
        isDisable = false;
        lineRenderer.startWidth = lineRenderer.endWidth = .6f;
        lineRenderer.material = startMaterial;
        plane.SwitchToInit ();
        if (endPoint != null) {
            Destroy (endPoint);
        }
        if (positionCount == 1) {
            positionCount = 2;
            SetPoint (transform.position);
            return;
        }
        if (lineRenderer.positionCount > 1) {
            if (sqrDistance (lineRenderer.GetPosition (lineRenderer.positionCount - 1), worldPos) > 0.5f * 0.5f) {
                SetPoint (worldPos);
                return;
            }
        } else if (sqrDistance (transform.position, worldPos) > 3 * 3) {
            SetPoint (worldPos);
            return;
        }

    }
    private float sqrDistance (Vector3 v1, Vector3 v2) {
        return (v2 - v1).sqrMagnitude;
    }
    public Vector2 GetLastPoint () {
        if (lineRenderer.positionCount - 1 >= 0)
            return lineRenderer.GetPosition (lineRenderer.positionCount - 1);
        else
            return transform.position;
    }

    private void SetPoint (Vector2 pos) {
        if (positionCount >= maxPointCount) {
            canDraw = false;
        }
        //spawnManager.AddDotIntoLine (pos);
        lineRenderer.positionCount = positionCount;
        finishLineIndex = positionCount - 1;
        lineRenderer.SetPosition (finishLineIndex, pos);
        positionCount++;
    }
    public void SetEndPoint (GameObject _endpoint) {
        canDraw = true;
        //spawnManager = null;
        //line.startColor = line.endColor = new Color (1, 1, 1, 1);
        lineRenderer.startWidth = lineRenderer.endWidth = 0.2f;
        lineRenderer.material = defaultMaterial;
        endPoint = _endpoint;
        endPoint.name = name + " endpoint";
        endPoint.GetComponent<EndPoint> ().SetLine (this);
        endPoint.GetComponent<SpriteRenderer> ().color = plane.initColor;
        if (lineRenderer.positionCount == 0)
            Destroy (endPoint);
    }
    private Vector3 V2ToV3 (Vector2 v) {
        return new Vector3 (v.x, v.y);
    }
    private float CalculationAngle (Vector3 direction) {
        return -Mathf.Atan2 (direction.x, direction.y) * Mathf.Rad2Deg;
    }
    //private void OnTriggerEnter2D(Collider2D coll) {
    //    if (coll.tag == "airPort") {
    //        if (coll.GetComponent<AirPort>().planeTag == planeTag && readyToLanding) {
    //            Land();
    //        }
    //    }
    //}
    public void SetPlaneTag (string tag) {
        planeTag = tag;
    }
    private IEnumerator Landing () {
        float lsX = transform.localScale.x;
        dynamicMoveSpeed = 3;
        Ally ally = GetComponent<Ally> ();
        if (ally) {
            ally.enabled = false;
        }
        while (lsX > 0.4f) {
            lsX = Mathf.Lerp (lsX, 0.4f, 2 * Time.deltaTime);
            transform.localScale = Vector3.one * lsX;
            if (lsX < 0.5f)
                break;
            yield return null;
        }
    }
    private IEnumerator Fade () {
        SpriteRenderer sp = GetComponent<SpriteRenderer> ();
        float alpha = sp.color.a;
        islanding = true;
        while (alpha > -1) {
            alpha = Mathf.Lerp (alpha, -1, 2 * Time.deltaTime);
            sp.color = new Color (sp.color.r, sp.color.g, sp.color.b, alpha);
            if (alpha <= 0)
                break;
            yield return null;
        }
        Destroy (gameObject);
    }
    private IEnumerator WaitForFallDown () {
        float lsX = transform.localScale.x;
        float slowSpeed = moveSpeed / 4;
        disableRotateSpeed = 120;
        isDisable = true;
        ClearAll ();
        plane.SwitchToInit ();
        while (lsX > 0.4f) {
            lsX = Mathf.Lerp (lsX, .4f, 2 * Time.deltaTime);
            dynamicMoveSpeed = Mathf.Lerp (dynamicMoveSpeed, slowSpeed, 2 * Time.deltaTime);
            transform.localScale = Vector3.one * lsX;
            if (lsX <= .5) {
                break;
            }
            yield return null;
        }
        yield return null;
        GetComponent<CircleCollider2D> ().isTrigger = true;
        SpawnManager.Instance.SpawnBlowEffect (transform.position);
        plane.TriggerGameOver ();
    }
    public void FallDown () {
        StartCoroutine (WaitForFallDown ());
    }
    private void Land () {
        if (!islanding) {
            islanding = true;
            ScoreManager.Instance.AddScore ();
            InGameSoundManager.Instance?.PlayASound (InGameSoundManager.Instance.ac_landedSound);
            SpawnManager.Instance.SpawnGreetText (transform.position);
            GetComponent<CircleCollider2D> ().isTrigger = true;
            if (GetComponent<Ally> ()) {
                GetComponent<Ally> ().enabled = false;
            }
            StartCoroutine (Landing ());
        }
    }
    private void OnTriggerStay2D (Collider2D coll) {
        if (coll.tag == "airPort") {
            if (coll.GetComponent<AirPort> ().planeTag == planeTag) {
                if (islanding) {
                    dynamicMoveSpeed = Mathf.Lerp (dynamicMoveSpeed, 4, 2 * Time.deltaTime);
                    if (lineRenderer.positionCount == 1) {
                        StartCoroutine (Fade ());
                    }
                }
                if (lineRenderer.positionCount <= 3 && readyToLanding) {
                    Land ();
                }
            }
        }
    }
    private void OnDestroy () {
        StopAllCoroutines ();
    }
}