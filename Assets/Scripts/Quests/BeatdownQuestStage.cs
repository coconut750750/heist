using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatdownQuestStage : QuestStage {

    private const string QUEST_DETAILS = "Beat up ";
	private List<string> npcRequirements = new List<string>();

	public BeatdownQuestStage(string[] npcRequirements, int reward) :
                             base(QUEST_DETAILS, reward) {
		InitDetails(npcRequirements);
		foreach (string name in npcRequirements) {
			this.npcRequirements.Add(name);
		}
    }
 
    public BeatdownQuestStage(NPC[] requirements, int reward) : 
                             base(QUEST_DETAILS, reward) {
		string[] npcNames = new string[requirements.Length];
		for (int i = 0; i < requirements.Length; i++) {
			string name = requirements[i].GetName();
			npcNames[i] = name;
			this.npcRequirements.Add(name);
		}
        InitDetails(npcNames);
    }

	private void InitDetails(string[] npcNames) {
		for (int i = 0; i < npcNames.Length; i++) {
			details += npcNames[i];
			if (i + 1 < npcNames.Length) {
				details += " ,";
			}
		}
	}

	public bool FulfillsRequirement(NPC npc) {
		if (npcRequirements.Contains(npc.GetName())) {
			npcRequirements.Remove(npc.GetName());
		}
		if (npcRequirements.Count <= 0) {
			return true;
		}
		return false;
	}

	public static BeatdownQuestStage LoadQuestStageFromData(BeatdownQuestStageData data) {
        return new BeatdownQuestStage(data.npcRequirements, data.reward);
    }


	[System.Serializable]
	public class BeatdownQuestStageData : QuestStageData {
		public string[] npcRequirements;

		public BeatdownQuestStageData(BeatdownQuestStage stage) : base(stage) {
            this.npcRequirements = stage.npcRequirements.ToArray();
		}
	}
}
