using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestStage {
 	
	protected int reward;
	
	public QuestStage(int reward) {
		this.reward = reward;
	}
	
	public void OnComplete(NPC reporter) {
		reporter.CompletedQuestStage();
		GameManager.instance.mainPlayer.SetMoney(GameManager.instance.mainPlayer.GetMoney() + reward);
	}
	
	public abstract string GetDetails();
	
	public int GetReward() {
		return reward;
	}

	[System.Serializable]
	public abstract class QuestStageData : GameData {
		public int reward;

		public QuestStageData(QuestStage stage) {
			this.reward = stage.GetReward();
		}
	}
}