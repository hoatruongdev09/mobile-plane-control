using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HighScoreView : UiView {
    public Text textHighScore;

    protected override LTDescr AnimateShow (System.Action callback) {
        return base.AnimateShow (callback);
    }
}