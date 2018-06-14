using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 		Quest Event Handler maintains the active quests. When certain events happen
// 	in game, this handler calls the event functions for each active quests in case
// 	an active quest was accomplished due to the event. 
// 		Game facing objects use the instance to call event functions (such as sell controller)
//		Quest manager use the instance to add and complete quests
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
			throw new QuestOverflowException();
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

public class QuestOverflowException : System.Exception {

}