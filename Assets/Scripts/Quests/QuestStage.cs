using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStage {

	private string details;
	private int stageNum;

	private int reward;

	public QuestStage(string details, int stageNum, int reward) {
		this.details = details;
		this.stageNum = stageNum;
		this.reward = reward;
	}

	public void OnComplete(NPC reporter) {
		// TODO: UI pops up
		reporter.CompletedQuestStage();
		GameManager.instance.mainPlayer.SetMoney(GameManager.instance.mainPlayer.GetMoney() + reward);
	}

	public string GetDetails() {
		return details;
	}

	public int GetReward() {
		return reward;
	}
}
