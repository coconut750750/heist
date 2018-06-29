using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatdownQuest : Quest {

    public const int NUM_STAGES = 2;

    public BeatdownQuest() {

    }

    public BeatdownQuest(NPC reporter) : base(reporter, Constants.BEATDOWN_QUEST)
    {
    }

    protected override QuestStage[] GenerateQuestStages() {
        if (base.reporter == null) {
            return null;
        }

        QuestStage[] stages = new QuestStage[NUM_STAGES];

        NPC[] targetNpcs = NPCSpawner.instance.GetRandomNpcs(1);
        stages[0] = new BeatdownQuestStage(targetNpcs, 15);
        
        targetNpcs = NPCSpawner.instance.GetRandomNpcs(3);
        stages[1] = new BeatdownQuestStage(targetNpcs, 25);

        return stages;
    }

    public override void OnDefeatedNPC(NPC npc) {
        if (GetCurrentStage<BeatdownQuestStage>().FulfillsRequirement(npc)) {
            CompleteQuestStage();
        }
	}

    public override QuestData SaveIntoData() {
        return new BeatdownQuestData(this);
    }

    public override void LoadFromData(QuestData data) {
        int numStages = data.stages.Length;

        this.name = Constants.SELLING_QUEST;
		this.stages = new BeatdownQuestStage[numStages];
		for (int i = 0; i < numStages; i++) {
			this.stages[i] = BeatdownQuestStage.LoadQuestStageFromData(data.stages[i] as BeatdownQuestStage.BeatdownQuestStageData);
		}

		this.currentStage = data.currentStage;
		this.active = data.active;
    }

    [System.Serializable]
	public class BeatdownQuestData : QuestData {

		public BeatdownQuestData(Quest quest) : base(quest) {

			QuestStage[] questStages = GetQuestStages(quest);
			this.stages = new QuestStage.QuestStageData[questStages.Length];
			
            for (int i = 0; i < questStages.Length; i++) {
				this.stages[i] = new BeatdownQuestStage.BeatdownQuestStageData(questStages[i] as BeatdownQuestStage);
			}
		}
	}
}
