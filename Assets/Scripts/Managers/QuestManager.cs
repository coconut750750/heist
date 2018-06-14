using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

	public const int MAX_OUTSTANDING_QUESTS = 5;

	public static QuestManager instance = null;

	private int outstandingQuests = 0;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	public Quest GetRandomQuest(NPC npc) {
		if (outstandingQuests >= MAX_OUTSTANDING_QUESTS) {
			return null;
		}
		outstandingQuests++;
		return new SellingQuest(npc);
	}

	public void OnFinishedQuest() {
		outstandingQuests--;
	}
}
