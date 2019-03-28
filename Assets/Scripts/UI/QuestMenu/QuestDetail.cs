using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestDetail : MonoBehaviour {

	private const string NO_QUEST = "No quests at this time right now.";
	private const string NO_QUEST_PRICE = "--";

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
		UpdateDisplayingQuest();
	}

	public void UpdateDisplayingQuest() {
		npcNameText.text = displayingQuest.reporter.GetName();
		questDetailsText.text = displayingQuest.GetCurrentDetails();
		rewardText.text = displayingQuest.GetCurrentReward().ToString();
	}

	public void DisplayEmptyQuest(NPC npc) {
		InitDisplay(null);
		npcNameText.text = npc.GetName();
		questDetailsText.text = NO_QUEST;
		rewardText.text = NO_QUEST_PRICE;
	}

	public Quest GetDisplayingQuest() {
		return displayingQuest;
	}
}
