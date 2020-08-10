using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SpawnManager : MonoBehaviour {

    public static SpawnManager Instance { get; private set; }
    public LayerMask borderLayer;
    public bool testOption = false;
    public bool isShowTutor = false;
    [Header ("Plane spawning")]
    public float spawnPlaneInterval = 5;
    public float planeSpawnTime = 5;
    public int maxPlaneInTime = 7;
    public List<GameObject> listPlane;
    public List<GameObject> listHelicopter;
    [Header ("Low Fuel mode")]
    public bool lowFuelMode;
    public float chanceToLowFuel = 20;
    [Header ("Cloud spawning")]
    public bool hasCloud;
    public float spawnCloudInterval = 3;
    public float cloudSpawnTime = 3;
    public int maxCloudInTime = 4;
    public Vector2 windDirection;
    public List<GameObject> listCloud;
    [Header ("Enemy spawning")]
    public bool hasEnemy = false;
    public int maxEnemyInTime = 1;
    public float spawnEnemyPlaneInterval = 10;
    public float enemyPlaneSpawnTime = 5;
    public List<Enemy> listEnemyPlane;
    [Header ("Hurricane spawning")]
    public bool hasHurricane = false;
    public int maxHurricaneInTime = 4;
    public float spawnHurricaneInterval = 10;
    public float hurricaneSpawnTime;
    public GameObject hurricanePrefab;
    [Header ("Forest fire")]
    public bool hasForestFire;
    public int maxForestFireInTime = 4;
    public float spawnForestFireInterval = 10;
    public float forestFireSpawnTime;
    public GameObject forestFirePrefab;
    [Header ("Effect")]
    public GameObject[] blowFX_inAirPrefab;

    public GameObject[] blowEffectPrefabs;
    public GameObject smokeEffectPrefab;
    [Header ("Airport in game")]
    public AirPort[] airports;

    [Header ("Miscellaneous")]
    public GameObject endpointPrefabs;
    public GameObject pointPrefabs;
    public GameObject indicatorPrefab;
    public GameObject txt_greetPrefab;
    public GameObject warning_signPrefab;
    public WaterDrop waterdrop_prefab;
    #region OBJECTS HOLDER
    private GameObject POINT_HOLDER;
    private GameObject PLANE_HOLDER;
    private GameObject SHADOW_HOLDER;
    private GameObject INDICATOR_HOLDER;
    private GameObject EFFECT_HOLDER;
    private GameObject HURRICANE_HOLDER;
    private GameObject ENEMY_PLANE_HOLDER;
    private GameObject CLOUD_HOLDER;
    private GameObject FORESTFIRE_HOLDER;
    public GameObject BULLET_HOLDER;

    #endregion

    private void Start () {
        Instance = this;
        POINT_HOLDER = new GameObject ("POINT_HOLDER");
        PLANE_HOLDER = new GameObject ("PLANE_HOLDER");
        SHADOW_HOLDER = new GameObject ("SHADOW_HOLDER");
        INDICATOR_HOLDER = new GameObject ("INDICATOR_HOLDER");
        EFFECT_HOLDER = new GameObject ("EFFECT_HOLDER");

        if (hasEnemy) {
            ENEMY_PLANE_HOLDER = new GameObject ("ENEMY_PLANE_HOLDER");
            BULLET_HOLDER = new GameObject ("BULLET_HOLDER");
        }
        if (hasHurricane)
            HURRICANE_HOLDER = new GameObject ("HURRICANE_HOLDER");
        if (hasCloud)
            CLOUD_HOLDER = new GameObject ("CLOUD_HOLDER");
        if (hasForestFire)
            FORESTFIRE_HOLDER = new GameObject ("FORESTFIRE_HOLDER");

        airports = FindObjectsOfType<AirPort> ();
        string difficultString = PlayerPrefs.GetString ("difficult");
        StartCoroutine (LoadDifficultData ("difficult_" + difficultString + ".json"));
    }

    private void Update () {
        if (testOption)
            return;
        if (isShowTutor) {
            return;
        }
        if (planeSpawnTime <= 0) {
            SpawnAPlane ();
            planeSpawnTime = spawnPlaneInterval;
        } else {
            planeSpawnTime -= Time.deltaTime;
        }
        if (hasHurricane) {
            if (hurricaneSpawnTime <= 0) {
                SpawnHurricane ();
                hurricaneSpawnTime = spawnHurricaneInterval;
            } else {
                hurricaneSpawnTime -= Time.deltaTime;
            }
        }
        if (hasForestFire) {
            if (forestFireSpawnTime <= 0) {
                SpawnForestFire ();
                forestFireSpawnTime = spawnForestFireInterval;
            } else {
                forestFireSpawnTime -= Time.deltaTime;
            }
        }
        if (hasEnemy) {
            if (enemyPlaneSpawnTime <= 0) {
                SpawnEnemy ();
                enemyPlaneSpawnTime = spawnEnemyPlaneInterval;
            } else {
                enemyPlaneSpawnTime -= Time.deltaTime;
            }
        }
        if (hasCloud) {
            if (cloudSpawnTime <= 0) {
                SpawnCloud ();
                cloudSpawnTime = spawnCloudInterval;
            } else {
                cloudSpawnTime -= Time.deltaTime;
            }
        }
    }
    public void OnReset () {
        Destroy (POINT_HOLDER);
        Destroy (PLANE_HOLDER);
        Destroy (SHADOW_HOLDER);
        Destroy (INDICATOR_HOLDER);
        Destroy (EFFECT_HOLDER);
        Destroy (ENEMY_PLANE_HOLDER);
        Destroy (HURRICANE_HOLDER);
        Destroy (BULLET_HOLDER);
        Destroy (CLOUD_HOLDER);

        POINT_HOLDER = new GameObject ("POINT_HOLDER");
        PLANE_HOLDER = new GameObject ("PLANE_HOLDER");
        SHADOW_HOLDER = new GameObject ("SHADOW_HOLDER");
        INDICATOR_HOLDER = new GameObject ("INDICATOR_HOLDER");
        EFFECT_HOLDER = new GameObject ("EFFECT_HOLDER");

        if (hasEnemy) {

            ENEMY_PLANE_HOLDER = new GameObject ("ENEMY_PLANE_HOLDER");
            BULLET_HOLDER = new GameObject ("BULLET_HOLDER");
        }
        if (hasHurricane)
            HURRICANE_HOLDER = new GameObject ("HURRICANE_HOLDER");
        if (hasCloud)
            CLOUD_HOLDER = new GameObject ("CLOUD_HOLDER");
    }
    public void SpawnSmokePrefab (Transform parent) {
        GameObject go = Instantiate (smokeEffectPrefab, parent.transform.position + new Vector3 (Random.Range (-.5f, .5f), Random.Range (-.5f, .5f)), Quaternion.identity);
        go.transform.parent = parent;

    }
    public void SpawnRandomInAirBlowFX (Vector2 point) {
        GameObject go = Instantiate (blowFX_inAirPrefab[Random.Range (0, blowFX_inAirPrefab.Length)], point, Quaternion.identity);
        go.transform.parent = EFFECT_HOLDER.transform;
        Destroy (go, 3);
    }
    public void SpawnCloud () {
        if (CLOUD_HOLDER.transform.childCount < maxCloudInTime) {
            GameObject cloudGo = Instantiate (listCloud[Random.Range (0, listCloud.Count)], MapManager.Instance.GetCloudRandomPosition (), Quaternion.identity);
            cloudGo.transform.parent = CLOUD_HOLDER.transform;
            cloudGo.AddComponent<Shadow> ();
        }
    }
    public void SpawnForestFire () {
        if (FORESTFIRE_HOLDER.transform.childCount < maxForestFireInTime) {
            GameObject forestfire = Instantiate (forestFirePrefab, MapManager.Instance.GetHurricaneRandomPosition (), Quaternion.identity);
            forestfire.transform.parent = FORESTFIRE_HOLDER.transform;
        }
    }
    public void SpawnHurricane () {
        if (HURRICANE_HOLDER.transform.childCount < maxHurricaneInTime) {
            GameObject hurricaneGo = Instantiate (hurricanePrefab, MapManager.Instance.GetHurricaneRandomPosition (), Quaternion.identity);
            hurricaneGo.transform.parent = HURRICANE_HOLDER.transform;
        }
    }
    public void SpawnBlowEffect (Vector3 position) {
        GameObject blowGO = Instantiate (blowEffectPrefabs[Random.Range (0, blowEffectPrefabs.Length)], position, Quaternion.identity);
        blowGO.transform.parent = EFFECT_HOLDER.transform;
        Destroy (blowGO, 3);
    }
    public GameObject SpawnEndPoint (Line activeLine) {
        GameObject pointGO = Instantiate (endpointPrefabs, activeLine.GetLastPoint (), Quaternion.identity);
        pointGO.transform.parent = POINT_HOLDER.transform;
        return pointGO;
    }
    public void SpawnGreetText (Vector3 position) {
        GameObject popUP = Instantiate (txt_greetPrefab, position, Quaternion.identity);
        popUP.transform.SetParent (EFFECT_HOLDER.transform); // = EFFECT_HOLDER.transform;
    }
    public void SpawnWarningSign (Vector3 position, float lifeTime) {
        GameObject warningSign = Instantiate (warning_signPrefab, position, Quaternion.identity);
        Destroy (warningSign, lifeTime);
    }
    public void SpawnEnemy () {
        //Stuff
        if (ENEMY_PLANE_HOLDER.transform.childCount < maxEnemyInTime) {
            Vector3 spawnPos = MapManager.Instance.RandomSpawnPos ();
            Vector3 destinatePos = MapManager.Instance.GetRandomPosition ();
            Debug.Log ("Random destinate: " + destinatePos);
            GameObject enemyPlane = Instantiate (listEnemyPlane[Random.Range (0, listEnemyPlane.Count)], spawnPos, Quaternion.Euler (0, 0, -CalculationAngle ((destinatePos - spawnPos).normalized))).gameObject;
            enemyPlane.GetComponent<Line> ().SetLookDirect ((destinatePos - enemyPlane.transform.position).normalized);
            enemyPlane.GetComponent<Enemy> ().SetDestination (destinatePos);
            enemyPlane.transform.parent = ENEMY_PLANE_HOLDER.transform;
            SpawnIndicator (enemyPlane, spawnPos, destinatePos);
            Debug.Log ("Spawn enemy");
        }
    }
    private void SpawnAPlane () {
        if (PLANE_HOLDER.transform.childCount < maxPlaneInTime) {
            Vector3 spawnPos = MapManager.Instance.RandomSpawnPos ();
            Vector3 destinatePos = MapManager.Instance.GetRandomPosition ();
            string[] analizedTag = GameControl.Instance.GetPlaneInfo (GameControl.Instance.GetPlaneTag ());

            GameObject plane = SpawnPlaneByInfo (analizedTag[0], analizedTag[1], spawnPos, destinatePos);
            Plane thisPlane = plane.GetComponent<Plane> ();
            thisPlane.initColor = GameControl.Instance.GetColor (analizedTag[1]);
            if (lowFuelMode && Chance (chanceToLowFuel)) {
                plane.AddComponent<Fuel> ();
            }
            if (hasForestFire) {
                WaterDrop waterdrop = Instantiate (waterdrop_prefab, plane.transform);
                thisPlane.waterDrop = waterdrop;
                waterdrop.transform.localPosition = Vector2.zero;
            }
            plane.GetComponent<Line> ().SetLookDirect ((destinatePos - plane.transform.position).normalized);
            plane.transform.parent = PLANE_HOLDER.transform;
            SpawnIndicator (plane, spawnPos, destinatePos);
        }
    }
    private GameObject SpawnPlaneByInfo (string type, string tag, Vector2 spawnPos, Vector2 destinatePos) {
        GameObject plane = null;
        switch (type) {
            case "plane":
                plane = Instantiate (listPlane[Random.Range (0, listPlane.Count)], spawnPos, Quaternion.Euler (0, 0, -CalculationAngle ((destinatePos - spawnPos).normalized)));
                plane.GetComponent<Line> ().planeTag = tag;
                break;
            case "heli":
                plane = Instantiate (listHelicopter[Random.Range (0, listHelicopter.Count)], spawnPos, Quaternion.Euler (0, 0, -CalculationAngle ((destinatePos - spawnPos).normalized)));
                plane.GetComponent<Line> ().planeTag = tag;
                break;
        }

        return plane;
    }
    private void SpawnIndicator (GameObject plane, Vector3 spawnPos, Vector3 destinatePos) {
        RaycastHit2D hit = Physics2D.Raycast (spawnPos, destinatePos - spawnPos, Mathf.Infinity, LayerMask.GetMask ("border"));
        GameObject indicatorGO = Instantiate (indicatorPrefab, hit.point, Quaternion.Euler (0, 0, -CalculationAngle ((destinatePos - spawnPos).normalized)));
        //indicatorGO.transform.position = hit.point;
        Indicator indicate = indicatorGO.GetComponent<Indicator> ();
        indicate.planeShape[0].sprite = plane.GetComponent<SpriteRenderer> ().sprite;
        Color color = plane.GetComponent<Plane> ().initColor;
        indicate.planeShape[0].color = color;
        color.a = .4f;
        indicate.planeShape[0].color = color;
        indicatorGO.transform.parent = INDICATOR_HOLDER.transform;
        //Debug.Log (hit.point);
        Debug.DrawRay (spawnPos, destinatePos - spawnPos, Color.red, 10);

    }
    public bool Chance (float chance) {
        return Random.Range (1, 101) <= chance;
    }
    public float CalculationAngle (Vector2 direction) {
        return Mathf.Atan2 (direction.x, direction.y) * Mathf.Rad2Deg;
    }

    public GameObject GetPlaneHolder () {
        return PLANE_HOLDER;
    }
    private IEnumerator LoadDifficultData (string fileName) {
        string filePath;
        filePath = Path.Combine (Application.streamingAssetsPath + "/", fileName);
        string dataAsJson;
        if (filePath.Contains ("://") || filePath.Contains (":///")) {
            UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get (filePath);
            yield return www.SendWebRequest ();
            dataAsJson = www.downloadHandler.text;
        } else {
            dataAsJson = File.ReadAllText (filePath);
        }
        DifficultConst difficult = JsonUtility.FromJson<DifficultConst> (dataAsJson);
        ApplyDifficultData (difficult);
        Debug.Log ("Loaded " + fileName);
    }

    private void ApplyDifficultData (DifficultConst difficult) {
        spawnPlaneInterval = difficult.spawnCloudInterval;
        maxPlaneInTime = difficult.maxPlaneInTime;
        spawnCloudInterval = difficult.spawnCloudInterval;
        maxCloudInTime = difficult.maxCloudInTime;
        spawnEnemyPlaneInterval = difficult.spawnEnemyPlaneInterval;
        maxEnemyInTime = difficult.maxEnemyInTime;
        spawnHurricaneInterval = difficult.spawnHurricaneInterval;
        maxHurricaneInTime = difficult.maxHurricaneInTime;
        spawnForestFireInterval = difficult.spawnForestFireInterval;
        maxForestFireInTime = difficult.maxForestFireInTime;
        chanceToLowFuel = difficult.chanceToLowFuel;
    }
}