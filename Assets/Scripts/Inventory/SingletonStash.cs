using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Different from a regular inventory
// Every time a UI holding a singleton stash opens up, the SAME item stash gets displayed
public abstract class SingletonStash : ItemStash {

	public ItemSlot[] itemSlots;
	protected Image[] itemImages;

    public SingletonStash(int numItems) {
		itemSlots = new ItemSlot[numItems];
		itemImages = new Image[numItems];
    }

    protected override void Start() {
		base.Start();
		SetItemSlots();
	}

	protected virtual void SetItemSlots() {
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
}
