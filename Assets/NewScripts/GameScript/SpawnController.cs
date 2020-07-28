using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public class SpawnController : MonoBehaviour {
    public List<PlaneControl> ListPlanes {
        get { return planes; }
        set { planes = value; }
    }
    public List<GameObject> ListEnemies {
        get { return enemies; }
        set { enemies = value; }
    }
    public int MaxPlaneInTime { get; set; }
    public Airport helipadPrefab;
    public Airport airportPrefab;
    public Indicator indicatorPrefab;
    private GameObject planesHolder;
    private GameObject indicateHolder;
    private GameObject airportHolder;
    private List<PlaneControl> planes;
    private List<GameObject> enemies;
    private void Awake () {
        planesHolder = new GameObject ("PlanesHolder");
        indicateHolder = new GameObject ("IndicateHolder");
        airportHolder = new GameObject ("AirportHolder");
    }
    public Airport CreateAirport () {
        var airport = Instantiate (airportPrefab, airportHolder.transform);
        return airport;
    }
    public Airport CreateHelipad () {
        var airport = Instantiate (helipadPrefab, airportHolder.transform);
        return airport;
    }
    public PlaneControl CreateAPlane () {
        var spawnPos = MapManager.Instance.RandomSpawnPos ();
        var destinatePost = MapManager.Instance.GetRandomPosition ();
        var plane = Instantiate (planes[Random.Range (0, planes.Count)], spawnPos, Quaternion.Euler (0, 0, -CalculationAngle (destinatePost - spawnPos)));
        plane.transform.SetParent (planesHolder.transform);
        SpawnIndicator (plane, spawnPos, destinatePost);
        return plane;
    }
    public PlaneControl CreateAPlaneForAirport (Airport airport, bool hasWater, bool hasFuel, float fuelMin, float fuelMax) {
        var spawnPosition = MapManager.Instance.RandomSpawnPos ();
        var destinatePosition = MapManager.Instance.GetRandomPosition ();
        var acceptedPlanes = planes.Where (planes => planes.planeType == airport.AcceptedPlaneType).ToArray ();
        if (acceptedPlanes.Length == 0) {
            throw new System.Exception ($"There is no planes accepted for airport {airport.name} ,type: {airport.AcceptedPlaneType}");
        }
        var plane = Instantiate (acceptedPlanes[Random.Range (0, acceptedPlanes.Length)], spawnPosition, Quaternion.Euler (0, 0, -CalculationAngle (destinatePosition - spawnPosition)));
        plane.transform.SetParent (planesHolder.transform);
        if (hasWater) {
            AddWaterComponent (plane);
        }
        if (hasFuel) {
            AddFuelComponent (plane, fuelMin, fuelMax);
        }
        plane.PlaneTag = airport.PlaneTag;
        plane.SetColor (airport.ColorTag);
        SpawnIndicator (plane, spawnPosition, destinatePosition);
        return plane;
    }
    private void AddWaterComponent (PlaneControl plane) {

    }
    private void AddFuelComponent (PlaneControl plane, float minTime, float maxTime) {

    }
    public float CalculationAngle (Vector2 direction) {
        return Mathf.Atan2 (direction.x, direction.y) * Mathf.Rad2Deg;
    }
    private void SpawnIndicator (PlaneControl plane, Vector3 spawnPos, Vector3 destinatePos) {
        RaycastHit2D hit = Physics2D.Raycast (spawnPos, destinatePos - spawnPos, Mathf.Infinity, LayerMask.GetMask ("border"));
        Indicator indicatorGO = Instantiate (indicatorPrefab, hit.point, Quaternion.Euler (0, 0, -CalculationAngle ((destinatePos - spawnPos).normalized)));
        indicatorGO.planeShape.sprite = plane.graphics[0].sprite;
        indicatorGO.planeShape.color = plane.baseColor;
        indicatorGO.transform.SetParent (indicateHolder.transform);
        Debug.DrawRay (spawnPos, destinatePos - spawnPos, Color.red, 10);
    }
}