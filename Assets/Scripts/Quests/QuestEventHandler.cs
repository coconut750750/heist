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

	public const int MAX_ACTIVE_QUESTS = 3;

	private List<Quest> activeQuests;

	public QuestEventHandler() {
		activeQuests = new List<Quest>();
		instance = this;
	}

	public bool CanAcceptQuest() {
		return activeQuests.Count < MAX_ACTIVE_QUESTS;
	}

	public void AddQuest(Quest quest) {
		if (!CanAcceptQuest()) {
			throw new QuestOverflowException();	
		}
		activeQuests.Add(quest);
	}

	public void DeleteQuest(Quest quest) {
		if (activeQuests.Contains(quest)) {
			activeQuests.Remove(quest);
		}
	}

	public void CompleteQuestStage(Quest quest) {
		activeQuests.Remove(quest);
	}
	
	public void OnStealItemQuest(NPC npc, Item item) {
		foreach (Quest quest in activeQuests) {
			if (quest.FulfillSteal(npc, item)) {
				quest.CompleteQuestStage();
				break;
			}
		}
	}

	public void OnCraftItemQuest(Item item) {
		foreach (Quest quest in activeQuests) {
			if (quest.FulfillCraft(item)) {
				quest.CompleteQuestStage();
				break;
			}
		}
	}

	public void OnDefeatNPCQuest(NPC npc) {
		foreach (Quest quest in activeQuests) {
			if (quest.FulfillDefeat(npc)) {
				quest.CompleteQuestStage();
				break;
			}
		}
	}

	public void OnSellItemQuest(NPC npc, Item item) {
		foreach (Quest quest in activeQuests) {
			if (quest.FulfillSell(npc, item)) {
				quest.CompleteQuestStage();
				break;
			}
		}
	}

	public Quest[] GetActiveQuests() {
		return activeQuests.ToArray();
	}
}

public class QuestOverflowException : System.Exception {

}