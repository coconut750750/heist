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

	void Start() {
		for (int i = 0; i < capacity; i++) {
			if (items[i] != null) {
				count++;
			}
		}
	}

	public virtual bool AddItemAtIndex(Item itemToAdd, int index) {
		if (itemToAdd == null) {
			return false;
		}
		
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
		if (index >= 0 && index < capacity) {
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

	public abstract void SetDisplaying(bool isDisplaying);

	public abstract bool IsDisplaying();

	public abstract void DeselectAll();

	public void Log() {
		Debug.Log(ToString());
	}

	public override string ToString() {
		string log = "" + count + " items: ";
		for (int i = 0; i < capacity; i++) {
			if (items[i] == null) {
				log = log + "null ";
			} else {
				log = log + items[i].name + " ";
			}
		}
		return log;
	}
}
