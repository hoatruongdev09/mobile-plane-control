using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class AnnouncerView : MonoBehaviour {
    public string Content {
        get {
            return textContent.text;
        }
        set {
            textContent.text = value;
        }
    }
    public CanvasGroup canvas;
    public TextMeshProUGUI textContent;
    public LTDescr AnimateFly (float time) {
        var canvas = FindObjectOfType<CanvasScaler> ();
        float size = canvas.referenceResolution.x;
        float thisSize = (transform as RectTransform).sizeDelta.x;
        return (transform as RectTransform).LeanMoveX (-(size + thisSize), time);

    }
    public LTDescr Show () {
        gameObject.SetActive (true);
        return canvas.LeanAlpha (1, .4f);
    }
    public LTDescr Hide () {
        return canvas.LeanAlpha (0, .4f).setOnComplete (() => {
            Destroy (gameObject);
        });
    }
}