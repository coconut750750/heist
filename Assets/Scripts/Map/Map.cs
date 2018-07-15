using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

	[SerializeField]
	private GameObject fade;

	private Building[] buildings;

	void Awake() {
		buildings = GameObject.FindObjectsOfType<Building>();
	}

	public void SetGroundFloorActive (bool active) {
    	foreach (Collider2D c in GetComponentsInChildren<Collider2D>()) {
			if (!c.CompareTag(Constants.STAIRS_TAG)) {
				c.enabled = active;
			}
    	}
	}

	public void SetSecondFloorActive (bool active) {
    	foreach (Building b in buildings) {
			if (active) {
				b.ShowFloor2();
			} else {
				b.HideFloor2();
			}
		}
		SetFadeEnable(active);
	}

	private void SetFadeEnable(bool enable) {
		fade.SetActive(enable);
	}
}
