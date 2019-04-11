using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public float moveSpeed = 2f;
    public float slowSpeed = 10f;
    public int damage = 1;
    public float liveTime;
    public GameObject target;
    private void Start () {
        StartCoroutine (DelayToDestroy ());
    }

    private void Update () {
        Move ();
    }
    protected virtual void Move () {
        transform.Translate (Vector2.up * moveSpeed * Time.deltaTime);
    }
    protected virtual IEnumerator DelayToDestroy () {
        yield return new WaitForSeconds (liveTime);
        Destroy (gameObject);
    }
    protected virtual void CollidedAction () {

    }
    private void OnTriggerEnter2D (Collider2D other) {
        CollidedAction ();
        if (this.tag == "plane" && other.tag == "enemy") {
            other.GetComponent<Enemy> ().GetDamage (damage);
            Destroy (gameObject);
        }
        if (this.tag == "enemy" && other.tag == "plane") {
            other.GetComponent<Ally> ().GetDamage (damage);
            Destroy (gameObject);
        }
    }
}