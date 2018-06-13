using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingQuest : Quest {

	public const string QUEST_NAME = "crafting_quest";
    public const int NUM_STAGES = 3;

    public CraftingQuest(NPC reporter) : base(reporter, QUEST_NAME)
    {

    }

    protected override QuestStage[] GenerateQuestStages() {
        QuestStage[] stages = new QuestStage[NUM_STAGES];
        stages[0] = new CraftingQuestStage(ItemManager.instance.GetRandomItem(),
                                            "Craft this item.", 0, 15);

        return stages;
    }

}
