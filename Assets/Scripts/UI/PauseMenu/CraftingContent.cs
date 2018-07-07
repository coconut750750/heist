using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingContent : MonoBehaviour {

	private CraftingStash craftingStash;

	void Awake () {
		craftingStash = gameObject.GetComponent<CraftingStash>();
	}

	void OnEnable() {
		craftingStash.Display();
	}

	void OnDisable() {
		craftingStash.Hide();
	}

	public CraftingStash GetStash() {
		return craftingStash;
	}
}
