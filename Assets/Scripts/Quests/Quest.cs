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

	public void OnAccept(NPC reporter) {
		reporter.AcceptedQuest();
	}

	public T GetCurrentStage<T>() where T : QuestStage {
		if (CompletedAll()) {
			return null;
		}
		return stages[currentStage] as T;
	}

	public bool CompletedAll() {
		return currentStage >= stages.Length;
	}

	public virtual void CompleteQuestStage() {
		stages[currentStage].OnComplete(reporter);
		currentStage++;
		if (CompletedAll()) {
			OnCompletedAll();
		}
	}

	public virtual void OnCompletedAll() {
		// TODO: Delete quest from quest manager
		QuestEventHandler.instance.OnCompleteQuest(this);
	}

	public abstract void OnStealItem(Player player, NPC npc, Item item);

	public abstract void OnCraftItem(Player player, Item item);

	public abstract void OnDefeatedNPC(Player player, NPC npc);

	public abstract void OnSellItem(Player player, NPC npc, Item item);
}
