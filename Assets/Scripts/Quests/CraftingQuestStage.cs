using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingQuestStage : QuestStage
{
    private Item requirement;

    public CraftingQuestStage(Item requirement, string details, int stageNum, int reward) : 
                         base(details, stageNum, reward) {
        this.requirement = requirement;
    }
}
