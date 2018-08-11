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

	public void OnPunch() {
		
	}
	
	public void OnStealItem(NPC npc, Item item) {
		QuestEventHandler.instance.OnStealItemQuest(npc, item);
	}

	public void OnCraftItem(Item item) {
		QuestEventHandler.instance.OnCraftItemQuest(item);
	}

	public void OnDefeatNPC(NPC npc) {
		QuestEventHandler.instance.OnDefeatNPCQuest(npc);
	}

	public void OnSellItem(NPC npc, Item item) {
		QuestEventHandler.instance.OnSellItemQuest(npc, item);
	}


}
