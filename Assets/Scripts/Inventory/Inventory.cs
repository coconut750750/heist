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
	
	
}
