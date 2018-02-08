using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Tilemaps;

public abstract class MovingObject : MonoBehaviour {

	public LayerMask blockingLayer;
	public GameObject doorTilemapGameObj;

	private Tilemap doorTilemap;

	private Rigidbody2D rb2D;
	private bool paused = false;

	private const string DOOR = "Door";
	private const float DOOR_DELAY_SECONDS = 0.02f;
	private Tile currentDoorTile;
	private Vector3Int currentDoorPos;

	IEnumerator doorDelay(Collision2D collision) {
		paused = true;
		yield return new WaitForSeconds(DOOR_DELAY_SECONDS);
		paused = false;
		Vector3 hitPosition = Vector3.zero;
		foreach (ContactPoint2D hit in collision.contacts) {
			hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
			hitPosition.y = hit.point.y - 0.01f * hit.normal.y;

			currentDoorPos = doorTilemap.WorldToCell(hitPosition);
			currentDoorTile = doorTilemap.GetTile<Tile>(currentDoorPos);
		}
	}

	void Awake() {
		rb2D = GetComponent<Rigidbody2D>();
		rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb2D.interpolation = RigidbodyInterpolation2D.Extrapolate;
		rb2D.freezeRotation = true;

		if (doorTilemapGameObj != null) {
			doorTilemap = doorTilemapGameObj.GetComponent<Tilemap>();
		}
	}

	// Use this for initialization
	protected virtual void Start () {
	}

	protected abstract void FixedUpdate ();

	protected void Move(Vector3 movement) {
		if (!paused) {
			rb2D.velocity = movement;
		} else {
			rb2D.velocity = new Vector3(0, 0, 0);
		}
	}

	protected void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject == doorTilemapGameObj) {
			//StartCoroutine(doorDelay(collision));
			Vector3 hitPosition = Vector3.zero;
			foreach (ContactPoint2D hit in collision.contacts) {
				hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
				hitPosition.y = hit.point.y - 0.01f * hit.normal.y;

				currentDoorPos = doorTilemap.WorldToCell(hitPosition);
				currentDoorTile = doorTilemap.GetTile<Tile>(currentDoorPos);
				doorTilemap.SetTile (currentDoorPos, null);
			}
		}
	}

	protected void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.CompareTag(DOOR)) {
			Debug.Log (currentDoorTile.name);
			doorTilemap.SetTile (currentDoorPos, currentDoorTile);
		}
	}
}
