using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEventHandler {

	public static QuestEventHandler instance = null;

	public const int TOTAL_ACTIVE_QUESTS = 3;

	private List<Quest> quests;
	
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
		Debug.Log("ACCEPTED");
		quests.Add(quest);
	}

	public void CompleteQuest(Quest quest) {
		quests.Remove(quest);
		quest = null;
	}
	
	public void OnStealItem(NPC npc, Item item) {
		foreach (Quest quest in quests) {
			quest.OnStealItem(npc, item);
		}
	}

	public void OnCraftItem(Item item) {
		foreach (Quest quest in quests) {
			quest.OnCraftItem(item);
		}
	}

	public void OnDefeatedNPC(NPC npc) {
		foreach (Quest quest in quests) {
			quest.OnDefeatedNPC(npc);
		}
	}

	public void OnSellItem(NPC npc, Item item) {
		foreach (Quest quest in quests) {
			quest.OnSellItem(npc, item);
		}
	}
}
