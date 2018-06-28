using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  	The Quest Manager keeps track of how many outstanding quests there are and 
//  gives NPC random quests on demand. It also is the interface between Quests themselves
//  and the Quest Event handler. 

public class QuestManager : MonoBehaviour {

	public const int MAX_OUTSTANDING_QUESTS = 5;
	public static string saveFile = "questmanager.dat";

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

	void RemoveQuestsWithoutReporter() {
		// safety measure in case npc data gets corrupted
		for (int i = outstandingQuests.Count - 1; i >= 0; i--) {
			if (outstandingQuests[i].reporter == null) {
				outstandingQuests.RemoveAt(i);
			}
		}
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

	public void AddActiveQuest(Quest quest) {
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

	public void Save() {
		RemoveQuestsWithoutReporter();
		QuestManagerData data = new QuestManagerData(this);
		GameManager.Save(data, saveFile);
	}

	public void Load() {
		saveFile = Application.persistentDataPath + "/" + saveFile;

		QuestManagerData data = GameManager.Load<QuestManagerData>(saveFile);
		if (data == null) {
			return;
		}
		foreach (Quest.QuestData questData in data.outstandingQuests) {
			Quest quest = Quest.LoadQuestFromData(questData);
			quest.reporter = NPCSpawner.instance.GetNpcByName(questData.reporterName);
			outstandingQuests.Add(quest);

			if (quest.IsActive()) {
				AddActiveQuest(quest);
			}
		}
	}

	[System.Serializable]
	public class QuestManagerData : GameData {
		public Quest.QuestData[] outstandingQuests;

		public QuestManagerData(QuestManager questManager) {
			int numQuests = questManager.outstandingQuests.Count;
			outstandingQuests = new Quest.QuestData[numQuests];
			for (int i = 0; i < numQuests; i++) {
				outstandingQuests[i] = questManager.outstandingQuests[i].SaveIntoData();
			}
		}
	}
}
