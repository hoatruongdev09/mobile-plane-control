using System.Collections.Generic;
using UnityEngine;

public class Airport : MonoBehaviour {
    public IAirportDelegate Delegate { get; set; }
    public string PlaneTag {
        get { return planeTag; }
        set { planeTag = value; }
    }
    public Color ColorTag {
        get { return colorTag; }
        private set { colorTag = value; }
    }
    public virtual PlaneControl.PlaneType AcceptedPlaneType {
        get {
            return PlaneControl.PlaneType.air_plane;
        }
    }

    [SerializeField] protected string planeTag;
    [SerializeField] protected Color colorTag;
    public SpriteRenderer highlight;
    public int maxPointRecord = 5;
    public float angleToLand = 360;
    [SerializeField] protected List<Vector3> points;
    protected Vector2 airportSize;
    private float thresholdToLand = 4;
    private bool isHighlight = false;
    protected void Start () {
        Collider2D collider = GetComponent<Collider2D> ();
        airportSize = collider.bounds.size;
    }
    public void SetColorHighlight (Color color) {
        highlight.color = color;
        ColorTag = color;
    }
    public void Highlight () {
        if (isHighlight) { return; }
        isHighlight = true;
        LeanTween.cancel (highlight.gameObject);
        Color hightLightColor = highlight.color;
        LeanTween.value (highlight.gameObject, 0.6f, 1, .5f).setOnUpdate ((float value) => {
            hightLightColor.a = value;
            highlight.color = hightLightColor;
        }).setLoopPingPong ().setIgnoreTimeScale (true);
    }
    public void Dehighlight () {
        if (!isHighlight) { return; }
        isHighlight = false;
        LeanTween.cancel (highlight.gameObject);
        Color hightLightColor = highlight.color;
        LeanTween.value (highlight.gameObject, hightLightColor.a, .6f, .5f).setOnUpdate ((float value) => {
            hightLightColor.a = value;
            highlight.color = hightLightColor;
        }).setIgnoreTimeScale (true);
    }
    public void Record (Vector3 point, NewScript.Path path) {
        if (path.Controller.PlaneTag != PlaneTag) { return; }
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
    }
    public void MultitouchRecord (Vector3 point, NewScript.Path path, int touchIndex) {
        if (path.Controller.PlaneTag != PlaneTag) { return; }
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
                Delegate?.OnAddLandingPlane (touchIndex);
            }
        }
    }
    public virtual List<Vector3> GetLandingPoint () {
        Vector3 point1 = transform.TransformPoint (new Vector3 (0, 1));
        Vector3 point2 = transform.TransformPoint (new Vector3 (0, 1.5f));
        List<Vector3> points = new List<Vector3> ();
        points.Add (point1);
        points.Add (point2);
        return points;
    }
    protected bool CheckDirectToLand (Vector3[] points) {
        if (!CheckFirstPoint (points[0])) {
            Debug.Log ("not the first point");
            return false;
        }
        var acceptPointCount = FindAcceptLandPoint (points).Count;
        return acceptPointCount >= 2;
    }
    protected List<Vector3> FindAcceptLandPoint (Vector3[] points) {
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

    public void ClearPoints () {
        points.Clear ();
    }

    protected virtual bool CheckFirstPoint (Vector3 point) {
        Vector3 localPosition = transform.InverseTransformPoint (point);
        if (localPosition.sqrMagnitude > airportSize.sqrMagnitude) {
            return false;
        }
        // Debug.Log ($"{localPosition.y}   {airportSize.y / 10}");
        // if (Mathf.Abs (localPosition.y) > airportSize.y / 10 && localPosition.y < 0) {
        //     return false;
        // }
        return true;
    }
    private void OnDrawGizmos () {
        Gizmos.DrawRay (transform.position, transform.up);
    }
}
public interface IAirportDelegate {
    void OnAddLandingPlane (int touchIndex);
    void OnAddLandingPlane ();

}