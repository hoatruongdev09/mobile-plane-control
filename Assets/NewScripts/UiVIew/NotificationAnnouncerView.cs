using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NotificationAnnouncerView : UiView {
    public Button.ButtonClickedEvent ConfirmEvents {
        get { return buttonOK.onClick; }
        set { buttonOK.onClick = value; }
    }
    public Text textTitle;
    public Text textTextContent;
    public Button buttonOK;

    private void Start () {
        buttonOK.onClick = ConfirmEvents;
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