using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DismantleStash : PauseStash {

	public const int NUM_ITEMS = 4;

	public int[] outputIndices;
    
    public DismantleStash() : base(NUM_ITEMS) {
		
    }

    protected override void SetItemSlots() {
		base.SetItemSlots();
		foreach (int i in outputIndices) {
			itemSlots[i].SetInputAllowed(false);
		}
	}
}
