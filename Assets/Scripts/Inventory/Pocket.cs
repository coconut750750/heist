using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>  
///		This is the Pocket class.
/// 	The Pocket contains the 7 items that are readily accessible to the player.
/// 	This item stash is a single displaying item stash attached to the player. When it displays (always is) 
///  there is only one inventory to display, so it extends the singleton stash.
/// 	SAVING and LOADING: done by this class
/// </summary>  
public class Pocket : SingletonStash {

	public const int NUM_ITEMS = 7;

	public Pocket() : base(NUM_ITEMS) {

	}
	
	public override void SetDisplaying(bool isDisplaying) {
		// do nothing, its always displaying
	}

	public override bool IsDisplaying() {
		return true;
	}

	public override void Save() {
	ItemStashData data = new ItemStashData(this);
	GameManager.Save(data, base.filename);
	}

	public override void Load() {
		base.Load();
		ItemStashData data = GameManager.Load<ItemStashData>(base.filename);

		if (data != null) {
			base.LoadFromData(data);
			for (int i = 0; i < base.capacity; i++) {
				itemSlots[i].SetItem(base.items[i], this);
			}
		}
	}
}