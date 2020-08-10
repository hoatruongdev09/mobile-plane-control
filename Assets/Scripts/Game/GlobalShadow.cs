using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalShadow : MonoBehaviour {
    public static GlobalShadow Instance { get; private set; }
    public Vector3 ShadowDirect {
        get { return shadowDirect; }
    }
    public float ShadowSize {
        get { return shadowSize; }
    }
    public float ShadowLength {
        get { return shadowLenght; }
    }
    public Vector3 shadowDirect = new Vector3 (-1, -1);
    public float shadowSize = 0.6f;
    public float shadowLenght = 1f;
    public Color shadowColor = new Color32 (12, 120, 111, 250);

    private void Start () {
        Instance = this;
    }

}