using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	public const int NUM_ITEMS = 7;

	public Image[] itemImages = new Image[NUM_ITEMS];
	public Item[] items = new Item[NUM_ITEMS];

	public void AddItem(Item itemToAdd) {
		for (int i = 0; i < NUM_ITEMS; i++) {
			if (items[i] == null) {
				items[i] = itemToAdd;
				itemImages[i].sprite = itemToAdd.sprite;
				itemImages[i].enabled = true;
				return;
			}
		}
	}

	public void RemoveItem(Item itemToRemove) {
		for (int i = 0; i < NUM_ITEMS; i++) {
			if (items[i] == itemToRemove) {
				itemImages[i] = null;
				itemImages[i].sprite = null;
				itemImages[i].enabled = false;
				return;
			}
		}
	}
}
