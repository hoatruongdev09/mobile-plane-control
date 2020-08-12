using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
public class SpawnController : MonoBehaviour {
    public static SpawnController Instance { get; private set; }
    public List<PlaneControl> ListPlanes {
        get { return planes; }
        set { planes = value; }
    }
    public List<GameObject> ListEnemies {
        get { return enemies; }
        set { enemies = value; }
    }
    public Cloud[] cloudPrefabs;
    [SerializeField] private List<PlaneControl> planes;
    [SerializeField] private List<GameObject> enemies;
    public NewWaterDrop waterDropPrefab;
    public GameObject warningSign;
    public ProcessBarGame processBarPrefab;
    public FuelAnnouncer fuelPrefab;
    public Tornado tornadoPrefab;
    public FireForest fireForestPrefab;
    public Airport helipadPrefab;
    public Airport airportPrefab;
    public Indicator indicatorPrefab;
    public GameObject crashBlowEffectPrefab;
    public GameObject inAirBlowEffectPrefab;
    public GameObject winSockPrefab;
    private GameObject planesHolder;
    private GameObject indicateHolder;
    private GameObject airportHolder;
    private GameObject tornadoHolder;
    private GameObject forestFireHolder;
    private GameObject fuelHolder;
    private GameObject cloudHolder;

    private void Awake () {
        planesHolder = new GameObject ("PlanesHolder");
        indicateHolder = new GameObject ("IndicateHolder");
        airportHolder = new GameObject ("AirportHolder");
        tornadoHolder = new GameObject ("TornadoHolder");
        forestFireHolder = new GameObject ("ForestFireHolder");
        fuelHolder = new GameObject ("FuelHolder");
        cloudHolder = new GameObject ("Cloud Holder");
        if (Instance == null) {
            Instance = this;
        }
    }
    public GameObject CreateBlowEffect (Vector3 position, GameObject effect) {
        var fx = Instantiate (effect, position, Quaternion.Euler (0, 0, Random.Range (0, 359)));
        Destroy (fx, 1);
        return fx;
    }
    public Cloud CreateCloud (Vector3 position) {
        return Instantiate (cloudPrefabs[Random.Range (0, cloudPrefabs.Length)], position, Quaternion.identity, cloudHolder.transform);
    }
    public NewWaterDrop CreateWaterDrop (Transform transform) {
        return Instantiate (waterDropPrefab, transform);
    }
    public ProcessBarGame CreateProcessBar (Transform transform) {
        return Instantiate (processBarPrefab, transform);
    }
    public FireForest CreateFireForest (float maxHp, Vector3 position) {
        var fire = Instantiate (fireForestPrefab, position, Quaternion.identity, forestFireHolder.transform);
        fire.MaxHP = maxHp;
        return fire;
    }
    public FuelAnnouncer CreateFuelAnnouncer () {
        return Instantiate (fuelPrefab, fuelHolder.transform);
    }
    public GameObject CreateWarningSign (Vector3 position, float lifeTime) {
        var sign = Instantiate (warningSign, position, Quaternion.identity);
        Destroy (sign, lifeTime);
        return sign;
    }
    public Tornado CreateTornado (float lifeTime, Vector3 position) {
        var tornado = Instantiate (tornadoPrefab, position, Quaternion.identity, tornadoHolder.transform);
        tornado.LifeTime = lifeTime;
        return tornado;
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
        Debug.Log ($"create plane with fuel: {hasFuel}");
        var spawnPosition = MapManager.Instance.RandomSpawnPos ();
        var destinatePosition = MapManager.Instance.GetRandomPosition ();
        var acceptedPlanes = planes.Where (planes => planes.planeType == airport.AcceptedPlaneType).ToArray ();
        if (acceptedPlanes.Length == 0) {
            throw new System.Exception ($"There is no planes accepted for airport {airport.name} ,type: {airport.AcceptedPlaneType}");
        }
        var plane = Instantiate (acceptedPlanes[Random.Range (0, acceptedPlanes.Length)], spawnPosition, Quaternion.Euler (0, 0, -CalculationAngle (destinatePosition - spawnPosition)));
        plane.transform.SetParent (planesHolder.transform);
        AddShadowComponent (plane);
        if (hasWater) {
            AddWaterComponent (plane, Random.Range (100, 120));
        }
        if (hasFuel) {
            AddFuelComponent (plane, fuelMin, fuelMax);
        }
        plane.PlaneTag = airport.PlaneTag;
        plane.SetColor (airport.ColorTag);
        SpawnIndicator (plane, spawnPosition, destinatePosition);
        return plane;
    }
    private void AddWaterComponent (PlaneControl plane, float maxWater) {
        var water = plane.gameObject.AddComponent<PlaneWaterComponent> ();
        if (water == null) {
            Debug.Log ("water is null");
            return;
        }
        water.MaxWater = maxWater;
        water.AttachToPlane (plane);
    }
    private void AddShadowComponent (PlaneControl plane) {
        var shadow = plane.gameObject.AddComponent<PlaneShadowComponent> ();
        shadow.AttachToPlane (plane);
    }
    private void AddFuelComponent (PlaneControl plane, float minTime, float maxTime) {
        var fuel = plane.gameObject.AddComponent<PlaneFuelComponent> ();
        if (fuel == null) {
            Debug.Log ("fuel is null");
            return;
        }
        fuel.StartFuel = Random.Range (minTime, maxTime);
        fuel.AttachToPlane (plane);
    }
    public float CalculationAngle (Vector2 direction) {
        return Mathf.Atan2 (direction.x, direction.y) * Mathf.Rad2Deg;
    }
    private void SpawnIndicator (PlaneControl plane, Vector3 spawnPos, Vector3 destinatePos) {
        Vector2 planeDirect = (destinatePos - spawnPos);
        RaycastHit2D hit = Physics2D.Raycast (spawnPos, planeDirect, Mathf.Infinity, LayerMask.GetMask ("border"));
        Indicator indicatorGO = Instantiate (indicatorPrefab, hit.point + planeDirect.normalized * 8, Quaternion.Euler (0, 0, -CalculationAngle ((destinatePos - spawnPos).normalized)));
        indicatorGO.CreatePlanShape (plane.graphics, plane.baseColor);
        // indicatorGO.planeShape[0].sprite = plane.graphics[0].sprite;
        // indicatorGO.planeShape[0].color = plane.baseColor;
        indicatorGO.transform.SetParent (indicateHolder.transform);
        // Debug.DrawRay (spawnPos, destinatePos - spawnPos, Color.red, 10);
    }
}