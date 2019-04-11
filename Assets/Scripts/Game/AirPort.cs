using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPort : MonoBehaviour {

    public string planeTag;
    public bool longAirtPort;
    public float angleToLand = 15;
    public float thresholdToLand = 4;
    public List<Vector2> points;
    public int maxPoint = 5;

    public List<Line> planeLanding;
    public SpriteRenderer highlight;
    public bool isHighlight;
    private float l2v;

    private void Awake () {
        var cll2d = GetComponent<Collider2D> ();
        l2v = cll2d.bounds.size.y / 2;

    }
    private void Start () {
        StartCoroutine (DelayInit ());
    }
    private void Update () {
        if (isHighlight) {
            highlight.color = Color.Lerp (highlight.color, new Color (highlight.color.r, highlight.color.g, highlight.color.b, 1), 10 * Time.deltaTime);
        } else {
            highlight.color = Color.Lerp (highlight.color, new Color (highlight.color.r, highlight.color.g, highlight.color.b, .4f), 5 * Time.deltaTime);
        }
    }

    public void Record (Vector2 point, ObjectDetect.LineInfo info) {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint (point);
        if (points == null || points.Count == 0) {
            points = new List<Vector2> ();
            SetPoint (worldPoint);
        }
        if (points.Count < maxPoint) {
            if (sqrDistance (points[points.Count - 1], worldPoint) > thresholdToLand * thresholdToLand) {
                SetPoint (worldPoint);
            }
        }
        if (info.planeTag == planeTag) {
            if (points.Count >= 2) {
                if (CheckDirectToLand (points.ToArray ())) {
                    AddPlaneLanding (info.line);
                }
            } else {
                RemovePlaneLanding (info.line);
            }
        }
    }
    private void AddPlaneLanding (Line plane) {
        if (!planeLanding.Contains (plane)) {
            planeLanding.Add (plane);
            plane.SetReadyToLand (true);
            Set2EndPoint (plane);
            ObjectDetect oj = FindObjectOfType<ObjectDetect> ();
            oj.SetActiveLine (new ObjectDetect.LineInfo (null, "nothing", "nothing"));
            oj.DeHighLight ();
        }
    }
    private void RemovePlaneLanding (Line plane) {
        if (planeLanding.Contains (plane)) {
            planeLanding.Remove (plane);
        }
    }
    public void ClearAll () {
        points.Clear ();
    }
    private bool CheckDirectToLand (Vector2[] _points) {
        int s = 0;
        if (!CheckFirstPoint (_points[0]) && longAirtPort) {
            Debug.Log ("not the first point");
            return false;
        }
        for (int i = 1; i < _points.Length; i++) {
            float angle = Vector2.Angle (_points[i] - _points[i - 1], transform.up);
            Debug.Log ("angle to land: " + angle);
            if (angle <= angleToLand)
                s++;
            angle = Vector2.Angle (_points[i] - _points[0], transform.up);
            if (angle <= angleToLand)
                s++;
        }

        Debug.Log (s);
        return s >= 2;
    }
    private float sqrDistance (Vector3 v1, Vector3 v2) {
        return (v2 - v1).sqrMagnitude;
    }
    private void SetPoint (Vector2 point) {
        points.Add (point);
    }
    private bool CheckFirstPoint (Vector2 p) {
        Vector2 w2l = transform.InverseTransformPoint (p);
        Debug.Log ("w2l.y : " + w2l.y);
        if (w2l.y > -l2v / 10)
            return false;
        return true;
    }
    private void Set2EndPoint (Line _line) {
        Vector2 v1 = transform.position - transform.up * l2v / 4;
        Vector2 v2 = transform.position; //+ transform.up * l2v / 8;
        _line.Set2EndPoints (v1, v2);
    }
    private IEnumerator DelayInit () {
        yield return new WaitUntil (() => GameControl.Instance != null);
        highlight.color = GameControl.Instance.GetColor (planeTag);
    }
}