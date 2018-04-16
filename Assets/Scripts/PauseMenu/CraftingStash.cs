using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingStash : ItemStash {

	public const int NUM_ITEMS = 4;
    public const int OUTPUT_INDEX = 3;

    public ItemSlot[] itemSlots = new ItemSlot[NUM_ITEMS];
    private Image[] itemImages = new Image[NUM_ITEMS];
    
    private bool isDisplaying;

    public CraftingStash() : base(NUM_ITEMS) {
		
    }

    void Awake() {
        SetItemSlots();
        isDisplaying = false;
    }

    protected void SetItemSlots() {
		for (int i = 0; i < capacity; i++) {
            itemImages[i] = itemSlots[i].GetItemImage();
			itemSlots[i].SetIndex(i);
			itemSlots[i].SetParentStash(this);
		}
        itemSlots[OUTPUT_INDEX].SetInputAllowed(false);
	}

    public override bool AddItemAtIndex(Item itemToAdd, int index) {
        if (!isDisplaying) {
            return false;
        }
		bool success = base.AddItemAtIndex(itemToAdd, index);

		if (success) {
			itemSlots[index].SetItem(itemToAdd);
		}

		return success;
	}

    public override bool RemoveItemAtIndex(int index) {
        if (!isDisplaying) {
            return false;
        }
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

    public void Display() {
        SetDisplaying(true);
    }

    public void Hide() {
        SetDisplaying(false);
    }

    public override bool IsDisplaying() 
    {
        return isDisplaying;
    }

    public override void SetDisplaying(bool isDisplaying)
    {
        this.isDisplaying = isDisplaying;
    }

    public override void Load() {
        return;
    }

    public override void Save() {
        return;
    }    
}
