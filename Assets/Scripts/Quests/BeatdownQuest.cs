using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatdownQuest : Quest {

    public const int NUM_STAGES = 3;
    private const string QUEST_DETAILS = "Beat up ";	

    public BeatdownQuest(NPC reporter) : base(reporter, Constants.BEATDOWN_QUEST)
    {
    }

    protected override QuestStage[] GenerateQuestStages() {
        if (base.reporter == null) {
            return null;
        }

        QuestStage[] stages = new QuestStage[NUM_STAGES];

        

        return stages;
    }

    public override void OnDefeatedNPC(NPC npc) {
        if (GetCurrentStage().FulfillsRequirement(null, npc)) {
            CompleteQuestStage();
        }
	}
}
