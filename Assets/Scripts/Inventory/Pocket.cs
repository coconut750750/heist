using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	ItemStashData data = new ItemStashData(base.items, base.count, base.capacity);
	GameManager.Save(data, base.filename);
	}

	public override void Load() {
		ItemStashData data = GameManager.Load<ItemStashData>(base.filename);

		if (data != null) {
			base.LoadFromData(data);
			for (int i = 0; i < base.capacity; i++) {
				itemSlots[i].InsertItem(base.items[i], this);
			}
		}
	}
}