using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	public const int NUM_ITEMS = 7;

	public Image[] itemImages = new Image[NUM_ITEMS];
	public Item[] items = new Item[NUM_ITEMS];
	public ItemButton[] itemButtons = new ItemButton[NUM_ITEMS];

	private int count = 0;

	public void AddItem(Item itemToAdd) {
		if (count == NUM_ITEMS) {
			return;
		}
		items[count] = itemToAdd;
		itemImages[count].sprite = itemToAdd.sprite;
		itemImages[count].enabled = true;
		itemButtons[count].SetItem(itemToAdd);
		count++;
	}

	public Item GetItem(int index) {
		if (index >= 0 || index < count) {
			return items[index];
		}
		return null;
	}

	public void RemoveItem(Item itemToRemove) {
		if (count == 0) {
			return;
		}
		count--;
		items[count] = null;
		itemImages[count].sprite = null;
		itemImages[count].enabled = false;
		itemButtons[count].Reset();
	}

	public int GetCapacity() {
		return NUM_ITEMS;
	}

	public int GetNumItems() {
		return count;
	}
}
