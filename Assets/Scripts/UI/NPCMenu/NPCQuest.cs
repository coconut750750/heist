using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour {

	public static NPCQuest instance = null;

	[SerializeField]
	private QuestDetail questDetail;

	private NPC npc;
	private Quest quest;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		gameObject.SetActive(false);
	}

	public void Display(NPC npc) {
		gameObject.SetActive(true);

		this.npc = npc;
		this.quest = npc.GetQuest();
		if (CanDisplayQuest(npc)) {
			questDetail.DisplayQuest(this.quest);
			EnableButtons();
		} else {
			questDetail.DisplayEmptyQuest(npc);
			DisableButtons();
		}
	}

	private bool CanDisplayQuest(NPC npc) {
		return npc.GetQuest() != null && !npc.GetQuest().IsActive() && !npc.IsKnockedOut();
	}

	private void EnableButtons() {
		foreach (Button button in GetComponentsInChildren<Button>()) {
			button.interactable = true;
		}
	}

	private void DisableButtons() {
		foreach (Button button in GetComponentsInChildren<Button>()) {
			button.interactable = false;
		}
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	// Used by the AcceptButton GameObject
	public void AcceptedQuest() {
		DisableButtons();
		Hide();

		try {
			quest.OnAccept();
		} catch (QuestOverflowException) {
			return;
		}
	}

	// Used by the RejectButton GameObject
	public void RejectedQuest() {
		quest.OnReject();
		DisableButtons();

		Hide();
	}
}
