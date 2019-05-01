using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPCInteractUtils {
	public static void ShowNPC(NPC npc) {
		NPCTrade npcTrade = NPCTrade.instance;
		npcTrade.Display(npc);
	}

	public static bool SellAvailable() {
		NPCTrade.instance.UpdateButtons();
		return NPCTrade.instance.sellController.sellButton.interactable;
	}

	private static void SelectItemAtIndex(int index) {
		NPCTrade npcTrade = NPCTrade.instance;
		npcTrade.ItemSlots[index].Select();
	}

	public static void PutSellItem(NPC npc, Item item) {
		PlayerUtils.GetPlayer().GetPocket().SetItemAtIndex(item, 0);
		ItemSlot itemSlot = PlayerUtils.GetPocketItemSlot(0);
		ItemDragger.itemBeingDragged = itemSlot.GetComponentInChildren<ItemDragger>();
		NPCTrade.instance.sellController.GetSellingStash().itemSlots[0].OnDrop(null);
	}

	public static bool Sell(NPC npc, Item item) {
		PutSellItem(npc, item);
		if (SellAvailable()) {
			return NPCTrade.instance.sellController.Sell(npc);
		}
		return false;
	}

	public static bool BuyAvailable() {
		NPCTrade.instance.UpdateButtons();
		return NPCTrade.instance.buyController.buyButton.interactable;
	}

	public static bool Buy(NPC npc, int index) {
		NPCTrade npcTrade = NPCTrade.instance;
		npcTrade.Display(npc);
		SelectItemAtIndex(index);

		if (BuyAvailable()) {
			return npcTrade.buyController.Buy(npc);
		}
		return false;
	}

	public static bool BuyRandom(NPC npc) {
		NPCUtils.AddRandomItem(npc, 0);
		return Buy(npc, 0);
	}

	public static void PutTradeItem(NPC npc, Item item) {
		PlayerUtils.GetPlayer().GetPocket().SetItemAtIndex(item, 0);
		ItemSlot itemSlot = PlayerUtils.GetPocketItemSlot(0);
		ItemDragger.itemBeingDragged = itemSlot.GetComponentInChildren<ItemDragger>();
		NPCTrade.instance.tradeController.GetTradingStash().itemSlots[0].OnDrop(null);
	}
}
