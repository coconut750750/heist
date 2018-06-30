using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatdownQuest : Quest {

    public const int NUM_STAGES = 2;
    public const int TARGETS_PER_STAGE = 4;
    public static List<NPC> takenNpcs = new List<NPC>();

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

        List<NPC> exclude = new List<NPC>() {base.reporter};
        exclude.AddRange(takenNpcs);

        NPC[] targetNpcs1 = NPCSpawner.instance.GetRandomNpcs(1, exclude);
        stages[0] = new BeatdownQuestStage(targetNpcs1, 15);
        exclude.AddRange(targetNpcs1);

        NPC[] targetNpcs2 = NPCSpawner.instance.GetRandomNpcs(3, exclude);
        stages[1] = new BeatdownQuestStage(targetNpcs2, 25);

        takenNpcs.AddRange(targetNpcs1);
        takenNpcs.AddRange(targetNpcs2);

        targetNpcs1 = null;
        targetNpcs2 = null;

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
