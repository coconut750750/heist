﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Desk : Interactable {

    private UnityAction call;

    private Inventory items;

	void Start () {
        items = gameObject.GetComponent<Inventory>();
	}
	
    public override void Interact(Player player) {
        StashDisplayer.instance.DisplayInventory(items);
    }

    public override void EnterRange(Player player)
    {
    }

    public override void ExitRange(Player player)
    {
    }
}
