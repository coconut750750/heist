using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  	The Quest Manager keeps track of how many outstanding quests there are and 
//  gives NPC random quests on demand. It also is the interface between Quests themselves
//  and the Quest Event handler. 

public class QuestManager : MonoBehaviour {

	public const int MAX_OUTSTANDING_QUESTS = 5;
	public const string saveFile = "questmanager.dat";
	
	public static QuestManager instance = null;

	private List<Quest> outstandingQuests;

	private QuestEventHandler eventHandler;

	[SerializeField]
	private GameObject questOverflowAlert;
	
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		eventHandler = new QuestEventHandler();
		questOverflowAlert.SetActive(false);
		outstandingQuests = new List<Quest>();
	}

	public Quest GetRandomQuest(NPC npc) {
		if (outstandingQuests.Count >= MAX_OUTSTANDING_QUESTS) {
			return null;
		}
		Quest quest = new SellingQuest(npc);
		outstandingQuests.Add(quest);
		return quest;
	}

	public Quest GetCurrentQuest(NPC npc) {
		foreach (Quest outstanding in outstandingQuests) {
			if (outstanding.reporter == npc) {
				return outstanding;
			}
		}
		return null;
	}

	public void OnAcceptQuest(Quest quest) {
		try {
			eventHandler.AddQuest(quest);
			ActiveQuestMenu.instance.AddActiveQuest(quest);
		} catch (QuestOverflowException e) {
			questOverflowAlert.SetActive(true);
			throw e;
		}
	}

	public void OnRejectQuest(Quest quest) {
		outstandingQuests.Remove(quest);
	}

	public void OnCompleteQuestStage(Quest quest) {
		eventHandler.CompleteQuestStage(quest);	
		ActiveQuestMenu.instance.RemoveActiveQuest(quest);
	}

	public void OnCompleteEntireQuest(Quest quest) {
		outstandingQuests.Remove(quest);
	}

	[System.Serializable]
	public class QuestManagerData : GameData {
		List<Quest.QuestData> outstandingQuests;

		public QuestManagerData(QuestManager questManager) {
			outstandingQuests = new List<Quest.QuestData>();
			foreach (Quest quest in questManager.outstandingQuests) {
				outstandingQuests.Add(new Quest.QuestData(quest));
			}
		}
	}
}
