using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneFuelComponent : MonoBehaviour, IPlaneComponent {
    public OnOutOfFuel outOfFuel { get; set; }
    public float StartFuel { get; set; }
    public Color normalColor = new Color32 (44, 62, 80, 255);
    public Color warningColor = new Color32 (243, 156, 48, 255);
    [SerializeField] private float currentFuel = 0;
    [SerializeField] private bool isCounting = true;
    [SerializeField] private FuelAnnouncer fuelAnnouncer;
    public delegate void OnOutOfFuel ();
    public void AttachToPlane (PlaneControl plane) {
        plane.Components?.Add (this);
        outOfFuel += plane.OnOutOfFuel;
        fuelAnnouncer = SpawnController.Instance.CreateFuelAnnouncer ();
        fuelAnnouncer.trackTransform = plane.transform;
    }

    public void Effect (PlaneControl plane) {

    }
    private void Update () {
        if (!isCounting) { return; }
        if (currentFuel >= StartFuel) {
            isCounting = false;
            outOfFuel?.Invoke ();
            return;
        }
        currentFuel += Time.deltaTime;
        if (fuelAnnouncer && !fuelAnnouncer.gameObject.activeSelf) {
            var remainFuel = StartFuel - currentFuel;
            fuelAnnouncer.Show (string.Format ("{0:0}", remainFuel), (remainFuel < 10) ? warningColor : normalColor);
        }
    }
    private void OnDestroy () {
        Destroy (fuelAnnouncer.gameObject);
    }

    public void UpdateEffect (PlaneControl plane) {

    }
}