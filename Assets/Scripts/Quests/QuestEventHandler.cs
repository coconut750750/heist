using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEventHandler {

	public static QuestEventHandler instance = null;

	public const int TOTAL_ACTIVE_QUESTS = 3;

	private List<Quest> quests;

	private bool iterationHadCompletedQuest;
	
	public QuestEventHandler() {
		quests = new List<Quest>();
		instance = this;
	}

	public bool CanAcceptQuest() {
		return quests.Count < TOTAL_ACTIVE_QUESTS;
	}

	public void AddQuest(Quest quest) {
		if (!CanAcceptQuest()) {
			// TODO: throw error
			return;
		}
		quests.Add(quest);
	}

	public void CompleteQuest(Quest quest) {
		iterationHadCompletedQuest = true;
		quests.Remove(quest);
		quest = null;
	}
	
	public void OnStealItem(NPC npc, Item item) {
		iterationHadCompletedQuest = false;
		foreach (Quest quest in quests) {
			quest.OnStealItem(npc, item);
			if (iterationHadCompletedQuest) {
				return;
			}
		}
	}

	public void OnCraftItem(Item item) {
		iterationHadCompletedQuest = false;
		foreach (Quest quest in quests) {
			quest.OnCraftItem(item);
			if (iterationHadCompletedQuest) {
				return;
			}
		}
	}

	public void OnDefeatedNPC(NPC npc) {
		iterationHadCompletedQuest = false;
		foreach (Quest quest in quests) {
			quest.OnDefeatedNPC(npc);
			if (iterationHadCompletedQuest) {
				return;
			}
		}
	}

	public void OnSellItem(NPC npc, Item item) {
		iterationHadCompletedQuest = false;
		foreach (Quest quest in quests) {
			quest.OnSellItem(npc, item);
			if (iterationHadCompletedQuest) {
				return;
			}
		}
	}
}
