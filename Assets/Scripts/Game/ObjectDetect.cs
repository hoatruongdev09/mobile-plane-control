using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetect : MonoBehaviour {

    public LayerMask objectLayer;
    public float detectRange = 5;
    public GameObject endPoint;

    private LineInfo activeLine;
    private AirPort activeAirport;
    private AirPort lastDetectAirport;
    private AirPort sameTagWithPlaneAirport;
    bool isMouseDown;

    private void Awake () {
        isMouseDown = false;
    }
    private void FixedUpdate () {
        //PC_Control ();
        New_PcControl ();
    }
    private void New_PcControl () {
        if (Input.GetMouseButtonDown (0)) {
            isMouseDown = true;
            if (activeLine.line == null) {
                activeLine = DetectPlane (Input.mousePosition);
                if (activeLine.line != null) {
                    Debug.Log ("active line tag: " + activeLine.tag);
                    if (activeLine.tag == "plane") {
                        activeLine.line.SetSpawnManager (SpawnManager.Instance);
                        activeLine.line.ClearAll ();
                    } else if (activeLine.tag == "endPoint") {
                        activeLine.line.SetSpawnManager (SpawnManager.Instance);
                        //activeLine.line.PrepareDottedLine ();
                    }
                    sameTagWithPlaneAirport = GetSameTagPlaneAirport (activeLine);
                    if (sameTagWithPlaneAirport != null) {
                        sameTagWithPlaneAirport.isHighlight = true;
                        InGameSoundManager.Instance.PlayASound (InGameSoundManager.Instance.ac_planeSelectSound);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp (0)) {
            Debug.Log ("Mouse UPPPP");
            isMouseDown = false;
            if (activeLine.line != null) {
                SpawnEndPoint (activeLine.line);
                DeHighLight ();
                activeLine.line = null;
            }
            if (activeAirport != null) {
                activeAirport.ClearAll ();
                activeAirport = null;
            }
        }
        if (Input.GetMouseButton (0)) {
            if (activeLine.line) {
                if (activeLine.line.canDraw) {
                    activeLine.line.DrawLine (Input.mousePosition);
                } else {
                    SpawnEndPoint (activeLine.line);
                    DeHighLight ();
                    activeLine.line = null;
                }
                activeAirport = DetectAirPort ();
                if (activeAirport != null) {
                    activeAirport.Record (Input.mousePosition, activeLine);
                    lastDetectAirport = activeAirport;
                } else {
                    if (lastDetectAirport != null)
                        lastDetectAirport.ClearAll ();
                }
            }
        } else {
            SpawnEndPoint (activeLine.line);
            DeHighLight ();
            activeLine.line = null;
        }
    }
    private void PC_Control () {
        if (Input.GetMouseButtonDown (0)) {
            isMouseDown = true;
            if (activeLine.line == null) {
                activeLine = DetectPlane ();
                if (activeLine.line != null) {
                    Debug.Log ("active line tag: " + activeLine.tag);
                    if (activeLine.tag == "plane") {
                        activeLine.line.SetSpawnManager (SpawnManager.Instance);
                        activeLine.line.ClearAll ();
                    } else if (activeLine.tag == "endPoint") {
                        activeLine.line.SetSpawnManager (SpawnManager.Instance);
                        //activeLine.line.PrepareDottedLine ();
                    }
                    sameTagWithPlaneAirport = GetSameTagPlaneAirport (activeLine);
                    if (sameTagWithPlaneAirport != null) {
                        sameTagWithPlaneAirport.isHighlight = true;
                        InGameSoundManager.Instance.PlayASound (InGameSoundManager.Instance.ac_planeSelectSound);
                    }
                }
            }

        }
        if (Input.GetMouseButtonUp (0)) {
            Debug.Log ("Mouse UPPPP");
            isMouseDown = false;
            if (activeLine.line != null) {
                SpawnEndPoint (activeLine.line);
                DeHighLight ();
                activeLine.line = null;
            }
            if (activeAirport != null) {
                activeAirport.ClearAll ();
                activeAirport = null;
            }
        }
        if (isMouseDown && Input.GetMouseButton (0)) {
            activeAirport = DetectAirPort ();
            if (activeAirport != null) {
                activeAirport.Record (Input.mousePosition, activeLine);
                lastDetectAirport = activeAirport;
            } else {
                if (lastDetectAirport != null)
                    lastDetectAirport.ClearAll ();
            }
            if (activeLine.line != null) {
                Debug.Log ("Drawing motherfucker");
                if (activeLine.line.canDraw) {
                    activeLine.line.DrawLine (Input.mousePosition);
                } else {
                    SpawnEndPoint (activeLine.line);
                    DeHighLight ();
                    activeLine.line = null;
                }
            }
        } else {
            SpawnEndPoint (activeLine.line);
            DeHighLight ();
            activeLine.line = null;
        }

        // if (activeLine.line != null && isMouseDown) {
        //     Debug.Log ("Drawing motherfucker");
        //     activeLine.line.DrawLine (Input.mousePosition);
        // }
    }
    public void DeHighLight () {
        if (sameTagWithPlaneAirport != null) {
            InGameSoundManager.Instance.PlayASound (InGameSoundManager.Instance.ac_planeDeselectSound);
            sameTagWithPlaneAirport.isHighlight = false;
            sameTagWithPlaneAirport = null;
        }
    }
    private AirPort GetSameTagPlaneAirport (LineInfo aline) {
        foreach (AirPort ap in FindObjectsOfType<AirPort> ()) {
            if (ap.planeTag == aline.planeTag)
                return ap;
        }
        return null;
    }
    private void SpawnEndPoint (Line _activeLine) {
        if (_activeLine != null) {
            GameObject pointGO = SpawnManager.Instance.SpawnEndPoint (_activeLine);
            _activeLine.SetEndPoint (pointGO);
        }
    }
    private AirPort DetectAirPort () {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, 1000, objectLayer);
        if (hit.collider != null) {
            if (hit.collider.tag == "airPort") {
                Debug.Log (hit.collider.name);
                return hit.collider.GetComponent<AirPort> ();
            }
        }
        return null;
    }
    private LineInfo DetectPlane (Vector2 mousePosition) {
        mousePosition = Camera.main.ScreenToWorldPoint (mousePosition);
        Collider2D[] planeNearby = Physics2D.OverlapCircleAll (mousePosition, detectRange, objectLayer);
        Debug.Log ("nearby plane: " + planeNearby.Length);
        Collider2D closestNearBy = GetClosestCollider (planeNearby, mousePosition);
        Line tmp = null;
        if (closestNearBy) {
            if (closestNearBy.tag == "plane") {
                tmp = closestNearBy.GetComponent<Line> ();
                if (!tmp.GetIsLanding ()) {
                    return new LineInfo (tmp, closestNearBy.tag, tmp.planeTag);
                }
            } else if (closestNearBy.tag == "endPoint") {
                tmp = closestNearBy.GetComponent<EndPoint> ().GetLine ();
                return new LineInfo (tmp, closestNearBy.tag, tmp.planeTag);
            } else {
                return new LineInfo (null, "nothing", "nothing");
            }
        }
        return new LineInfo (null, "nothing", "nothing");

    }
    private Collider2D GetClosestCollider (Collider2D[] collArr, Vector2 position) {
        if (collArr.Length == 0)
            return null;
        Collider2D closestCollider = collArr[0];
        float closestDistance = ((Vector2) closestCollider.transform.position - position).sqrMagnitude;
        float range = Mathf.Infinity;
        for (int i = 1; i < collArr.Length; i++) {
            range = ((Vector2) collArr[i].transform.position - position).sqrMagnitude;
            if (range < closestDistance) {
                closestCollider = collArr[i];
                closestDistance = range;
            }
        }
        return closestCollider;
    }
    private LineInfo DetectPlane () {
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast (ray.origin, ray.direction, 1000, objectLayer);
        if (hit.collider != null) {
            if (hit.collider.tag == "plane") {
                Debug.Log (hit.collider.name);
                if (hit.collider.GetComponent<Enemy> ()) {
                    Debug.Log ("This is enemy dude");
                    return new LineInfo (null, "nothing", "nothing");
                }
                Line tmp = hit.collider.GetComponent<Line> ();
                if (!tmp.GetIsLanding ())
                    return new LineInfo (tmp, hit.collider.tag, tmp.planeTag);
            } else if (hit.collider.tag == "endPoint") {
                Debug.Log (hit.collider.name);
                Line tmp = hit.collider.GetComponent<EndPoint> ().GetLine ();
                return new LineInfo (tmp, hit.collider.tag, tmp.planeTag);
            }
        }
        Debug.Log ("null");
        return new LineInfo (null, "nothing", "nothing");
    }
    public LineInfo GetActiveLine () {
        return activeLine;
    }
    public void SetActiveLine (LineInfo _line) {
        activeLine = _line;
    }
    public struct LineInfo {
        public Line line;
        public string tag;
        public string planeTag;
        public LineInfo (global::Line _line, string _tag, string _planeTag) {
            line = _line;
            tag = _tag;
            planeTag = _planeTag;
        }
    }

}