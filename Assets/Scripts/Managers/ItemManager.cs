using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour {

	public static int COMMON_CHANCE = 100;
	public static int UNCOMMON_CHANCE = 70;
	public static int RARE_CHANCE = 30;

	public static ItemManager instance = null;

	private Dictionary<string, Item> itemSpriteDic;

	private List<Item> commonItems = new List<Item>();
	private List<Item> uncommonItems = new List<Item>();
	private List<Item> rareItems = new List<Item>();

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
			AddItemToRarityList(items[i]);

			totalRand += items[i].chance;
		}
	}

	private void AddItemToRarityList(Item item) {
		if (item.chance <= RARE_CHANCE) {
			rareItems.Add(item);
		} else if (item.chance <= UNCOMMON_CHANCE) {
			uncommonItems.Add(item);
		} else {
			commonItems.Add(item);
		}
	}

	public Item GetItem(string name) {
		return Object.Instantiate(itemSpriteDic[name]);
	}

	public Item GetRandomItem() {
		// gets a random item based on each item's rarity
		int rand = Random.Range(0, totalRand);
        int sum = 0;
        int i = 0;

        while (sum < rand) {
             sum = sum + items[i++].chance;
        }

        return Object.Instantiate(items[Mathf.Max(0, i - 1)]);
	}

	public Item GetRandomCommonItem() {
		int randIndex = Random.Range(0, commonItems.Count);
		return Object.Instantiate(commonItems[randIndex]);
	}

	public Item GetRandomUncommonItem() {
		int randIndex = Random.Range(0, uncommonItems.Count);
		return Object.Instantiate(uncommonItems[randIndex]);
	}

	public Item GetRandomRareItem() {
		int randIndex = Random.Range(0, rareItems.Count);
		return Object.Instantiate(rareItems[randIndex]);		
	}
}