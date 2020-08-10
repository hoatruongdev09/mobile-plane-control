using System;
using UnityEngine;

[Serializable]
public class LevelDataModel {
    public LevelDataInfo info;
    public MapImageModel levelBackground;
    public MapImageModel sceneBackground;
    public bool enemies = false;
    public bool fire = false;
    public bool cloud = false;
    public bool tornado = false;
    public bool fuel = false;
    public AirportDataModel[] airport;
    public int[] planeId;
    public int[] enemyId;
}

[Serializable]
public class MapImageModel {
    public string background;
    public VectorModel position;
    public VectorModel scale;
    public float rotation;
}

[Serializable]
public class LevelLoadOption {
    public string sceneLoading = "default";
}

[Serializable]
public class LevelDataInfo {
    public string id;
    public string name;
    public string levelImage;
    public int difficult;
    public int unlock = 150;
    public int unlockType = 0;
    public LevelLoadOption loadOption;

    public enum UnlockType { landed, adsWatched, buy }
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
    public float waterChance = 25;
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
    public float x;
    public float y;
    public Vector2 ToVector2 () {
        return new Vector2 (x, y);
    }
    public Vector3 ToVector3 () {
        return new Vector3 (x, y);
    }
    public static VectorModel FromVector3 (Vector3 vector) {
        return new VectorModel () { x = vector.x, y = vector.y };
    }
    public static VectorModel FromVector2 (Vector2 vector) {
        return new VectorModel () { x = vector.x, y = vector.y };
    }
}