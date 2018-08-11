using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour {

	public static EventManager instance = null;

	// Use this for initialization
	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
	}
	
	public void OnStealItem(NPC npc, Item item) {
	}

	public void OnCraftItem(Item item) {
	}

	public void OnDefeatNPC(NPC npc) {
	}

	public void OnSellItem(NPC npc, Item item) {
	}


}
