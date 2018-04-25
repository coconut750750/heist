using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class Item : ScriptableObject {

	public string itemName;
	public Sprite sprite;
	public int quality; // out of 100

}

[Serializable]
public class ItemData : GameData {
	public string itemName;
	public int itemQuality;
	
	public ItemData(Item item) {
		this.itemName = item.name;
		this.itemQuality = item.quality;
	}
}