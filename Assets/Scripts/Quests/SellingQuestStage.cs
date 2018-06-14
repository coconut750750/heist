using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuestStage : QuestStage
{
    private const string QUEST_DETAILS = "Sell me a ";
    private Item requirement;

    public SellingQuestStage(Item requirement, int reward) : 
                         base(QUEST_DETAILS + requirement.itemName, reward) {
        this.requirement = requirement;
    }

    public bool FulfillsRequirement(Item item) {
        return item.itemName == requirement.itemName;
    }
}
