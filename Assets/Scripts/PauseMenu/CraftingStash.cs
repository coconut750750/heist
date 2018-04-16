using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingStash : PauseStash {

    public int outputIndex;
    
    public CraftingStash() : base() {
		
    }

    protected override void SetItemSlots() {
		base.SetItemSlots();
        itemSlots[outputIndex].SetInputAllowed(false);
	}
}
