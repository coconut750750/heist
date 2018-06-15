﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStage {

	private string details;

	private int reward;

	public QuestStage(string details, int reward) {
		this.details = details;
		this.reward = reward;
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
}
