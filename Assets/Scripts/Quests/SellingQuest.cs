using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuest : Quest {

    public const int NUM_STAGES = 2;
    private const string QUEST_DETAILS = "Sell me a ";

    public SellingQuest(NPC reporter) : base(reporter, Constants.SELLING_QUEST)
    {

    }

    protected override QuestStage[] GenerateQuestStages() {
        if (base.reporter == null) {
            return null;
        }
        QuestStage[] stages = new QuestStage[NUM_STAGES];
        stages[0] = new QuestStage(QUEST_DETAILS + "Apple", 15, "Apple", base.reporter.GetName());
        stages[1] = new QuestStage(QUEST_DETAILS + "Apple", 20, "Apple", base.reporter.GetName());

        return stages;
    }

    public override void OnSellItem(NPC npc, Item item)
    {
        if (GetCurrentStage().FulfillsRequirement(item, npc)) {
            CompleteQuestStage();
        }
    }
}
