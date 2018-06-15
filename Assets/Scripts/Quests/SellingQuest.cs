using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuest : Quest {

	public const string QUEST_NAME = "stealing_quest";
    public const int NUM_STAGES = 2;

    public SellingQuest(NPC reporter) : base(reporter, QUEST_NAME)
    {

    }

    protected override QuestStage[] GenerateQuestStages() {
        QuestStage[] stages = new QuestStage[NUM_STAGES];
        stages[0] = new SellingQuestStage(ItemManager.instance.GetItem("Apple"), 15);
        stages[1] = new SellingQuestStage(ItemManager.instance.GetItem("Apple"), 20);

        return stages;
    }

    public override void OnCraftItem(Item item)
    {
        return;
    }

    public override void OnDefeatedNPC(NPC npc)
    {
        return;
    }

    public override void OnSellItem(NPC npc, Item item)
    {
        if (npc != reporter) {
            return;
        }
        if (GetCurrentStage<SellingQuestStage>().FulfillsRequirement(item)) {
            Debug.Log("here");
            CompleteQuestStage();
        }
    }

    public override void OnStealItem(NPC npc, Item item)
    {
        return;
    }

}
