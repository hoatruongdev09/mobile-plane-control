using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour {
    public SpriteRenderer[] planeShape;

    private SpriteRenderer spriteRenderer;
    public float flashTime = 0.3f;

    private void Start () {
        spriteRenderer = GetComponent<SpriteRenderer> ();
        AnimateFlashing ();
        StartCoroutine (DelayToDestroy (flashTime));
    }
    private void AnimateFlashing () {
        LeanTween.value (gameObject, 1, .3f, 1).setOnUpdate ((float value) => {
            spriteRenderer.color = new Color (spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, value);
        }).setLoopPingPong (5).setIgnoreTimeScale (true);
    }
    private IEnumerator DelayToDestroy (float time) {
        yield return new WaitForSecondsRealtime (time);
        Destroy (gameObject);
    }
    public void CreatePlanShape (SpriteRenderer[] shapes, Color baseColor) {
        for (int i = 0; i < shapes.Length; i++) {
            if (i >= planeShape.Length) { break; }
            planeShape[i].sprite = shapes[i].sprite;
            planeShape[i].color = baseColor;
        }
    }
}