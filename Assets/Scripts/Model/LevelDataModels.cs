using System;
using UnityEngine;

[Serializable]
public class LevelDataModel {
    public string background;
    public string levelBackground;
    public bool enemies = false;
    public bool fire = false;
    public bool cloud = false;
    public bool tornado = false;
    public bool fuel = false;
    public AirportDataModel[] airport;
    public int[] planeId;
}

[Serializable]
public class LevelDifficultData {
    public float planeCreateInterval = 15;
    public float maxPlaneInTime = 10;
    public float cloudCreateInterval = 8;
    public float maxCloudInTime = 4;
    public float enemyCreateInterval = 20;
    public float maxEnemyInTime = 3;
    public float createTornadoInterval = 8;
    public float maxTornadoInTime = 3;
    public float createFireInterval = 10;
    public float maxFireInTime = 5;
    public float lowFuelChance = 25;
    public float fuelTimeRangeMin = 15;
    public float fuelTimeRangeMax = 20;
}

[Serializable]
public class AirportDataModel {
    public string name;
    public string type;
    public VectorModel position;
    public float rotation;
    public string planeTag;
    public string color;
    public VectorModel scale;
}

[Serializable]
public class VectorModel {
    public int x;
    public int y;
    public Vector2 ToVector2 () {
        return new Vector2 (x, y);
    }
    public Vector3 ToVector3 () {
        return new Vector3 (x, y);
    }
}