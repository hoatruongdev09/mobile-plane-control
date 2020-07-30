using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Cloud : MonoBehaviour {
    public OnDisappear onDisappear { get; set; }
    public float LifeTime { get; set; }
    public float floatSpeed = 3;
    public SpriteRenderer[] graphics;
    public delegate void OnDisappear (Cloud cloud);
    private void Start () {
        Appear ();
        floatSpeed += Random.Range (-0.05f, 0.1f);
        StartCoroutine (DelayToDisappear (LifeTime));
    }
    private void Update () {
        transform.Translate (Vector2.right * floatSpeed * Time.smoothDeltaTime);
    }

    private void Appear () {
        LeanTween.value (gameObject, 0, .9f, 1f).setOnUpdate ((float value) => {
            foreach (var graphic in graphics) {
                graphic.color = new Color (graphic.color.r, graphic.color.g, graphic.color.b, value);
            }
        });
    }
    private void Disappear () {
        LeanTween.value (gameObject, .9f, 0, 1f).setOnUpdate ((float value) => {
            foreach (var graphic in graphics) {
                graphic.color = new Color (graphic.color.r, graphic.color.g, graphic.color.b, value);
            }
        }).setOnComplete (() => {
            onDisappear?.Invoke (this);
            // Destroy (gameObject);
        });
    }
    private IEnumerator DelayToDisappear (float time) {
        yield return new WaitForSeconds (time);
        Disappear ();
    }
    public void DestroySelf () {
        Destroy (gameObject);
    }
}