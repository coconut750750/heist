using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NPCInteractUtils {
	public static void ShowNPC(NPC npc) {
		NPCTrade.instance.Display(npc);
	}

	private static void SelectItemAtIndex(int index) {
		NPCTrade npcTrade = NPCTrade.instance;
		npcTrade.ItemSlots[index].Select();
	}

	public static bool SellAvailable() {
		NPCTrade.instance.UpdateButtons();
		return NPCTrade.instance.sellController.CanSell();
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
		return NPCTrade.instance.buyController.CanBuy();
	}

	public static bool Buy(NPC npc, int index) {
		SelectItemAtIndex(index);

		if (BuyAvailable()) {
			return NPCTrade.instance.buyController.Buy(npc);
		}
		return false;
	}

	public static bool BuyRandom(NPC npc) {
		NPCUtils.AddRandomItem(npc, 0);
		return Buy(npc, 0);
	}

	public static bool TradeAvailable() {
		NPCTrade.instance.UpdateButtons();
		return NPCTrade.instance.tradeController.CanTrade();
	}

	public static void PutTradeItem(NPC npc, Item item) {
		PlayerUtils.GetPlayer().GetPocket().SetItemAtIndex(item, 0);
		ItemSlot itemSlot = PlayerUtils.GetPocketItemSlot(0);
		ItemDragger.itemBeingDragged = itemSlot.GetComponentInChildren<ItemDragger>();
		NPCTrade.instance.tradeController.GetTradingStash().itemSlots[0].OnDrop(null);
	}

	public static bool Trade(NPC npc, Item playerItem, int index) {
		PutTradeItem(npc, playerItem);
		SelectItemAtIndex(index);

		if (TradeAvailable()) {
			return NPCTrade.instance.tradeController.Trade(npc);
		}
		return false;
	}
}
