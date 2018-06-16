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
		QuestCompletionMenu.instance.Display(stages[currentStage], reporter);

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
		QuestManager.instance.OnCompleteEntireQuest();
	}

	public abstract void OnStealItem(NPC npc, Item item);

	public abstract void OnCraftItem(Item item);

	public abstract void OnDefeatedNPC(NPC npc);

	public abstract void OnSellItem(NPC npc, Item item);

	public bool IsActive() {
		return active;
	}
}
