using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestFire : MonoBehaviour {

	public float hp = 100;
	private bool isDisappear;
	private void Update () {
		transform.localScale = Vector2.Lerp (transform.localScale, Vector2.one * hp / 100, .1f * Time.deltaTime);
	}
	public void CoolingMaster (float cool) {
		hp -= cool;
		if (hp < 10) {
			Destroy (gameObject);
		}
	}

	private IEnumerator Disappear () {
		float scaleX = transform.localScale.x;
		isDisappear = true;
		while (scaleX >= 0) {
			scaleX = Mathf.Lerp (scaleX, -1, .1f * Time.deltaTime);
			transform.localScale = scaleX * transform.localScale;
			yield return null;
		}
		Destroy (gameObject);
	}
}