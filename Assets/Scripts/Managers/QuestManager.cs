using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

	public const int MAX_OUTSTANDING_QUESTS = 5;

	public static QuestManager instance = null;

	private int outstandingQuests = 0;
	private QuestEventHandler eventHandler;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		eventHandler = new QuestEventHandler();
	}

	public Quest GetRandomQuest(NPC npc) {
		if (outstandingQuests >= MAX_OUTSTANDING_QUESTS) {
			return null;
		}
		outstandingQuests++;
		return new SellingQuest(npc);
	}

	public void OnAcceptQuest(Quest quest) {
		eventHandler.AddQuest(quest);
	}

	public void OnCompleteQuest(Quest quest) {
		eventHandler.CompleteQuest(quest);
		outstandingQuests--;
	}
}
