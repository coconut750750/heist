using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pocket : ItemStash {

	public const int NUM_ITEMS = 7;

	public Image[] itemImages = new Image[NUM_ITEMS];

	public Pocket() : base(NUM_ITEMS) {

	}

	void Awake() {
		SetDraggerIndices();
	}

	protected void SetDraggerIndices() {
		for (int i = 0; i < capacity; i++) {
			itemImages[i].transform.parent.GetComponent<ItemSlot>().SetIndex(i);
		}
	}

	public override bool AddItemAtIndex(Item itemToAdd, int index) {
		bool success = base.AddItemAtIndex(itemToAdd, index);

		if (success) {
			itemImages[index].sprite = itemToAdd.sprite;
			itemImages[index].enabled = true;
			itemImages[index].transform.parent.GetComponent<ItemSlot>().SetItem(itemToAdd);
		}

		return success;
	}

	public override bool RemoveItemAtIndex(int index) {
		bool success = base.RemoveItemAtIndex(index);

		if (success) {
			itemImages[index].sprite = null;
			itemImages[index].enabled = false;
			itemImages[index].transform.parent.GetComponent<ItemSlot>().Reset();
		}
			
		return success;
	}

	public void DeselectAll() {
		for (int i = 0; i < capacity; i++) {
			itemImages[i].transform.parent.GetComponent<ItemSlot>().Deselect();
		}
	}
}
