using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStage {

	public string details;
	private int stageNum;

	public int reward;

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
}
