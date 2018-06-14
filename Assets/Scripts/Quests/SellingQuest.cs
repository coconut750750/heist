using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuest : Quest {

	public const string QUEST_NAME = "stealing_quest";
    public const int NUM_STAGES = 1;

    public SellingQuest(NPC reporter) : base(reporter, QUEST_NAME)
    {

    }

    protected override QuestStage[] GenerateQuestStages() {
        QuestStage[] stages = new QuestStage[NUM_STAGES];
        stages[0] = new SellingQuestStage(ItemManager.instance.GetItem("Apple"),
                                            "Steal an Apple.", 0, 15);

        return stages;
    }

    public override void OnCraftItem(Player player, Item item)
    {
        return;
    }

    public override void OnDefeatedNPC(Player player, NPC npc)
    {
        return;
    }

    public override void OnSellItem(Player player, NPC npc, Item item)
    {
        if (GetCurrentStage<SellingQuestStage>().FulfillsRequirement(item)) {
            CompleteQuestStage();
        }
    }

    public override void OnStealItem(Player player, NPC npc, Item item)
    {
        return;
    }

}
