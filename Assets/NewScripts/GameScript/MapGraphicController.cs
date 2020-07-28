using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraphicController : MonoBehaviour {
    public SpriteRenderer mapBackground;
    public SpriteRenderer levelBackground;

    public void SetMapBackground (Sprite sprite) {
        mapBackground.sprite = sprite;
    }
    public void SetLevelBackground (Sprite sprite) {
        levelBackground.sprite = sprite;
    }
}