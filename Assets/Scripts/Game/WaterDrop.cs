using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : MonoBehaviour {

	public ParticleSystem particle;

	public float dropWaterInterval = 10;
	[SerializeField] private float dropWaterTime = 0;

	private void Update () {
		if (dropWaterTime > 0) {
			dropWaterTime -= Time.deltaTime;
		} else {
			dropWaterTime = 0;
		}
	}

	public void DropWater (ForestFire fire) {
		if (dropWaterTime != 0)
			return;
		fire.CoolingMaster (30);
		particle.gameObject.SetActive (false);
		particle.gameObject.SetActive (true);
		dropWaterTime = dropWaterInterval;
	}

}