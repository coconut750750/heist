using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEventHandler : MonoBehaviour {

	public const int TOTAL_ACTIVE_QUESTS = 3;

	public static QuestEventHandler instance = null;

	private List<Quest> quests = new List<Quest>();
	
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	public bool CanAcceptQuest() {
		return quests.Count < TOTAL_ACTIVE_QUESTS;
	}

	public void OnAcceptQuest(Quest quest) {
		if (!CanAcceptQuest()) {
			// TODO: throw error
			return;
		}
		quests.Add(quest);
	}

	public void OnCompleteQuest(Quest quest) {
		quests.Remove(quest);
		quest = null;
		QuestManager.instance.OnFinishedQuest();
	}
	
	public void OnStealItem(Player player, NPC npc, Item item) {
		foreach (Quest quest in quests) {
			quest.OnStealItem(player, npc, item);
		}
	}

	public void OnCraftItem(Player player, Item item) {
		foreach (Quest quest in quests) {
			quest.OnCraftItem(player, item);
		}
	}

	public void OnDefeatedNPC(Player player, NPC npc) {
		foreach (Quest quest in quests) {
			quest.OnDefeatedNPC(player, npc);
		}
	}

	public void OnSellItem(Player player, NPC npc, Item item) {
		foreach (Quest quest in quests) {
			quest.OnSellItem(player, npc, item);
		}
	}
}
