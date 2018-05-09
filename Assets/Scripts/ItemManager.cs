using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

	public static ItemManager instance = null;

	private Dictionary<string, Item> itemSpriteDic; 

	[SerializeField]
	private Item[] items;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		itemSpriteDic = new Dictionary<string, Item>();
		for (int i = 0; i < items.Length; i++) {
			itemSpriteDic.Add(items[i].itemName, items[i]);
		}
	}

	void Update() {
		// finished loading items here so delete
		Delete();
	}

	public Item GetItem(string name) {
		return Object.Instantiate(itemSpriteDic[name]);
	}

	public void Delete() {
		Destroy(instance);
	}
}
