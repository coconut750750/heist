using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuestStage : QuestStage
{
    private Item requirement;

    public SellingQuestStage(Item requirement, string details, int stageNum, int reward) : 
                         base(details, stageNum, reward) {
        this.requirement = requirement;
    }

    public bool FulfillsRequirement(Item item) {
        return item.itemName == requirement.itemName;
    }
}
