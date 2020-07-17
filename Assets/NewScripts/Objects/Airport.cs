using System.Collections.Generic;
using UnityEngine;

public class Airport : MonoBehaviour {
    public IAirportDelegate Delegate { get; set; }
    public string planeTag;
    public SpriteRenderer highlight;
    public int maxPointRecord = 5;
    public float angleToLand = 360;
    [SerializeField] private List<Vector3> points;
    private Vector2 airportSize;
    private float thresholdToLand = 4;

    private void Start () {
        Collider2D collider = GetComponent<Collider2D> ();
        airportSize = collider.bounds.size;
    }
    public void Record (Vector3 point, NewScript.Path path) {
        if (path.Controller.PlaneTag != planeTag) { return; }
        if (points == null || points.Count == 0) {
            points = new List<Vector3> ();
            points.Add (point);
        }
        if (points.Count < maxPointRecord) {
            if ((point - points[points.Count - 1]).sqrMagnitude > Mathf.Pow (thresholdToLand, 2)) {
                points.Add (point);
            }
        }
        if (points.Count >= 2) {
            if (CheckDirectToLand (points.ToArray ())) {
                Delegate?.OnAddLandingPlane ();
            }
        }
        // else {
        //     // Debug.Log ("remove plane to landing");
        // }
    }
    public List<Vector3> GetLandindPoint () {
        Vector3 point1 = transform.position;
        Vector3 point2 = transform.position + transform.up * (airportSize.y / 2);
        List<Vector3> points = new List<Vector3> ();
        points.Add (point1);
        points.Add (point2);
        return points;
    }
    private bool CheckDirectToLand (Vector3[] points) {
        if (!CheckFirstPoint (points[0])) {
            // Debug.Log ("not the first point");
            return false;
        }
        var acceptPointCount = FindAcceptLandPoint (points).Count;
        return acceptPointCount >= 2;
    }
    private List<Vector3> FindAcceptLandPoint (Vector3[] points) {
        List<Vector3> acceptedPoints = new List<Vector3> ();
        float tempAngle = 0;
        for (int i = 1; i < points.Length; i++) {
            tempAngle = Vector2.Angle (points[i] - points[i - 1], transform.up);
            // Debug.Log ($"angle to land: {tempAngle}");
            if (tempAngle <= angleToLand) {
                acceptedPoints.Add (points[i]);
            }
            tempAngle = Vector2.Angle (points[i] - points[0], transform.up);
            if (tempAngle <= angleToLand) {
                acceptedPoints.Add (points[i]);
            }
        }
        return acceptedPoints;
    }

    internal void ClearPoints () {
        points.Clear ();
    }

    private bool CheckFirstPoint (Vector3 point) {
        Vector3 localPosition = transform.InverseTransformPoint (point);
        // Debug.Log ($"{localPosition.y}  {-(airportSize.y)/10}");
        if (localPosition.y > -(airportSize.y) / 10) {
            return false;
        }
        return true;
    }

}
public interface IAirportDelegate {
    void OnAddLandingPlane ();

}