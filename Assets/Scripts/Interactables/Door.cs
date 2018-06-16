using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {

	private Sprite sprite;
	private bool open;
	private int count;

	private const string PLAYER_TAG = "Player";
	private const string NPC_TAG = "NPC";

	void Start () {
		sprite = GetComponent<SpriteRenderer>().sprite;
		open = false;
	}
	
	protected virtual void OnTriggerEnter2D(Collider2D other) {
		// different floors so don't interact
		if (other.transform.position.z != transform.position.z) {
			return;
		}

		count++;
		if (open) {
			return;
		}

		open = true;
		GetComponent<SpriteRenderer>().sprite = null;
		ForceCharacterToPause(other);
	}

	protected virtual void OnTriggerExit2D(Collider2D other) {
		count--;
		if (count == 0) {
			GetComponent<SpriteRenderer>().sprite = sprite;
			open = false;
		}
	}

	private void ForceCharacterToPause(Collider2D other) {
		if (other.gameObject.CompareTag (PLAYER_TAG)) {
			other.gameObject.GetComponent<Player>().StartDoorDelay();
		} else if (other.gameObject.CompareTag (NPC_TAG)) {
			other.gameObject.GetComponent<NPC>().StartDoorDelay();
		}
	}
}
