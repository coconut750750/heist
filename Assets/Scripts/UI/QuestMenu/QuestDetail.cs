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

	public void InitDisplay() {
		transform.localScale = new Vector3(1, 1, 1);
	}

	public void DisplayQuest(Quest quest) {
		InitDisplay();
		npcNameText.text = quest.reporter.GetName();
		questDetailsText.text = quest.GetCurrentDetails();
		rewardText.text = quest.GetCurrentReward().ToString();
	}

	public void DisplayQuestStage(QuestStage questStage, NPC reporter) {
		InitDisplay();
		npcNameText.text = reporter.GetName();
		questDetailsText.text = questStage.GetDetails();
		rewardText.text = questStage.GetReward().ToString();
	}

	public void DisplayEmptyQuest(NPC npc) {
		InitDisplay();
		npcNameText.text = npc.GetName();
		questDetailsText.text = "No quest at this time right now.";
		rewardText.text = "--";
	}
}
