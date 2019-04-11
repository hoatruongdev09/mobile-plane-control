using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurricaneDust : MonoBehaviour {

	public float rotateSpeed = 30;

	private void Update () {
		transform.Rotate (0, 0, -rotateSpeed * Time.deltaTime);
	}
}