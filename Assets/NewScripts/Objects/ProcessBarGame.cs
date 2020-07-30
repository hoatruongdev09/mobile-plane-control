using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcessBarGame : MonoBehaviour {
    public float Percent {
        get { return currentPercent; }
        set {
            currentPercent = value;
            processBar.transform.localScale = new Vector3 (currentPercent * maxScale, processBar.transform.localScale.y);
        }
    }
    public SpriteRenderer holder;
    public SpriteRenderer processBar;
    private float maxScale;
    private float currentPercent;
    public void Start () {
        maxScale = processBar.transform.localScale.x;
    }
    public void SetProcessBarColor (Color color) {
        processBar.color = color;
    }
}