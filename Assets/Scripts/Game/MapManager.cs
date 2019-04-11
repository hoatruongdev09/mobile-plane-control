using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

	public static MapManager Instance { get; private set; }

	public float colliderOffset = 2f;
	public float borderOffset = 3f;
	public float mapOffset = 15;
	public EdgeCollider2D borderCollider;
	public SpriteRenderer mapBackground;
	private Vector2 mapSize = new Vector2 (10, 10);
	private Vector2 trueScreenSize;
	private EdgeCollider2D edgeCollider2D;

	private Vector2 lastRandomPosition;
	private void Start () {
		Instance = this;
		AutoConfigSize ();
		SetUpEdgeCollider ();
	}
	private void AutoConfigSize () {
		Camera cam = Camera.main;
		float sizeY = 2f * cam.orthographicSize;
		float sizeX = sizeY * cam.aspect;
		trueScreenSize = new Vector2 (sizeX, sizeY);
		mapSize = trueScreenSize + new Vector2 (mapOffset, mapOffset);
		mapBackground.size = mapSize;
	}
	private void SetUpEdgeCollider () {
		edgeCollider2D = GetComponent<EdgeCollider2D> ();
		edgeCollider2D.edgeRadius = 2;
		float halfRadius = edgeCollider2D.edgeRadius / 2 + colliderOffset;
		List<Vector2> edgePoints = new List<Vector2> ();
		edgePoints.Add (new Vector2 (trueScreenSize.x / 2 + halfRadius, trueScreenSize.y / 2 + halfRadius));
		edgePoints.Add (new Vector2 (trueScreenSize.x / 2 + halfRadius, -trueScreenSize.y / 2 - halfRadius));
		edgePoints.Add (new Vector2 (-trueScreenSize.x / 2 - halfRadius, -trueScreenSize.y / 2 - halfRadius));
		edgePoints.Add (new Vector2 (-trueScreenSize.x / 2 - halfRadius, trueScreenSize.y / 2 + halfRadius));
		edgePoints.Add (new Vector2 (trueScreenSize.x / 2 + halfRadius, trueScreenSize.y / 2 + halfRadius));
		edgeCollider2D.points = edgePoints.ToArray ();
		List<Vector2> border = new List<Vector2> ();
		border.Add (new Vector2 (trueScreenSize.x / 2 - borderOffset, trueScreenSize.y / 2 - borderOffset));
		border.Add (new Vector2 (trueScreenSize.x / 2 - borderOffset, -trueScreenSize.y / 2 + borderOffset));
		border.Add (new Vector2 (-trueScreenSize.x / 2 + borderOffset, -trueScreenSize.y / 2 + borderOffset));
		border.Add (new Vector2 (-trueScreenSize.x / 2 + borderOffset, trueScreenSize.y / 2 - borderOffset));
		border.Add (new Vector2 (trueScreenSize.x / 2 - borderOffset, trueScreenSize.y / 2 - borderOffset));
		borderCollider.points = border.ToArray ();
	}
	public Vector3 RandomSpawnPos () {

		float randomX = 0;
		float randomY = 0;
		while ((new Vector2 (randomX, randomY) - lastRandomPosition).sqrMagnitude <= 9) {
			bool isHorizon = (Random.Range (0, 2) == 1);
			if (isHorizon) {
				randomX = Random.Range (-mapSize.x / 2, mapSize.x / 2);
				float yFactor = (Random.Range (0, 2) == 1) ? 1 : -1;
				randomY = yFactor * mapSize.y / 2;
			} else {
				randomY = Random.Range (-mapSize.y / 2, mapSize.y / 2);
				float xFactor = (Random.Range (0, 2) == 1) ? 1 : -1;
				randomX = xFactor * mapSize.x / 2;
			}
		}
		return new Vector2 (randomX, randomY);
	}
	public Vector3 GetCloudRandomPosition () {
		float randomX = 0;
		float randomY = 0;
		randomY = Random.Range (-trueScreenSize.y / 2, trueScreenSize.y / 2);
		randomX = -mapSize.x / 2;
		return new Vector3 (randomX, randomY);
	}
	public Vector3 GetHurricaneRandomPosition () {

		float randomX = Random.Range (-(trueScreenSize.x - mapOffset) / 2, (trueScreenSize.x - mapOffset) / 2);
		float randomY = Random.Range (-(trueScreenSize.y - mapOffset) / 2, (trueScreenSize.y - mapOffset) / 2);

		return new Vector2 (randomX, randomY);
	}
	public Vector3 GetRandomPosition () {
		float randomY = Random.Range (-trueScreenSize.y / 2 + mapOffset, trueScreenSize.y / 2 - mapOffset);
		float randomX = Random.Range (-trueScreenSize.x / 2 + mapOffset, trueScreenSize.x / 2 - mapOffset);
		return new Vector2 (randomX, randomY);
	}

	private void OnDrawGizmos () {
		Gizmos.DrawWireCube (new Vector2 (0, 0), mapSize);

	}
}