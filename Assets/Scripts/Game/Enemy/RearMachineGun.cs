using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RearMachineGun : Weapon {
	public override void Fire (GameObject target) {
		if (cooldown != 0) {
			return;
		}
		if (currentBulletNumber <= 0 && !isReloading) {
			StartCoroutine (base.Reload ());
		}
		if (currentBulletNumber <= 0) {
			return;
		}

		currentBulletNumber--;
		transform.rotation = Quaternion.Euler (0, 0, CalculationAngle ((target.transform.position - transform.position).normalized));
		GameObject bulletGO = Instantiate (bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
		bulletGO.tag = bulletColliderTag;
		bulletGO.GetComponent<BoxCollider2D> ().isTrigger = true;
		bulletGO.GetComponent<Bullet> ().target = target;
		cooldown = fireRate;
		bulletGO.transform.parent = SpawnManager.Instance.BULLET_HOLDER.transform;
	}
	private float CalculationAngle (Vector3 direction) {
		return -Mathf.Atan2 (direction.x, direction.y) * Mathf.Rad2Deg;
	}
}