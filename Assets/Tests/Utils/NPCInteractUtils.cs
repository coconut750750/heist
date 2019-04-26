using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPCInteractUtils {
	public static bool Sell(NPC npc, Item item) {
		NPCTrade npcTrade = NPCTrade.instance;
		npcTrade.Display(npc);
		npcTrade.sellController.SellItemEntered(item);
		return npcTrade.sellController.Sell(npc);
	}

	public static bool Buy(NPC npc, Item item, int index) {
		NPCTrade npcTrade = NPCTrade.instance;
		npcTrade.Display(npc);
		npcTrade.buyController.SetSelectedItem(item, index);
		return npcTrade.buyController.Buy(npc);
	}
}
