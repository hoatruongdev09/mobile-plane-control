using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PathDrawer : MonoBehaviour {
    public LayerMask detectPlaneLayerMask;
    public LayerMask detectEndpointLayerMask;
    public LayerMask detectAirportLayerMask;

    public void DrawPath (NewScript.Path path, Vector3 position) {
        if (path == null) { return; }
        path.AddPoint (position);
    }
    public Airport DetectAirport (Vector3 worldPosition) {
        Collider2D[] airPortNearby = Physics2D.OverlapCircleAll (worldPosition, .1f, detectAirportLayerMask);
        Collider2D closest = FindClosestObject (airPortNearby, worldPosition);
        if (closest == null) { return null; }
        return closest.GetComponent<Airport> ();
    }
    public PlaneControl DetectPlane (Vector3 worldPosition) {
        Collider2D[] planeNearby = Physics2D.OverlapCircleAll (worldPosition, 1f, detectPlaneLayerMask);
        Collider2D closest = FindClosestObject (planeNearby, worldPosition);
        if (closest == null) { return null; }
        return closest.GetComponent<PlaneControl> ();
    }
    public PathEndpoint DetectEndPoint (Vector3 worldPosition) {
        Collider2D[] endPointNearby = Physics2D.OverlapCircleAll (worldPosition, 1f, detectEndpointLayerMask);
        Collider2D closest = FindClosestObject (endPointNearby, worldPosition);
        if (closest == null) { return null; }
        return closest.GetComponent<PathEndpoint> ();
    }
    private Collider2D FindClosestObject (Collider2D[] colliders, Vector3 position) {
        if (colliders.Length == 1) {
            return colliders[0];
        }
        if (colliders.Length == 0) {
            return null;
        }
        Collider2D closest = colliders[0];
        for (int i = 1; i < colliders.Length; i++) {
            if ((colliders[i].transform.position - position).sqrMagnitude < (closest.transform.position - position).sqrMagnitude) {
                closest = colliders[i];
            }
        }
        return closest;
    }
}