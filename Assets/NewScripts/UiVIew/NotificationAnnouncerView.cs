using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NotificationAnnouncerView : UiView {
    public Text textTitle;
    public Text textTextContent;
    public Button buttonOK;

    private void Start () {
        buttonOK.onClick.AddListener (ButtonOK);
    }

    private void ButtonOK () {
        Hide ();
    }
    public override void Hide () {
        AnimateHide (() => {
            Destroy (gameObject);
        });
    }

}