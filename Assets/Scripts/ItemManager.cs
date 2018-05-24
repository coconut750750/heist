using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
	}

	public Item GetItem(string name) {
		return Object.Instantiate(itemSpriteDic[name]);
	}

	public Item GetRandomItem() {
		int size = items.Length;
		return items[Random.Range(0, size)];
	}
}
