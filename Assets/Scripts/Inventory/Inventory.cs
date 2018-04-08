using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : ItemStash {

	public const int NUM_ITEMS = 24;

	private bool isDisplaying;

	public Inventory() : base(NUM_ITEMS) {
		isDisplaying = false;
	}

	void Awake () {
		
	}

    public override void SetDisplaying(bool isDisplaying) {
        this.isDisplaying = isDisplaying;
    }

    public override bool IsDisplaying() {
        return isDisplaying;
    }

    public override void DeselectAll() {
        if (isDisplaying) {
			GameManager.instance.stashDisplayer.DeselectAll();
		}
    }
}
