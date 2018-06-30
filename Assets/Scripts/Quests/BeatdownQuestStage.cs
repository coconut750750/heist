using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BeatdownQuestStage : QuestStage {

    private const string QUEST_DETAILS = "Beat up ";
	private List<string> npcRequirements;

	public BeatdownQuestStage(string[] npcRequirements, int reward) :
                              base(QUEST_DETAILS, reward) {
		this.npcRequirements = npcRequirements.ToList();
		InitDetails(npcRequirements);
    }
 
    public BeatdownQuestStage(NPC[] requirements, int reward) : 
                              base(QUEST_DETAILS, reward) {
		this.npcRequirements = requirements.Select(npc => npc.GetName()).ToList();
        InitDetails(npcRequirements.ToArray());
    }

	private void InitDetails(string[] npcNames) {
		for (int i = 0; i < npcNames.Length; i++) {
			details += npcNames[i];
			if (i + 1 < npcNames.Length) {
				details += ", ";
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
		BeatdownQuest.takenNpcNames.AddRange(data.npcRequirements);
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
