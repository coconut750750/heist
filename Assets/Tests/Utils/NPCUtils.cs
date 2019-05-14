using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPCUtils {
	public static NPC SpawnNPC() {
		NPCSpawner.instance.SpawnUnconditionally(new Vector2(0, 0));
		NPC[] npcs = NPCSpawner.instance.GetNPCs();
		return npcs[npcs.Length - 1];
	}

	public static Item AddRandomItem(NPC npc, int index) {
		Item randomItem = ItemManager.instance.GetRandomItem();
		npc.GetInventory().SetItemAtIndex(randomItem, index);
		return randomItem;
	}
}
