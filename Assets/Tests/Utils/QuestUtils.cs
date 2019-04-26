using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class QuestUtils {
	public static void CreateNewSellingQuest(NPC npc) {
		SellingQuest sellingQuest = new SellingQuest(npc);
		npc.ReceiveQuest();
		QuestManager.instance.AddOutstandingQuest(sellingQuest);
	}

	public static void ActivateQuest(Quest q) {
		QuestManager.instance.AddActiveQuest(q);
	}

	public static Item GetQuestItem(SellingQuest quest) {
		SellingQuestStage questStage = quest.GetCurrentStage<SellingQuestStage>();
		string itemReq = questStage.ItemRequirement();
		return ItemManager.instance.GetItem(itemReq);
	}
}
