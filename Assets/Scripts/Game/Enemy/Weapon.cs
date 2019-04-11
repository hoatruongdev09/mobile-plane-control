using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
	public GameObject bulletPrefab;
	public Transform bulletSpawn;
	public float fireRate = .1f;
	public int clipSize = 30;
	public float reloadTime = 5;
	public string bulletColliderTag;

	protected int currentBulletNumber;
	protected float cooldown;
	protected bool isReloading;
	private void Start () {
		bulletColliderTag = transform.parent.tag;
		currentBulletNumber = clipSize;

	}
	private void Update () {
		if (cooldown > 0) {
			cooldown -= Time.deltaTime;
		} else {
			cooldown = 0;
		}
	}

	public virtual void Fire (GameObject target) {
		if (cooldown != 0)
			return;
		if (currentBulletNumber <= 0) {
			return;
		}
		currentBulletNumber--;
		GameObject bulletGO = Instantiate (bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
		bulletGO.tag = bulletColliderTag;
		bulletGO.GetComponent<BoxCollider2D> ().isTrigger = true;
		bulletGO.GetComponent<Bullet> ().target = target;
		cooldown = fireRate;
		bulletGO.transform.parent = SpawnManager.Instance.BULLET_HOLDER.transform;
		if (currentBulletNumber <= 0 && !isReloading) {
			StartCoroutine (Reload ());
		}
	}

	protected IEnumerator Reload () {
		float reload = reloadTime + Random.Range (0, reloadTime / 2);
		isReloading = true;
		yield return new WaitForSeconds (reload);
		isReloading = false;
		currentBulletNumber = clipSize;
	}

}