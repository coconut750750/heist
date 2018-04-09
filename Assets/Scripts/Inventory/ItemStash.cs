using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public abstract class ItemStash : MonoBehaviour {

	public Item[] items;

	protected int count = 0;
	protected int capacity = 0;

	protected string filename;

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

		filename = Application.persistentDataPath + "/" + gameObject.name + ".dat";
				
		Debug.Log("loading from: " + filename);
		Load();
	}

	#if UNITY_EDITOR || UNITY_STANDALONE
	protected void OnApplicationQuit() {
		Debug.Log("saving to: " + filename);
		Save();
	}
	#elif UNITY_ANDROID || UNITY_IOS
	protected void OnApplicationPause() {
		Save();
	}
	#endif

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

	public abstract void Save();

	public abstract void Load();

	protected void LoadFromData(ItemStashData data) {
		this.count = data.count;
		this.capacity = data.capacity;
		
		string[] itemNames = data.itemNames;
		for (int i = 0; i < capacity; i++) {
			if (itemNames[i] == null) {
				items[i] = null;
			} else {
				items[i] = ItemManager.instance.GetItem(itemNames[i]);
			}
		}
	}
}

[Serializable]
public class ItemStashData {
	public string[] itemNames;
	public int count = 0;
	public int capacity = 0;

	public ItemStashData(Item[] items, int count, int capacity) {
		this.itemNames = new string[capacity];
		for (int i = 0; i < capacity; i++) {
			Item item = items[i];
			if (item == null) {
				itemNames[i] = null;
			} else {
				itemNames[i] = item.name;
			}
		}
		this.count = count;
		this.capacity = capacity;
	}
}