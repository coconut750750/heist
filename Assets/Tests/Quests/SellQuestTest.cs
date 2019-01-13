using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class SellQuestTest {

	[SetUp]
	public void LoadMainScene() {
		SceneManager.LoadScene("MainScene");
	}

	public SellController GetSellController() {
		return SellController.instance;
	}

	public NPC SpawnNPC() {
		NPCSpawner.instance.SpawnUnconditionally(new Vector2(0, 0));
		return NPCSpawner.instance.GetRandomNpc();
	}

	[UnityTest]
	public IEnumerator SimpleSellQuest() {
		yield return null;

		NPC npc = SpawnNPC();

		Quest sellingQuest = new SellingQuest(npc);
		npc.ReceiveQuest();
		QuestManager.instance.AddOutstandingQuest(sellingQuest);
		QuestManager.instance.AddActiveQuest(sellingQuest);

		Assert.AreEqual(1, QuestManager.instance.NumOutstanding());
		Assert.AreEqual(1, QuestManager.instance.NumActive());

		SellingQuestStage questStage = sellingQuest.GetCurrentStage<SellingQuestStage>();
		string itemReq = questStage.ItemRequirement();
		Item item = ItemManager.instance.GetItem(itemReq);

		SellController sellControl = GetSellController();		
		NPCTrade.instance.Display(npc);
		sellControl.SellItemEntered(item);
		sellControl.Sell(npc);

		yield return null;

		Assert.True(QuestCompletionMenu.instance.isActiveAndEnabled);
		
		Assert.AreEqual(0, QuestManager.instance.NumActive());
		Assert.AreEqual(1, QuestManager.instance.NumOutstanding());
		
		Assert.False(npc.GetInventory().ContainsItem(item));
	}

	[TearDown]
	public void UnloadMainScene() {
		GameManager.instance.save = false;
		SceneManager.UnloadSceneAsync("MainScene");
	}
}
