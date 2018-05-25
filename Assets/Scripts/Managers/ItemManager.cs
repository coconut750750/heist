using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour {

	public static ItemManager instance = null;

	private Dictionary<string, Item> itemSpriteDic;

	// Used for random item generator
	private int totalRand;

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

			totalRand += items[i].chance;
		}
	}

	void Update() {
	}

	public Item GetItem(string name) {
		return Object.Instantiate(itemSpriteDic[name]);
	}

	public Item GetRandomItem() {
		// gets a random item based on each item's probability
		int rand = Random.Range(0, totalRand);
        int sum = 0;
        int i = 0;

        while(sum < rand) {
             sum = sum + items[i++].chance;
        }

        return Object.Instantiate(items[Mathf.Max(0, i - 1)]);
	}
}