using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuest : Quest {

    public const int NUM_STAGES = 2;

    public SellingQuest(NPC reporter) : base(reporter, Constants.SELLING_QUEST)
    {

    }

    protected override QuestStage[] GenerateQuestStages() {
        QuestStage[] stages = new QuestStage[NUM_STAGES];
        stages[0] = new SellingQuestStage("Apple", 15);
        stages[1] = new SellingQuestStage("Apple", 20);

        return stages;
    }

    public override void OnSellItem(NPC npc, Item item)
    {
        if (npc != reporter) {
            return;
        }
        if (GetCurrentStage<SellingQuestStage>().FulfillsRequirement(item)) {
            CompleteQuestStage();
        }
    }
}
