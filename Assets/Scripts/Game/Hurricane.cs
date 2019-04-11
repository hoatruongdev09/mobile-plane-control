using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurricane : MonoBehaviour {
    public float moveSpeed = 10f;
    public float lifeTime = 15f;
    public float distanceToChangePosition = 2;
    private Vector3 randomPosition;

    private void Start () {
        randomPosition = MapManager.Instance.GetRandomPosition ();
        StartCoroutine (RisingUp ());
        StartCoroutine (DelayToDie ());
    }
    private void Update () {
        if ((randomPosition - transform.position).sqrMagnitude <= Mathf.Pow (distanceToChangePosition, 2)) {
            randomPosition = MapManager.Instance.GetRandomPosition ();
        }
        transform.Translate ((randomPosition - transform.position).normalized * Time.deltaTime);
    }
    private IEnumerator DelayToDie () {
        yield return new WaitForSeconds (lifeTime - 5);
        Destroy (GetComponent<CircleCollider2D> ());
        while (transform.localScale.x > 0.01f) {
            transform.localScale = Vector3.Lerp (transform.localScale, Vector3.zero, Time.deltaTime);
            yield return null;
        }
        Debug.Log ("hurricane disappeared");
        Destroy (gameObject);
    }
    private IEnumerator RisingUp () {
        transform.localScale = Vector3.zero;
        SpawnManager.Instance.SpawnWarningSign (transform.position, 2);
        yield return new WaitForSeconds (2);
        while (transform.localScale.x <= 0.99f) {
            transform.localScale = Vector3.Lerp (transform.localScale, Vector3.one, Time.deltaTime);
            yield return null;
        }
        transform.localScale = Vector3.one;
    }
}