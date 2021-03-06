﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyController : MonoBehaviour {

	private const string EMPTY_PRICE_TEXT = "---";

	[SerializeField]
	private Button buyButton;

	[SerializeField]
	private Text priceText;

	private int price;

	private Item selectedItem;
	private int selectedIndex;

	void Awake() {
		ResetPriceText();
		Disable();
		selectedItem = null;
		selectedIndex = -1;

		price = -1;
	}

	public bool Buy(NPC npc) {
		if (selectedItem == null) {
			return false;
		}
		int playerMoney = GameManager.instance.mainPlayer.GetMoney();

		if (playerMoney >= price) {
			GameManager.instance.mainPlayer.SetMoney(playerMoney - price);
			GameManager.instance.mainPlayer.AddItem(selectedItem);
			selectedItem.ChangedHands();
			
			npc.GetInventory().RemoveItemAtIndex(selectedIndex);
			npc.SetMoney(npc.GetMoney() + price);

			return true;
		}

		return false;
	}

	public void UpdateButtons(bool hasTradingItem) {
		Disable();

		if (!GameManager.instance.mainPlayer.CanAddItem()) {
			return;
		}

		if (selectedItem != null) {
			if (GameManager.instance.mainPlayer.NumItems() == 
						GameManager.instance.mainPlayer.GetPocket().GetCapacity() - 1 &&
						hasTradingItem) {
				// player has open slot but trading item isnt empty
				Disable();
			} else if (GameManager.instance.mainPlayer.GetMoney() >= selectedItem.price) {
				Enable();
			}
		}
	}

	public void Enable() {
		buyButton.interactable = true;
	}

	public void Disable() {
		buyButton.interactable = false;
	}

	private void SetPriceText() {
		priceText.text = price.ToString();
	}

	private void ResetPriceText() {
		priceText.text = EMPTY_PRICE_TEXT;
	}

	public Item GetSelectedItem() {
		return selectedItem;
	}

	public void SetSelectedItem(Item item, int index) {
		selectedItem = item;
		selectedIndex = index;
		if (item != null) {
			price = Mathf.RoundToInt(selectedItem.GetValue() * NPC.SELL_PERC);
			SetPriceText();
		} else {
			ResetPriceText();
		}
	}
}
