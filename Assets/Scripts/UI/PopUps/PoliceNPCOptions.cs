using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoliceNPCOptions : NPCOptions {

	[SerializeField]
	private Button lootButton;

	public void ShowLootButton() {
		lootButton.gameObject.SetActive(true);
	}

	public void HideLootButton() {
		lootButton.gameObject.SetActive(false);
	}
}
