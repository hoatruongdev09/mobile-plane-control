using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelLoadIndicator : UiView {
    public Image imgLoading;
    public override void Show () {
        gameObject.SetActive (true);
        canvasGroup.interactable = true;
        AnimateShow (() => {
            AnimateImgLoading ();
        });
    }
    public override void Hide () {
        AnimateHide (() => {
            gameObject.SetActive (false);
            canvasGroup.interactable = false;
            LeanTween.cancel (imgLoading.gameObject);
            imgLoading.fillAmount = 0;
        });
    }

    private void AnimateImgLoading () {
        LeanTween.value (imgLoading.gameObject, 0, 359, .5f).setOnUpdate ((float value) => {
            imgLoading.transform.rotation = Quaternion.Euler (0, 0, value);
        }).setLoopClamp ();
        LeanTween.value (imgLoading.gameObject, 0, 1, 1f).setOnUpdate ((float value) => {
                imgLoading.fillAmount = value;
            })
            .setLoopPingPong ()
            .setOnCompleteOnRepeat (true)
            .setOnComplete (() => { imgLoading.fillClockwise = !imgLoading.fillClockwise; })
            .setEaseInOutSine ();
    }
}