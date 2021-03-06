﻿using System.Collections;
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
	protected bool active;

	public Quest() {
		this.reporter = null;
	}

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
			QuestManager.instance.AddActiveQuest(this);
		} catch (QuestOverflowException e) {
			throw e;
		}

		reporter.AcceptedQuestStage();
		
		active = true;
	}

	public virtual void OnReject() {
		Delete();
		reporter.RejectedQuest();
	}

	public virtual void Delete() {
		QuestManager.instance.DeleteQuest(this);
	}

	public T GetCurrentStage<T>() where T : QuestStage {
		if (HasCompletedAll()) {
			return null;
		}
		return stages[currentStage] as T;
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

	public abstract QuestData SaveIntoData();

	public abstract void LoadFromData(QuestData data);

	public static Quest LoadQuestFromData(QuestData data) {
		if (data == null) {
			return null;
		}
		string questName = data.name;

		Quest returnQuest;
		if (questName == Constants.SELLING_QUEST) {
			returnQuest = new SellingQuest();
		} else if (questName == Constants.BEATDOWN_QUEST) {
			returnQuest = new BeatdownQuest();
		} else {
			throw new InvalidQuestNameException();
		}
		
		returnQuest.LoadFromData(data);
		returnQuest.name = questName;
		return returnQuest;
	}

	[System.Serializable]
	public abstract class QuestData : GameData {
		public string name;
		public QuestStage.QuestStageData[] stages;
		public string reporterName;
		public int currentStage;

		public bool active;

		public QuestData(Quest quest) {
			this.name = quest.name;
			this.stages = new QuestStage.QuestStageData[quest.stages.Length];
			this.reporterName = quest.reporter.GetName();
			this.currentStage = quest.currentStage;
			this.active = quest.active;
		}

		public QuestStage[] GetQuestStages(Quest quest) {
			return quest.stages;
		}
	}

	public class InvalidQuestNameException : System.Exception {

	}
}
