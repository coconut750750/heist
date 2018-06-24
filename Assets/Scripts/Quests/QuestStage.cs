using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStage {

	private string details;
	private int reward;

	protected string itemRequirement; // name of item. empty if not using
	protected string npcRequirement; // name of npc. empty if not using

	public QuestStage(string details, int reward, string itemRequirement, string npcRequirement) {
		this.details = details;
		this.reward = reward;
		this.itemRequirement = itemRequirement;
		this.npcRequirement = npcRequirement;
	}

	public void OnComplete(NPC reporter) {
		reporter.CompletedQuestStage();
		GameManager.instance.mainPlayer.SetMoney(GameManager.instance.mainPlayer.GetMoney() + reward);
	}

	public string GetDetails() {
		return details;
	}

	public int GetReward() {
		return reward;
	}

	[System.Serializable]
	public class QuestStageData : GameData {
		public string details;
		public int reward;

		public string itemRequirement;
		public string npcRequirement;

		public QuestStageData(QuestStage stage) {
			this.details = stage.GetDetails();
			this.reward = stage.GetReward();
			this.itemRequirement = stage.itemRequirement;
			this.npcRequirement = stage.npcRequirement;
		}
	}
}