using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWaterComponent : MonoBehaviour, IPlaneComponent {

    public float MaxWater { get; set; }
    private float currentWater;
    private ProcessBarGame processBar;
    private NewWaterDrop waterDrop;
    public void AttachToPlane (PlaneControl plane) {
        plane.Components?.Add (this);
        processBar = SpawnController.Instance.CreateProcessBar (transform);
        waterDrop = SpawnController.Instance.CreateWaterDrop (transform);
        waterDrop.transform.localPosition = Vector3.zero;
    }
    private void Update () {
        processBar.transform.position = transform.position + new Vector3 (0, -4.5f);
        processBar.transform.rotation = Quaternion.identity;
    }

    public void Effect (PlaneControl plane) {

    }
    private void OnTriggerExit2D (Collider2D other) {
        if (other.tag == "forestfire") {
            waterDrop.Stop ();
        }
    }
    private void OnTriggerStay2D (Collider2D other) {
        if (other.tag == "forestfire") {
            if (currentWater >= MaxWater) {
                return;
            }
            var fire = other.GetComponent<FireForest> ();
            fire.CoolOut (5 * Time.deltaTime);
            waterDrop.Drop ();
            currentWater += Time.deltaTime;
            currentWater = Mathf.Clamp (currentWater, 0, MaxWater);
            processBar.Percent = (MaxWater - currentWater) / MaxWater;
        }
    }

    public void UpdateEffect (PlaneControl plane) {

    }
}