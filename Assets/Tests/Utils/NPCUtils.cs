using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPCUtils {
	public static NPC SpawnNPC() {
		NPCSpawner.instance.SpawnUnconditionally(new Vector2(0, 0));
		NPC[] npcs = NPCSpawner.instance.GetNpcs();
		return npcs[npcs.Length - 1];
	}
}
