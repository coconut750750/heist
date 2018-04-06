using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	public const int NUM_ITEMS = 7;

	public Image[] itemImages = new Image[NUM_ITEMS];
	public Item[] items = new Item[NUM_ITEMS];

	private int count = 0;

	void Awake() {
		SetDraggerIndices();
	}

	void SetDraggerIndices() {
		for (int i = 0; i < NUM_ITEMS; i++) {
			itemImages[i].gameObject.GetComponent<ItemDragger>().index = i;
		}
	}

	public void AddItem(Item itemToAdd) {
		if (count == NUM_ITEMS) {
			return;
		}

		for (int i = 0; i < NUM_ITEMS; i++) {
			if (items[i] == null) {
				Debug.Log("added at index: " + i);
				items[i] = itemToAdd;
				itemImages[i].sprite = itemToAdd.sprite;
				itemImages[i].enabled = true;
				itemImages[i].gameObject.GetComponent<ItemDragger>().SetItem(itemToAdd);
				count++;
				return;
			}
		}
	}

	public Item GetItem(int index) {
		if (index >= 0 && index < count) {
			return items[index];
		}
		return null;
	}

	public void RemoveItemAtIndex(int index) {
		if (count == 0) {
			return;
		}

		if (items[index] != null) {
			count--;
			items[index] = null;
			itemImages[index].sprite = null;
			itemImages[index].enabled = false;
			itemImages[index].gameObject.GetComponent<ItemDragger>().Reset();
		}
	}

	public void RemoveItem(Item itemToRemove) {
		if (count == 0) {
			return;
		}

		for (int i = 0; i < NUM_ITEMS; i++) {
			if (items[i] != null && items[i] == itemToRemove) {
				RemoveItemAtIndex(i);
				return;
			}
		}
	}

	public void SwapItemPositions(int index1, int index2) {
		Item temp = items[index1];
		items[index1] = items[index2];
		items[index2] = temp;
	}

	public void DeselectAll() {
		for (int i = 0; i < NUM_ITEMS; i++) {
			itemImages[i].gameObject.GetComponent<ItemDragger>().SetSelected(false);
		}
	}

	public int GetCapacity() {
		return NUM_ITEMS;
	}

	public int GetNumItems() {
		return count;
	}

	public void Log() {
		string log = "Items: ";
		for (int i = 0; i < NUM_ITEMS; i++) {
			if (items[i] == null) {
				log = log + "null ";
			} else {
				log = log + items[i].name + " ";
			}
		}
		Debug.Log(log);
	}
}
