using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  	Each quest has a reporter and a number of stages (1, 2, 3, etc). This object
//  keeps track of the stages as well as what happens when a quest is accepted, 
//  rejected, and completed. 

public abstract class Quest {

	public string name;
	protected QuestStage[] stages;
	protected int currentStage;

	public NPC reporter;
	public string reporterNameFromLoad;
	private bool active;

	public Quest(NPC reporter, string questName) {
		this.reporter = reporter;
		this.name = questName;
		this.stages = GenerateQuestStages();
		currentStage = 0;
		active = false;
	}

	protected abstract QuestStage[] GenerateQuestStages();

	public void OnAccept() {
		try {
			QuestManager.instance.OnAcceptQuest(this);
		} catch (QuestOverflowException e) {
			throw e;
		}

		reporter.AcceptedQuest();
		
		active = true;
	}

	public void OnReject() {
		QuestManager.instance.OnRejectQuest(this);
		reporter.RejectedQuest();
	}

	public QuestStage GetCurrentStage() {
		if (HasCompletedAll()) {
			return null;
		}
		return stages[currentStage];
	}

	public string GetCurrentDetails() {
		return stages[currentStage].GetDetails();
	}

	public int GetCurrentReward() {
		return stages[currentStage].GetReward();
	}

	public bool HasCompletedAll() {
		return currentStage >= stages.Length;
	}

	public virtual void CompleteQuestStage() {
		QuestCompletionMenu.instance.Display(this);

		stages[currentStage].OnComplete(reporter);
		QuestManager.instance.OnCompleteQuestStage(this);

		currentStage++;
		active = false;
		if (HasCompletedAll()) {
			OnCompletedAll();
		}
	}

	public virtual void OnCompletedAll() {
		reporter.CompletedEntireQuest();
		QuestManager.instance.OnCompleteEntireQuest(this);
	}

	public virtual void OnStealItem(NPC npc, Item item) {
		return;
	}

	public virtual void OnCraftItem(Item item) {
		return;
	}

	public virtual void OnDefeatedNPC(NPC npc) {
		return;
	}

	public virtual void OnSellItem(NPC npc, Item item) {
		return;
	}

	public bool IsActive() {
		return active;
	}

	public static Quest GetQuestFromData(QuestData data) {
		if (data == null) {
			return null;
		}
		string questName = data.name;
		Quest returnQuest;
		if (questName == Constants.SELLING_QUEST) {
			returnQuest = new SellingQuest(null);
		} else {
			return null;
		}

		int numStages = data.stages.Length;
		returnQuest.stages = new QuestStage[numStages];
		for (int i = 0; i < numStages; i++) {
			returnQuest.stages[i] = QuestStage.GetQuestStageFromData(data.stages[i]);
		}

		returnQuest.reporterNameFromLoad = data.reporterName;
		returnQuest.currentStage = data.currentStage;
		returnQuest.active = data.active;

		return returnQuest;
	}

	[System.Serializable]
	public class QuestData : GameData {
		public string name;
		public QuestStage.QuestStageData[] stages;
		public string reporterName;
		public int currentStage;

		public bool active;

		public QuestData(Quest quest) {
			this.name = quest.name;
			QuestStage[] questStages = quest.stages;
			this.stages = new QuestStage.QuestStageData[questStages.Length];
			for (int i = 0; i < questStages.Length; i++) {
				this.stages[i] = new QuestStage.QuestStageData(questStages[i]);
			}
			Debug.Log(quest.reporter == null);
			this.reporterName = quest.reporter.GetName();
			this.currentStage = quest.currentStage;
			this.active = quest.active;
		}
	}
}
