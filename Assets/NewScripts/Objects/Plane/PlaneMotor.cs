using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMotor : MonoBehaviour {
    public Vector3 rotateDirect;
    public float rotateSpeed;

    private void Update () {
        transform.Rotate (rotateDirect * rotateSpeed * Time.deltaTime, Space.Self);
    }
}