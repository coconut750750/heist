using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class CraftingStash : PauseStash {

    public const int NUM_ITEMS = 4;

    public int outputIndex;
    
    public CraftingStash() : base(NUM_ITEMS) {
		
    }

    protected override void SetItemSlots() {
		base.SetItemSlots();
        itemSlots[outputIndex].SetInputAllowed(false);
	}

    public Item[] GetInputs() {
        return items.Where(item => item != null).ToArray();
    }

    public void SetOutput(Item item) {
        AddItemAtIndex(item, outputIndex);
    }

    public Item GetOutput() {
        return GetItem(outputIndex);
    }
}
