using System.Collections.Generic;
using UnityEngine;
public class Helipad : Airport {
    public override PlaneControl.PlaneType AcceptedPlaneType {
        get {
            return PlaneControl.PlaneType.helicopter;
        }
    }
    protected override bool CheckFirstPoint (Vector3 point) {
        var distance = (point - transform.position).sqrMagnitude;
        Debug.Log ($"distance: {distance}  {airportSize.x*airportSize.x}");
        if (distance <= airportSize.x * airportSize.x) {
            return true;
        }
        return false;
    }
    public override List<Vector3> GetLandingPoint () {
        List<Vector3> points = new List<Vector3> ();
        points.Add (transform.position);
        return points;
    }
}