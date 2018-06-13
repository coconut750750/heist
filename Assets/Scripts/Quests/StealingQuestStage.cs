using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealingQuestStage : QuestStage
{
    private Item requirement;

    public StealingQuestStage(Item requirement, string details, int stageNum, int reward) : 
                         base(details, stageNum, reward) {
        this.requirement = requirement;
    }
}
