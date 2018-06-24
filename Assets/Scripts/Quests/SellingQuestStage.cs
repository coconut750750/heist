using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuestStage : QuestStage
{
    private const string QUEST_DETAILS = "Sell me a ";

    public SellingQuestStage(string requirementName, int reward) : 
                         base(QUEST_DETAILS + requirementName, reward, requirementName, "") {
    }

    public bool FulfillsRequirement(Item item) {
        return item.itemName == base.itemRequirement;
    }
}
