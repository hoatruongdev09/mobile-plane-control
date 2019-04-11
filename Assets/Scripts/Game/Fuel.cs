using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel : MonoBehaviour {

    public float timeRemain;
    public bool outOfFuel;

    private Line line;
    private bool delayed;
    private void Start () {
        spawnPos = gameObject.transform.position;
        line = GetComponent<Line> ();
        string planetag = line.planeTag;
        AirPort thisAirport = null;
        foreach (AirPort ap in SpawnManager.Instance.airports) {
            if (ap.planeTag == planetag) {
                thisAirport = ap;
                break;
            }
        }
        if (thisAirport) {
            float distance = (thisAirport.transform.position - transform.position).magnitude;
            timeRemain = distance / GetComponent<Line> ().moveSpeed + Random.Range (10, 12);
        }
    }

    private void Update () {
        if (timeRemain >= 0) {
            timeRemain -= Time.deltaTime;
            if (!delayed && !line.GetIsLanding ()) {
                delayed = true;
                StartCoroutine (ShowFuelAfter (1f));
            }
        } else {
            timeRemain = 0;
            if (!outOfFuel && !line.GetIsLanding ()) {
                outOfFuel = true;
                GetComponent<Line> ().FallDown ();
            }
        }
    }
    private Vector3 spawnPos;
    private IEnumerator ShowFuelAfter (float duration) {

        InGameUIControl.Instance.ShowTextFuel (((int) timeRemain).ToString (), spawnPos);
        yield return new WaitForSeconds (duration);
        spawnPos = gameObject.transform.position;
        delayed = false;

    }

}