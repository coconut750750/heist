using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	[SerializeField]
	private GameObject groundFloor;

	[SerializeField]
	private GameObject secondFloor;

	void Awake() {
		secondFloor.transform.localPosition = new Vector3(0, 0, -0.1f);
	}

	public void ShowFloor2() {
		SetSecondFloorActive(true);
		SetGroundFloorActive(false);
	}

	public void HideFloor2() {
		SetSecondFloorActive(false);
		SetGroundFloorActive(true);
	}

	public void SetGroundFloorActive (bool active) {
    	foreach (Collider2D c in groundFloor.GetComponentsInChildren<Collider2D>()) {
			if (!c.CompareTag(Constants.STAIRS_TAG)) {
				c.enabled = active;
			}
    	}
	}

	public void SetSecondFloorActive (bool active) {
		foreach (Collider2D c in secondFloor.GetComponentsInChildren<Collider2D>()) {
			if (!c.CompareTag(Constants.STAIRS_TAG)) {
				c.enabled = active;
			}
    	}
		secondFloor.SetActive(active);
	}

}
