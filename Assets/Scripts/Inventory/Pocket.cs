using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Pocket : ItemStash {

	public const int NUM_ITEMS = 7;

	private Image[] itemImages = new Image[NUM_ITEMS];
	public ItemSlot[] itemSlots = new ItemSlot[NUM_ITEMS];

	public Pocket() : base(NUM_ITEMS) {

	}

	void Awake() {
		SetItemSlots();
	}

	protected void SetItemSlots() {
		for (int i = 0; i < capacity; i++) {
			itemImages[i] = itemSlots[i].GetItemImage();
			itemSlots[i].SetIndex(i);
			itemSlots[i].SetParentStash(this);
		}
	}
	
	public override bool AddItemAtIndex(Item itemToAdd, int index) {
		bool success = base.AddItemAtIndex(itemToAdd, index);

		if (success) {
			itemSlots[index].SetItem(itemToAdd);
		}

		return success;
	}

	public override bool RemoveItemAtIndex(int index) {
		bool success = base.RemoveItemAtIndex(index);

		if (success) {
			itemSlots[index].Reset();
		}
			
		return success;
	}

	public override void DeselectAll() {
		for (int i = 0; i < capacity; i++) {
			itemSlots[i].Deselect();
		}
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
				itemSlots[i].SetItem(base.items[i]);
			}
		}
    }
}