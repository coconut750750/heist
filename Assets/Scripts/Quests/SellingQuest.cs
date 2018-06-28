using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellingQuest : Quest {

    public const int NUM_STAGES = 3;

    public SellingQuest() {
        
    }

    public SellingQuest(NPC reporter) : base(reporter, Constants.SELLING_QUEST)
    {

    }

    protected override QuestStage[] GenerateQuestStages() {
        if (base.reporter == null) {
            return null;
        }
        QuestStage[] stages = new QuestStage[NUM_STAGES];

        Item common = ItemManager.instance.GetRandomCommonItem();
        stages[0] = new SellingQuestStage(common, 15);
        
        Item uncommon = ItemManager.instance.GetRandomUncommonItem();
        stages[1] = new SellingQuestStage(uncommon, 20);
        
        Item rare = ItemManager.instance.GetRandomRareItem();
        stages[2] = new SellingQuestStage(rare, 25);

        return stages;
    }

    public override void OnSellItem(NPC npc, Item item)
    {
        if (npc != reporter) {
            return;
        }
        if (GetCurrentStage<SellingQuestStage>().FulfillsRequirement(item)) {
            CompleteQuestStage();
        }
    }

    public override QuestData SaveIntoData() {
        return new SellingQuestData(this);
    }

    public static new SellingQuest GetQuestFromData(QuestData data) {
        SellingQuest returnQuest = new SellingQuest();

        int numStages = data.stages.Length;
		returnQuest.stages = new SellingQuestStage[numStages];
		for (int i = 0; i < numStages; i++) {
			returnQuest.stages[i] = SellingQuestStage.GetQuestStageFromData(data.stages[i] as SellingQuestStage.SellingQuestStageData);
		}

		returnQuest.reporterNameFromLoad = data.reporterName;
		returnQuest.currentStage = data.currentStage;
		returnQuest.active = data.active;

        return returnQuest;
    }

    [System.Serializable]
	public class SellingQuestData : QuestData {

		public SellingQuestData(Quest quest) : base(quest) {

			QuestStage[] questStages = GetQuestStages(quest);
			this.stages = new QuestStage.QuestStageData[questStages.Length];
			
            for (int i = 0; i < questStages.Length; i++) {
				this.stages[i] = new SellingQuestStage.SellingQuestStageData(questStages[i] as SellingQuestStage);
			}
		}
	}
}
