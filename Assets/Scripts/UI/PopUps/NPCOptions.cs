using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>  
///		This is the NPCOptions class.
/// 	Pops up when players interact with an NPC alongside the Speech Bubble object.
///
/// 	SAVING AND LOADING: None
/// </summary>  
public class NPCOptions : PopUp {

	private static Vector3 optionsOffset = new Vector3(-1.75f, -0.5f, 0);

	private event Action OnClickInventory;
	private event Action OnClickQuest;
	private event Action OnClickInfo;


    public NPCOptions() : base(optionsOffset) {
    }

	public override void Display(Transform parent) {
		base.Display(parent);
	}

	public override void Destroy() {
		base.Destroy();
	}

	public void SetCallbacks(Action OnClickInventory, Action OnClickQuest, Action OnClickInfo) {
		this.OnClickInventory = OnClickInventory;
		this.OnClickQuest = OnClickQuest;
		this.OnClickInfo = OnClickInfo;
	}

	public void ShowInventory() {
		if (OnClickInventory != null) {
			OnClickInventory();
		}
	}

	public void ShowQuest() {
		if (OnClickQuest != null) {
			OnClickQuest();
		}
	}

	public void ShowInfo() {
		if (OnClickInfo != null) {
			OnClickInfo();
		}
	}
}
