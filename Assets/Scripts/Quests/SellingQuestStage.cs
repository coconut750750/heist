using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuestStage : QuestStage
{
    private const string QUEST_DETAILS = "Sell me a ";
    private string itemRequirement;

    public SellingQuestStage(string itemRequirement, int reward) :
                             base(reward) {
        this.itemRequirement = itemRequirement;
    }
 
    public SellingQuestStage(Item requirement, int reward) : 
                             this(requirement.itemName, reward) {
    }
 
    public bool FulfillsRequirement(Item item) {
        return item.itemName == itemRequirement;
    }

    public override string GetDetails() {
        return QUEST_DETAILS + itemRequirement;
    }

    public static SellingQuestStage LoadQuestStageFromData(SellingQuestStageData data) {
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
