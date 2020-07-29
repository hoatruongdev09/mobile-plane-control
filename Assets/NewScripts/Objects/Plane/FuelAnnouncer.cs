using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class FuelAnnouncer : MonoBehaviour {
    public TextMeshPro text;
    public SpriteRenderer icon;
    public Transform trackTransform;
    private void Update () {
        if (trackTransform) {
            transform.position = trackTransform.position + new Vector3 (0, 5.5f);
        }
    }
    public void Show (string text, Color textColor) {
        gameObject.SetActive (true);
        this.text.text = text;
        this.text.color = icon.color = textColor;
        AnimateShow ();
    }
    private void AnimateShow () {
        LeanTween.cancel (gameObject);
        LeanTween.value (gameObject, 0, 1, .2f).setOnUpdate ((float value) => {
            text.color = new Color (text.color.r, text.color.g, text.color.b, value);
            icon.color = new Color (icon.color.r, icon.color.g, icon.color.b, value);
        });
        LeanTween.value (gameObject, 1, 0, .2f).setOnUpdate ((float value) => {
            text.color = new Color (text.color.r, text.color.g, text.color.b, value);
            icon.color = new Color (icon.color.r, icon.color.g, icon.color.b, value);
        }).setDelay (0.8f).setOnComplete (() => {
            // Debug.Log ("wttttffffff");
            gameObject.SetActive (false);
        });
    }
}