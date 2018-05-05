using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingStash : PauseStash {

    public const int NUM_ITEMS = 4;

    public int outputIndex;
    
    public CraftingStash() : base(NUM_ITEMS) {
		
    }

    protected override void SetItemSlots() {
		base.SetItemSlots();
        itemSlots[outputIndex].SetInputAllowed(false);
	}
}
