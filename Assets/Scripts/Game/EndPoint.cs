using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndPoint : MonoBehaviour {

    public Line line;

    public void SetLine (Line _line) {
        line = _line;
    }
    public Line GetLine () {
        return line;
    }
    private void Update () {
        if (line == null) {
            Destroy (gameObject);
        }
    }

}