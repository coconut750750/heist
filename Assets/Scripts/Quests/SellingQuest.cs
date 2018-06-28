using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuest : Quest {

    public const int NUM_STAGES = 3;
    private const string QUEST_DETAILS = "Sell me a ";

    public SellingQuest(NPC reporter) : base(reporter, Constants.SELLING_QUEST)
    {

    }

    protected override QuestStage[] GenerateQuestStages() {
        if (base.reporter == null) {
            return null;
        }
        QuestStage[] stages = new QuestStage[NUM_STAGES];

        Item common = ItemManager.instance.GetRandomCommonItem();
        stages[0] = new QuestStage(QUEST_DETAILS + common.itemName, 15, common.itemName, base.reporter.GetName());
        
        Item uncommon = ItemManager.instance.GetRandomUncommonItem();
        stages[1] = new QuestStage(QUEST_DETAILS + uncommon.itemName, 20, uncommon.itemName, base.reporter.GetName());
        
        Item rare = ItemManager.instance.GetRandomRareItem();
        stages[2] = new QuestStage(QUEST_DETAILS + rare.itemName, 25, rare.itemName, base.reporter.GetName());

        return stages;
    }

    public override void OnSellItem(NPC npc, Item item)
    {
        if (GetCurrentStage().FulfillsRequirement(item, npc)) {
            CompleteQuestStage();
        }
    }
}
