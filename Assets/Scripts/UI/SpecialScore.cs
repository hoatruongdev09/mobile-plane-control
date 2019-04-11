using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpecialScore : MonoBehaviour {
	public Image image;
	public Text text;

	public void SetDetail (Sprite image, string text) {
		this.image.sprite = image;
		this.text.text = text;
	}
}