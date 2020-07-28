using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Gameplay_SoundButton : MonoBehaviour {

    public Image img_disable;
    public string optionTag;
    private void Start () {
        GetComponent<Button> ().onClick.AddListener (OnClickButton);
        img_disable.gameObject.SetActive (PlayerPrefs.GetInt (optionTag, 1) == 1 ? false : true);
        Debug.Log ("start status: " + optionTag + (PlayerPrefs.GetInt (optionTag, 1) == 1 ? false : true));
    }
    public void OnClickButton () {
        if (img_disable.gameObject.activeSelf) {
            img_disable.gameObject.SetActive (false);

        } else {
            img_disable.gameObject.SetActive (true);
        }
        if (optionTag == "sound")
            InGameSoundManager.Instance?.SetPlaySound (!img_disable.gameObject.activeSelf);
        if (optionTag == "music")
            InGameSoundManager.Instance?.SetPlayMusic (!img_disable.gameObject.activeSelf);
    }
}