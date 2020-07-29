using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireForest : MonoBehaviour {
    public OnFireCooledDown onFireCooledDown;
    public float MaxHP = 110;
    public float HP = 10;
    private bool destroyed = false;
    public delegate void OnFireCooledDown (FireForest fire);
    private void Update () {
        if (destroyed) { return; }
        HP += 0.5f * Time.deltaTime;
        HP = Mathf.Clamp (HP, 0, MaxHP);
        transform.localScale = Vector3.one * (HP / MaxHP);
    }
    public void CoolOut (float cool) {
        if (destroyed) { return; }
        HP -= cool;
        if (HP <= 0) {
            destroyed = true;
            onFireCooledDown?.Invoke (this);
        }
    }
    public void DestorySelf () {
        Destroy (gameObject);
    }
}