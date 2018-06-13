using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStage {

	public string details;
	private int stageNum;

	public int reward;

	public QuestStage(string details, int stageNum, int reward) {
		this.details = details;
		this.stageNum = stageNum;
		this.reward = reward;
	}

	public void OnComplete(NPC reporter, Player player) {
		reporter.CompletedQuest();
		player.SetMoney(player.GetMoney() + reward);
	}

	public void OnAccept(NPC reporter, Player player) {
		reporter.AcceptedQuest();
	}
}
