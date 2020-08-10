using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonMapSelect : MonoBehaviour {
    public Image[] mapImages;
    public Image lockedImage;
    public Text mapName;
    public Image[] starts;

    public void SetStars (int number) {
        for (int i = 0; i < starts.Length; i++) {
            if (i < number) {
                starts[i].gameObject.SetActive (true);
            } else {
                starts[i].gameObject.SetActive (false);
            }
        }
    }
}