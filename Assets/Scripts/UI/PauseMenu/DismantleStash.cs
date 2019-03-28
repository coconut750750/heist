using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

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

	public void SetOutputRemovedCallBack(UnityAction OnOutputRemoved) {
		foreach (int index in outputIndices) {
	        itemSlots[index].OnRemoved += OnOutputRemoved;			
		}
    }

	public Item GetInput() {
		return GetItem(0);
	}

	public void SetOutput(Item[] item) {
		if (item.Length > outputIndices.Length) {
			return;
		}
		for (int i = 0; i < item.Length; i++) {
			AddItemAtIndex(item[i], outputIndices[i]);
		}
    }

    public Item[] GetOutput() {
        return (from i in outputIndices select GetItem(i)).ToArray();
    }

	public bool ReadyForDismantle() {
		foreach (Item item in GetOutput()) {
			if (item != null) {
				return false;
			}
		}
        return true;
    }
}
