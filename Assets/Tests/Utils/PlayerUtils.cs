using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerUtils {

	public static Player GetPlayer() {
		return GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

	public static ItemSlot GetPocketItemSlot(int index) {
		return GameObject.Find("Pocket").GetComponentsInChildren<ItemSlot>()[index];
	}
}
