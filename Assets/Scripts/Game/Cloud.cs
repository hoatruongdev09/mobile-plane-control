using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Cloud : MonoBehaviour {
    public float floatSpeed = 3;
    private void Start () {
        floatSpeed += Random.Range (-0.05f, 0.1f);
        Destroy (gameObject, 60f);
    }
    private void Update () {
        transform.Translate (SpawnManager.Instance.windDirection * floatSpeed);
    }
}