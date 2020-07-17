using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class UiView : MonoBehaviour {
    public CanvasGroup canvasGroup;
    public virtual void Show () {
        gameObject.SetActive (true);
        AnimateShow (() => { });
    }
    public virtual void Hide () {
        AnimateHide (() => {
            gameObject.SetActive (false);
        });
    }
    public virtual void Show (Action onShowCallback) {
        gameObject.SetActive (true);
        AnimateShow (onShowCallback);
    }
    public virtual void Hide (Action onHideCallback) {
        AnimateHide (() => {
            onHideCallback ();
            gameObject.SetActive (false);
        });
    }
    protected virtual void AnimateShow (System.Action callback) {
        canvasGroup.LeanAlpha (1, .5f).setOnComplete (callback).setIgnoreTimeScale (true);
    }
    protected virtual void AnimateHide (Action callback) {
        canvasGroup.LeanAlpha (0, .5f).setOnComplete (callback).setIgnoreTimeScale (true);
    }
}