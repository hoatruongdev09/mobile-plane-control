using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PanelQuitConfirm : MonoBehaviour {
    public Button buttonYes;
    public Button buttonNo;
    public CanvasGroup canvasGroup;

    private void Start () {
        buttonNo.onClick.AddListener (ButtonNo);
        buttonYes.onClick.AddListener (ButtonYes);
    }
    public void Show () {
        gameObject.SetActive (true);
        canvasGroup.LeanAlpha (1, .5f).setEaseInSine ();
    }
    public void Hide () {
        canvasGroup.LeanAlpha (0, 1f).setEaseInSine ().setOnComplete (() => {
            gameObject.SetActive (false);
        });
    }
    private void ButtonYes () {
        Application.Quit ();
    }
    private void ButtonNo () {
        Hide ();
    }
}