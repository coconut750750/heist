using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellController : MonoBehaviour {

	private const string EMPTY_PRICE_TEXT = "---";

	[SerializeField]
	private Button sellButton;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private SellingStash sellingStash;

	private Item sellingItem;
	private int sellingPrice;
	private bool npcWillBuy;

	void Awake () {
		Disable();
		sellingItem = null;
		sellingPrice = -1;
		npcWillBuy = false;

		priceText.text = EMPTY_PRICE_TEXT;

		sellingStash.OnAdded += SellItemEntered;
		sellingStash.OnRemoved += SellItemRemoved;
	}
	
	void Start () {
		Disable();
	}

	public bool Sell(NPC npc) {
		if (sellingItem == null || !npcWillBuy) {
			return false;
		}

		int currentMoney = GameManager.instance.mainPlayer.GetMoney();
		GameManager.instance.mainPlayer.SetMoney(currentMoney + sellingPrice);

		npc.SetMoney(npc.GetMoney() - sellingPrice);
		npc.GetInventory().AddItem(sellingItem);
		sellingItem.ChangedHands();

		sellingStash.RemoveItem(sellingItem);
		sellingItem = null;
		sellingPrice = -1;
		npcWillBuy = false;
		priceText.text = EMPTY_PRICE_TEXT;

		return true;
	}

	public void UpdateButtons() {
		Disable();

		NPC npc = NPCTrade.instance.GetNPC();
		if (npc.GetInventory().GetNumItems() == npc.GetInventory().GetCapacity()) {
			return;
		}

		if (sellingItem != null) {
			npcWillBuy = sellingPrice <= npc.GetMoney();
			if (npcWillBuy) {
				Enable();
			}
		}
	}

	private void SellItemEntered(Item item) {
		sellingItem = item;
		sellingPrice = Mathf.RoundToInt(sellingItem.GetValue() * NPC.BUY_PERC);
		priceText.text = sellingPrice.ToString();

		UpdateButtons();
	}

	private void SellItemRemoved() {
		sellingItem = null;
		sellingPrice = -1;
		priceText.text = EMPTY_PRICE_TEXT;

		UpdateButtons();
	}

	public void Enable() {
		sellButton.interactable = true;
	}

	public void Disable() {
		sellButton.interactable = false;
	}

	public void HideSellingStash() {
		sellingStash.Hide();
	}

	public void DeselectAll() {
		sellingStash.DeselectAll();
	}
}
