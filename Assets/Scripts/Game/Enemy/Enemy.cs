using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Enemy : MonoBehaviour {

    public float mainWeaponAttackRange = 10;
    public float subWeaponAttackRange = 10;
    [Range (0, 180)]
    public float mainWeaponAngleRange = 30;
    [Range (0, 180)]
    public float subWeaponAngleRange = 30;
    public float distanceToChangeDirect = 2;
    public LayerMask alliedMask;
    private Vector3 destination;
    private Line line;

    public GameObject closestEnemy;
    public GameObject forwardEnemy;

    public Weapon[] mainWeapon;
    public Weapon[] subWeapon;
    public int healthPoint = 100;
    private float randomDelay;
    private void Start () {
        alliedMask = LayerMask.GetMask ("plane");
        line = GetComponent<Line> ();
        line.canDraw = false;
    }
    public void GetDamage (int damage) {
        healthPoint -= damage;
        if (healthPoint <= 50 && healthPoint >= 30) {
            SpawnManager.Instance.SpawnSmokePrefab (transform);
        } else if (healthPoint <= 0) {
            Debug.Log ("Die");
            // score this plane
            SpawnManager.Instance.SpawnBlowEffect (transform.position);
            ScoreManager.Instance.ScoreSpecial ("enemy", 1);
            Destroy (gameObject);
        }
    }
    private void Update () {
        if ((transform.position - destination).sqrMagnitude <= Mathf.Pow (distanceToChangeDirect, 2)) {
            Debug.Log ("Change direction");
            destination = MapManager.Instance.GetHurricaneRandomPosition ();
            line.SetLookDirect ((destination - transform.position).normalized);
        }
        closestEnemy = GetEnemyInRange (subWeaponAttackRange, subWeaponAngleRange);
        forwardEnemy = GetEnemyInRange (mainWeaponAttackRange, mainWeaponAngleRange);

        if (forwardEnemy != null) {
            foreach (Weapon wp in mainWeapon) {
                wp.Fire (forwardEnemy);
            }
        }
        if (closestEnemy != null) {
            foreach (Weapon wp in subWeapon) {
                wp.Fire (closestEnemy);
            }
        }
    }
    private GameObject GetEnemyInRange (float range, float angle) {
        Collider2D[] col = Physics2D.OverlapCircleAll (transform.position, range, alliedMask);
        float angleToObj = 0;
        foreach (Collider2D obj in col) {
            angleToObj = Vector3.Angle (transform.up, obj.transform.position - transform.position);
            if (angleToObj <= angle) {
                return obj.gameObject;
            }
        }
        return null;
    }
    private GameObject GetEnemyForward () {
        Collider2D[] col = Physics2D.OverlapCircleAll (transform.position, mainWeaponAttackRange, alliedMask);
        foreach (Collider2D obj in col) {
            if (obj.gameObject != this.gameObject) {
                float angle = Vector3.Angle (transform.up, obj.transform.position - transform.position);
                //Debug.Log ("angle : " + angle);
                if (angle <= mainWeaponAngleRange) {
                    return obj.gameObject;
                }
            }
        }
        return null;
    }
    private GameObject GetEnemyClosest () {
        Collider2D[] col = Physics2D.OverlapCircleAll (transform.position, mainWeaponAttackRange, alliedMask);
        GameObject closestEnemy = null;
        foreach (Collider2D obj in col) {
            if (obj.gameObject != this.gameObject) {
                if (closestEnemy == null) {
                    closestEnemy = obj.gameObject;
                } else {
                    if ((obj.transform.position - transform.position).sqrMagnitude < (closestEnemy.transform.position - transform.position).sqrMagnitude) {
                        closestEnemy = obj.gameObject;
                    }
                }
            }
        }
        return closestEnemy;
    }
    public void SetDestination (Vector3 destination) {
        this.destination = destination;
    }
    private void OnDrawGizmos () {
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay (transform.position, transform.up * mainWeaponAttackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay (transform.position, new Vector2 (transform.up.y, -transform.up.x) * subWeaponAttackRange);
        Gizmos.color = Color.red;
        if (forwardEnemy != null)
            Gizmos.DrawRay (transform.position, forwardEnemy.transform.position - transform.position);
    }

}