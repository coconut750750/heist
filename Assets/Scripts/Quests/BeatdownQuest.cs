using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatdownQuest { //: Quest {

    // public const int NUM_STAGES = 2;
    // private const string QUEST_DETAILS = "Beat up ";	

    // public BeatdownQuest(NPC reporter) : base(reporter, Constants.BEATDOWN_QUEST)
    // {
    // }

    // protected override QuestStage[] GenerateQuestStages() {
    //     if (base.reporter == null) {
    //         return null;
    //     }

    //     QuestStage[] stages = new QuestStage[NUM_STAGES];

    //     NPC[] targetNpcs = NPCSpawner.instance.GetRandomNpcs(4);

    //     Item common = ItemManager.instance.GetRandomCommonItem();
    //     // stages[0] = new QuestStage(QUEST_DETAILS + targetNpcs[0].GetName(), 15, "", targetNpcs[0].GetName());
        
    //     // Item uncommon = ItemManager.instance.GetRandomUncommonItem();
    //     // stages[1] = new QuestStage(QUEST_DETAILS + targetNpcs[1].GetName() + ", " +
    //     //                                            targetNpcs[2].GetName() + ", and " +
    //     //                                            targetNpcs[3].GetName(),
    //     //                            25, "", base.reporter.GetName());

    //     return stages;
    // }

    // public override void OnDefeatedNPC(NPC npc) {
    //     if (GetCurrentStage().FulfillsRequirement(null, npc)) {
    //         CompleteQuestStage();
    //     }
	// }
}
