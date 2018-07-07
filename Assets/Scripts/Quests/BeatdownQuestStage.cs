using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BeatdownQuestStage : QuestStage {

    private const string QUEST_DETAILS = "Beat up ";
	private string[] npcRequirements;
	private List<string> completedRequirements;
	private string originalDetails;

	public BeatdownQuestStage(string[] npcRequirements, string[] completed, int reward) :
                              base(reward) {
		this.npcRequirements = npcRequirements;
		this.completedRequirements = completed.ToList();
		this.originalDetails = GetDetails();
    }
 
    public BeatdownQuestStage(NPC[] requirements, int reward) : 
                              this(requirements.Select(npc => npc.GetName()).ToArray(), new string[]{}, reward) {
    }

	public override string GetDetails() {
		if (npcRequirements.Length == completedRequirements.Count) {
			return originalDetails; // restore original to present to Player
		}
		string details = QUEST_DETAILS;
		for (int i = 0; i < npcRequirements.Length; i++) {
			string req = npcRequirements[i];
			if (completedRequirements.Contains(req)) {
				details += "<color=silver>" + req + "</color>";
			} else {
				details += req;
			}
			if (i + 1 < npcRequirements.Length) {
				details += ", ";
			}
		}
		return details;
	}

	public bool FulfillsRequirement(NPC npc) {
		if (npcRequirements.Contains(npc.GetName())) {
			completedRequirements.Add(npc.GetName());
			BeatdownQuest.takenNpcNames.Remove(npc.GetName());
		}
		if (completedRequirements.Count == npcRequirements.Length) {
			return true;
		}
		return false;
	}

	public void RemoveAllRequirements() {
		BeatdownQuest.takenNpcNames.RemoveAll(npc => npcRequirements.Contains(npc));
	}

	public static BeatdownQuestStage LoadQuestStageFromData(BeatdownQuestStageData data) {
		BeatdownQuest.takenNpcNames.AddRange(data.npcRequirements.Where(req => !data.completedRequirements.Contains(req)));
        BeatdownQuestStage stage = new BeatdownQuestStage(data.npcRequirements, data.completedRequirements, data.reward);
		stage.originalDetails = data.originalDetails;
		return stage;
	}


	[System.Serializable]
	public class BeatdownQuestStageData : QuestStageData {
		public string[] npcRequirements;
		public string[] completedRequirements;
		public string originalDetails;

		public BeatdownQuestStageData(BeatdownQuestStage stage) : base(stage) {
			this.originalDetails = stage.originalDetails;
            this.npcRequirements = stage.npcRequirements;
			this.completedRequirements = stage.completedRequirements.ToArray();
		}
	}
}
