using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour {
    public SpriteRenderer planeShape;

    private SpriteRenderer spriteRenderer;
    public float flashTime = 0.3f;

    public float counting = .3f;

    private void Start () {
        Destroy (gameObject, 2f);
        spriteRenderer = GetComponent<SpriteRenderer> ();
        counting = flashTime;
    }

    private void Update () {
        if (counting <= 0) {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            planeShape.enabled = !planeShape.enabled;
            counting = flashTime;
        } else {
            counting -= Time.deltaTime;
        }
    }
}