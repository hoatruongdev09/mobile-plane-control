using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossSceneData : MonoBehaviour {
    public static CrossSceneData Instance { get; set; }
    public bool IsRemoveAd { get; set; }
    private void Awake () {
        var crossSceneData = FindObjectOfType<CrossSceneData> ();
        if (crossSceneData != this) {
            Destroy (gameObject);
        } else {
            DontDestroyOnLoad (gameObject);
        }
        Instance = this;
    }
}