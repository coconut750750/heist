using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DismantleStash : PauseStash {

	public int[] outputIndices;
    
    public DismantleStash() : base() {
		
    }

    protected override void SetItemSlots() {
		base.SetItemSlots();
		foreach (int i in outputIndices) {
			itemSlots[i].SetInputAllowed(false);
		}
	}
}
