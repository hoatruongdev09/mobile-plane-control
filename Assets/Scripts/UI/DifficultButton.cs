using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DifficultButton : MonoBehaviour {
	public string content;
	private Image image;

	private Color originColor;
	private void Start () {
		image = GetComponent<Image> ();
		originColor = image.color;
		GetComponent<Button> ().onClick.AddListener (Active);
	}
	public void Active () {
		foreach (DifficultButton button in FindObjectsOfType<DifficultButton> ()) {
			button.Deactive ();
		}
		image.color = originColor;
		transform.localScale = new Vector3 (1.2f, 1.2f, 1);
		PlayerPrefs.SetString ("difficult", content);
	}
	public void Deactive () {
		image.color = new Color32 (128, 142, 155, 255);
		transform.localScale = Vector3.one;
	}

}