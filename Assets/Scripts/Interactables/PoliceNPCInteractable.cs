﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPCInteractable : NPCInteractable {

	private bool knockedOut = false;

	public override void OnKnockout() {
		base.OnKnockout();
		knockedOut = true;
	}

	private void ShowInventoryOption() {
		((PoliceNPCOptions)base.npcOptionsInstance).ShowLootButton();
	}

	protected override void InitNPCOptions() {
		base.InitNPCOptions();
        if (knockedOut) {
			((PoliceNPCOptions)base.npcOptionsInstance).ShowLootButton();
        }
    }

	public override void ShowInventory() {
		StashDisplayer.instance.DisplayInventory(base.npc.GetInventory());
    }
}
