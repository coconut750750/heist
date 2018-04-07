using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ItemStash : MonoBehaviour {

	public Item[] items;

	protected int count = 0;
	protected int capacity = 0;

	public ItemStash(int numItems) {
		items = new Item[numItems];

		capacity = numItems;
	}

	void Awake() {
		
	}

	public virtual bool AddItemAtIndex(Item itemToAdd, int index) {
		if (count != capacity && items[index] == null) {
			items[index] = itemToAdd;
			count++;
			return true;
		}

		return false;
	}

	public void AddItem(Item itemToAdd) {
		if (count == capacity) {
			return;
		}

		for (int i = 0; i < capacity; i++) {
			if (items[i] == null) {
				AddItemAtIndex(itemToAdd, i);
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

	public virtual bool RemoveItemAtIndex(int index) {
		if (count != 0 && items[index] != null) {
			count--;
			items[index] = null;
			return true;
		}

		return false;
	}

	public void RemoveItem(Item itemToRemove) {
		if (count == 0) {
			return;
		}

		for (int i = 0; i < capacity; i++) {
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

	public int GetCapacity() {
		return capacity;
	}

	public int GetNumItems() {
		return count;
	}

	public void Log() {
		string log = "Items: ";
		for (int i = 0; i < capacity; i++) {
			if (items[i] == null) {
				log = log + "null ";
			} else {
				log = log + items[i].name + " ";
			}
		}
		Debug.Log(log);
	}
}
