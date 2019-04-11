using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {

    public bool useGlobalShadow;
    public Vector3 shadowDirect;
    public float shadowLenght;
    public float shadowSize;
    public Color shadowColor;

    private float lenght;

    SpriteRenderer spriteRD;
    SpriteRenderer sd;
    GameObject shadow;

    private void Start () {
        shadow = new GameObject (name + ".Shadow");
        spriteRD = GetComponent<SpriteRenderer> ();
        sd = shadow.AddComponent<SpriteRenderer> ();

        sd.sortingLayerName = "shadow";

        shadow.transform.position = transform.position + shadowDirect * lenght * transform.localScale.x;
        shadow.transform.rotation = transform.rotation;

        shadow.layer = 10;
        if (GlobalShadow.Instance != null && useGlobalShadow) {
            shadowDirect = GlobalShadow.Instance.shadowDirect;
            shadowLenght = GlobalShadow.Instance.shadowLenght;
            shadowSize = GlobalShadow.Instance.shadowSize;
            shadowColor = GlobalShadow.Instance.shadowColor;
        } else {
            sd.color = shadowColor;
        }
        sd.sprite = spriteRD.sprite;
        shadow.transform.localScale = transform.localScale * shadowSize;
        shadow.transform.parent = GameObject.Find ("SHADOW_HOLDER").transform;

    }
    private void Update () {
        shadow.transform.position = transform.position + shadowDirect * lenght * transform.localScale.x;
        shadow.transform.rotation = transform.rotation;
        //shadow.transform.localScale = transform.localScale * shadowSize;
        sd.color = shadowColor;
        lenght = transform.localScale.x * shadowLenght;
        shadow.transform.localScale = shadowSize * transform.localScale;
        //shadowColor.a = spriteRD.color.a;
        //sd.sprite = spriteRD.sprite;
    }

    private void OnDestroy () {
        Destroy (shadow);
    }
}