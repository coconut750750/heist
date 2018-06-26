using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetail : MonoBehaviour {

	[SerializeField]
	private Text npcNameText;
	[SerializeField]
	private Text questDetailsText;
	[SerializeField]
	private Text rewardText;

	private Quest displayingQuest = null;

	public void InitDisplay(Quest quest) {
		transform.localScale = new Vector3(1, 1, 1);
		displayingQuest = quest;
	}

	public void DisplayQuest(Quest quest) {
		InitDisplay(quest);
		npcNameText.text = quest.reporter.GetName();
		questDetailsText.text = quest.GetCurrentDetails();
		rewardText.text = quest.GetCurrentReward().ToString();
	}

	public void DisplayEmptyQuest(NPC npc) {
		InitDisplay(null);
		npcNameText.text = npc.GetName();
		questDetailsText.text = "No quests at this time right now.";
		rewardText.text = "--";
	}

	public Quest GetDisplayingQuest() {
		return displayingQuest;
	}
}
