using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuestStage : QuestStage
{
    private const string QUEST_DETAILS = "Sell me a ";
    private string itemRequirement;

    public SellingQuestStage(string itemRequirement, int reward) :
                             base(QUEST_DETAILS + itemRequirement, reward) {
        this.itemRequirement = itemRequirement;
    }
 
    public SellingQuestStage(Item requirement, int reward) : 
                             base(QUEST_DETAILS + requirement.itemName, reward) {
        this.itemRequirement = requirement.itemName;
    }
 
    public bool FulfillsRequirement(Item item) {
        return item.itemName == itemRequirement;
    }

    public static SellingQuestStage GetQuestStageFromData(SellingQuestStageData data) {
        return new SellingQuestStage(data.itemRequirement, data.reward);
    }


    [System.Serializable]
	public class SellingQuestStageData : QuestStageData {
		public string itemRequirement;

		public SellingQuestStageData(SellingQuestStage stage) : base(stage) {
            this.itemRequirement = stage.itemRequirement;
		}
	}
}
