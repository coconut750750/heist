using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  	The Quest Manager keeps track of how many outstanding quests there are and 
//  gives NPC random quests on demand. It also is the interface between Quests themselves
//  and the Quest Event handler. 

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
		try {
			eventHandler.AddQuest(quest);
		} catch (QuestOverflowException e) {
			throw e;
		}
	}

	public void OnRejectQuest(Quest quest) {
		outstandingQuests--;
	}

	public void OnCompleteQuestStage(Quest quest) {
		eventHandler.CompleteQuestStage(quest);	
	}

	public void OnCompleteEntireQuest() {
		outstandingQuests--;
	}
}
