using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour {

	public static NPCQuest instance = null;

	[SerializeField]
	private Text npcNameText;
	[SerializeField]
	private Text questText;
	[SerializeField]
	private Text rewardText;

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
		GameManager.instance.PauseGame();

		gameObject.SetActive(true);

		this.npc = npc;
		this.npcNameText.text = npc.GetName();
		this.quest = npc.GetQuest();
		if (this.quest != null && !this.quest.IsActive()) {
			questText.text = this.quest.GetCurrentDetails();
			rewardText.text = this.quest.GetCurrentReward().ToString();
			EnableButtons();
		} else {
			questText.text = "No quest at this time right now.";
			rewardText.text = "--";
			DisableButtons();
		}
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
		
		GameManager.instance.UnpauseGame();
	}

	public void AcceptedQuest() {
		try {
			quest.OnAccept();
		} catch (QuestOverflowException) {
			return;
		}
		
		DisableButtons();

		Hide();
	}

	public void RejectedQuest() {
		quest.OnReject();
		DisableButtons();

		Hide();
	}
}
