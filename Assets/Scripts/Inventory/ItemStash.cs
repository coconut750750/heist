using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>  
///		This is the abstract base ItemStash class.
/// 	This class contains a list of Items, the number of items, and the capacity of the stash.
///		SAVING and LOADING:
/// 		saving: subclasses must override saving
///			loading: subclasses load, but this class contains a helper function to load base class members
/// </summary>  
public abstract class ItemStash : MonoBehaviour {

	private const string CLASS_NAME = "itemstash";

	public Item[] items;

	protected int count = 0;
	protected int capacity = 0;

	protected string filename;

	protected virtual void Start() {
		capacity = items.Length;

		filename = Application.persistentDataPath + "/" + gameObject.name + "-" + CLASS_NAME + ".dat";
		Load();
	}

	#if UNITY_EDITOR || UNITY_STANDALONE
	protected void OnApplicationQuit() {
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

	public void SetItemAtIndex(Item itemToSet, int index) {
		if (items[index] == null && itemToSet != null) {
			count++;
		} else if (itemToSet == null && items[index] != null) {
			count--;
		}
		items[index] = itemToSet;
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

	public void RemoveAll() {
		for (int i = 0; i < capacity; i++) {
			RemoveItemAtIndex(i);
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

	public bool IsFull() {
		return count == capacity;
	}

	public abstract void SetDisplaying(bool isDisplaying);

	public abstract bool IsDisplaying();

	public abstract void DeselectAll();

	public void Log() {
		Debug.Log(ToString());
	}

	public override string ToString() {
		string log = "" + gameObject.name + " " + count + " items: ";
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

	public void LoadFromData(ItemStashData data) {
		this.count = data.count;
		this.capacity = data.capacity;
		
		ItemData[] itemData = data.itemData;
		for (int i = 0; i < capacity; i++) {
			if (itemData[i] == null) {
				items[i] = null;
			} else {
				items[i] = ItemManager.instance.GetItem(itemData[i].itemName);
				items[i].quality = itemData[i].itemQuality;
			}
		}
	}
}

[Serializable]
public class ItemStashData : GameData {
	public ItemData[] itemData;

	public int count;
	public int capacity;

	public ItemStashData(ItemStash stash) {
		this.count = stash.GetNumItems();
		this.capacity = stash.GetCapacity();

		this.itemData = new ItemData[capacity];
		for (int i = 0; i < capacity; i++) {
			Item item = stash.GetItem(i);
			if (item == null) {
				itemData[i] = null;
			} else {
				itemData[i] = new ItemData(item);
			}
		}
		
	}
}