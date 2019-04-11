using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour {
	public float mainWeaponAttackRange = 10;
	public float subWeaponAttackRange = 10;
	[Range (0, 180)]
	public float mainWeaponAngleRange = 30;
	[Range (0, 180)]
	public float subWeaponAngleRange = 30;
	public LayerMask alliedMask;

	public GameObject closestEnemy;
	public GameObject forwardEnemy;

	public Weapon[] mainWeapon;
	public Weapon[] subWeapon;

	public int healthPoint = 100;
	private float randomDelay;
	private Line line;

	private void Start () {
		alliedMask = LayerMask.GetMask ("enemy");
		line = GetComponent<Line> ();
	}
	public void GetDamage (int damage) {
		healthPoint -= damage;
		if (healthPoint <= 50 && healthPoint >= 30) {
			SpawnManager.Instance.SpawnSmokePrefab (transform);
		} else if (healthPoint <= 0) {
			Debug.Log ("Die");
			GetComponent<Plane> ().TriggerGameOver ();
		}
	}
	private void Update () {
		closestEnemy = GetEnemyInRange (subWeaponAttackRange, subWeaponAngleRange);
		forwardEnemy = GetEnemyInRange (mainWeaponAttackRange, mainWeaponAngleRange);
		if (!line.GetIsLanding ()) {
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