using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SellingStash : PauseStash {

	public const int NUM_ITEMS = 1;

    public UnityAction<Item> OnAdded;
    public UnityAction OnRemoved;

    public SellingStash() : base(NUM_ITEMS) {

    }

    protected override void SetItemSlots() {
		    base.SetItemSlots();
        itemSlots[0].OnDropped += OnAdded;
        itemSlots[0].OnRemoved += OnRemoved;
	}
}
