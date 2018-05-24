using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class Item : ScriptableObject {

	public const float CHANGE_HANDS_DECAY = 0.95f;

	public string itemName;
	public int quality; // out of 100

	// constant for all items
	public Sprite sprite;
	public int price; // constant for all items
	public int chance; // out of 100, the chance of an npc getting this item;

	public float GetValue() {
		return (float)(price * quality) / 100f;
	}

	public void ChangedHands() {
		quality = Mathf.RoundToInt(quality * CHANGE_HANDS_DECAY);
	}

}

[Serializable]
public class ItemData : GameData {
	public string itemName;
	public int itemQuality;
	
	public ItemData(Item item) {
		this.itemName = item.itemName;
		this.itemQuality = item.quality;
	}
}