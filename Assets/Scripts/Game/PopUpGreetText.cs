using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class PopUpGreetText : MonoBehaviour {

	[SerializeField]
	private TextMeshPro textMesh;

	private int randomRotateDirect;

	private void Start () {
		string[] randomSentence = new string[] {
			"Bonjour",
			"Hello",
			"Saluton",
			"Kamusta",
			"Salam",
			"Kaixo",
			"Hola",
			"Welina",
			"velkominn",
			"Selamat datang",
			"Benvenuta",
			"Bem vinda",
			"Bine ati venit",
			"Amohela",
			"Bienvenida",
			"Xin chao"
		};
		textMesh = GetComponent<TextMeshPro> ();
		textMesh.text = randomSentence[Random.Range (0, randomSentence.Length)];
		randomRotateDirect = Random.Range (-1, 2);
	}
	private void Update () {
		transform.Rotate (0, 0, 30 * randomRotateDirect * Time.deltaTime);
		transform.localScale = Vector3.Lerp (transform.localScale, new Vector3 (2.5f, 2.5f, 1), 1.5f * Time.deltaTime);
		textMesh.color = new Color (textMesh.color.r, textMesh.color.g, textMesh.color.b, textMesh.color.a - 1.5f * Time.deltaTime);
		if (textMesh.color.a <= 0.02f) {
			Destroy (gameObject);
		}
	}

}