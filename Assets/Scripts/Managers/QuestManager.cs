using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  	The Quest Manager keeps track of how many outstanding quests there are and 
//  gives NPC random quests on demand. It also is the interface between Quests themselves
//  and the Quest Event handler. 

public class QuestManager : MonoBehaviour {

	public const int MAX_OUTSTANDING_QUESTS = 5;
	public const string SAVE_FILE = "questmanager.dat";

	public static QuestManager instance = null;

	public List<Quest> outstandingQuests;

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
		Quest quest;
		int i = Random.Range(0, 2);
		if (i == 0) {
			quest = new SellingQuest(npc);
		} else {
			if (NPCSpawner.instance.NumNpcs() > 
				BeatdownQuest.TARGETS_PER_STAGE + BeatdownQuest.takenNpcNames.Count) {
				quest = new BeatdownQuest(npc);
			} else {
				return null;
			}
		}
		return quest;
	}

	public int NumOutstanding() {
		return outstandingQuests.Count;
	}

	public int NumActive() {
		return eventHandler.GetActiveQuests().Length;
	}

	void Update() {
		if (outstandingQuests.Count < MAX_OUTSTANDING_QUESTS &&
			NPCSpawner.instance.NumNpcs() > 0) {
			NPC npc = NPCSpawner.instance.GetRandomNpc();
			if (!npc.hasQuest) {
				Quest newQuest = GetRandomQuest(npc);
				if (newQuest != null) {
					npc.ReceiveQuest();
					AddOutstandingQuest(newQuest);
				}
			}
		}
	}

	public Quest GetCurrentQuest(NPC npc) {
		foreach (Quest outstanding in outstandingQuests) {
			if (outstanding.reporter == npc) {
				return outstanding;
			}
		}
		return null;
	}

	public void AddOutstandingQuest(Quest quest) {
		outstandingQuests.Add(quest);
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

	public void DeleteQuest(Quest quest) {
		outstandingQuests.Remove(quest);
		eventHandler.DeleteQuest(quest);
		ActiveQuestMenu.instance.RemoveActiveQuest(quest);
	}

	public void OnCompleteQuestStage(Quest quest) {
		eventHandler.CompleteQuestStage(quest);	
		ActiveQuestMenu.instance.RemoveActiveQuest(quest);
	}

	public void OnCompleteEntireQuest(Quest quest) {
		outstandingQuests.Remove(quest);
	}

	public void Save() {
		string saveFile = Application.persistentDataPath + "/" + SAVE_FILE;
		QuestManagerData data = new QuestManagerData(this);
		GameManager.Save(data, saveFile);
	}

	public void Load() {
		string saveFile = Application.persistentDataPath + "/" + SAVE_FILE;

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
