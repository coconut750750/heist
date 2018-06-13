using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEventHandler : MonoBehaviour {

	public const int TOTAL_ACTIVE_QUESTS = 3;

	public static QuestEventHandler instance = null;

	private Quest[] quests = new Quest[TOTAL_ACTIVE_QUESTS];
	private int numActiveQuests = 0;
	
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	public void OnAcceptQuest(Quest quest) {
		if (numActiveQuests == TOTAL_ACTIVE_QUESTS) {
			return;
		}
		quests[numActiveQuests] = quest;
		numActiveQuests++;
	}
	
	public void OnStealItem(Player player, NPC npc) {

	}

	public void OnCraftItem(Player player) {
		
	}

	public void OnDefeatedNPC(Player player, NPC npc) {

	}
}
