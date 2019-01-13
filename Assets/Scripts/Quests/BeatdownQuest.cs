using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BeatdownQuest : Quest {

    public const int NUM_STAGES = 2;
    public const int TARGETS_PER_STAGE = 4;
    public static List<string> takenNpcNames = new List<string>();

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

        List<string> exclude = new List<string>() {base.reporter.GetName()};
        exclude.AddRange(takenNpcNames);

        NPC[] targetNpcs1 = NPCSpawner.instance.GetRandomNpcs(1, exclude);
        stages[0] = new BeatdownQuestStage(targetNpcs1, 15);
        exclude.AddRange(targetNpcs1.Select(npc => npc.GetName()));

        NPC[] targetNpcs2 = NPCSpawner.instance.GetRandomNpcs(3, exclude);
        stages[1] = new BeatdownQuestStage(targetNpcs2, 25);

        takenNpcNames.AddRange(targetNpcs1.Select(npc => npc.GetName()));
        takenNpcNames.AddRange(targetNpcs2.Select(npc => npc.GetName()));

        targetNpcs1 = null;
        targetNpcs2 = null;

        return stages;
    }

    public override void Delete() {
        base.Delete();
        foreach (BeatdownQuestStage stage in base.stages) {
            stage.RemoveAllRequirements();
        }
    }

    public override bool FulfillDefeat(NPC npc) {
        BeatdownQuestStage stage = GetCurrentStage<BeatdownQuestStage>();
        return stage.FulfillsRequirement(npc);
	}

    public override QuestData SaveIntoData() {
        return new BeatdownQuestData(this);
    }

    public override void LoadFromData(QuestData data) {
        int numStages = data.stages.Length;

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
