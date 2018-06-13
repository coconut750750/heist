using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Quest {

	public string name;
	protected QuestStage[] stages;
	protected int currentStage;

	public NPC reporter;

	public Quest(NPC reporter, string questName) {
		this.reporter = reporter;
		this.name = questName;
		this.stages = GenerateQuestStages();
		currentStage = 0;
	}

	protected abstract QuestStage[] GenerateQuestStages();

	public QuestStage GetCurrentStage() {
		if (currentStage >= stages.Length) {
			return null;
		}
		return stages[currentStage];
	}

	public bool CompletedAll() {
		return currentStage >= stages.Length;
	}

	public virtual void CompleteQuestStage(Player player) {
		stages[currentStage].OnComplete(reporter, player);
		currentStage++;
	}
}
