using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGraphicController : MonoBehaviour {
    public SpriteRenderer mapBackground;
    public SpriteRenderer sceneBackground;

    public void SetMapBackground (Sprite sprite) {
        mapBackground.sprite = sprite;
    }
    public void SetSceneBackground (Sprite sprite) {
        sceneBackground.sprite = sprite;
    }
}