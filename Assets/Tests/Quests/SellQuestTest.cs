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
		NPC[] npcs = NPCSpawner.instance.GetNpcs();
		return npcs[npcs.Length - 1];
	}

	public SellingQuest CreateNewSellingQuest(NPC npc) {
		SellingQuest sellingQuest = new SellingQuest(npc);
		npc.ReceiveQuest();
		QuestManager.instance.AddOutstandingQuest(sellingQuest);

		return sellingQuest;
	}

	public void ActivateQuest(Quest q) {
		QuestManager.instance.AddActiveQuest(q);
	}

	public Item GetQuestItem(SellingQuest quest) {
		SellingQuestStage questStage = quest.GetCurrentStage<SellingQuestStage>();
		string itemReq = questStage.ItemRequirement();
		return ItemManager.instance.GetItem(itemReq);
	}

	public void Sell(NPC npc, Item item) {
		NPCTrade.instance.Display(npc);
		SellController.instance.SellItemEntered(item);
		SellController.instance.Sell(npc);
	}

	[UnityTest]
	public IEnumerator SimpleSellQuest() {
		yield return null;

		NPC npc = SpawnNPC();
		npc.GetInventory().RemoveAll();

		SellingQuest sellingQuest = CreateNewSellingQuest(npc);
		ActivateQuest(sellingQuest);
		Assert.AreEqual(1, QuestManager.instance.NumOutstanding());
		Assert.AreEqual(1, QuestManager.instance.NumActive());
		Item item = GetQuestItem(sellingQuest);

		Sell(npc, item);

		yield return null;

		Assert.AreEqual(0, QuestManager.instance.NumActive());
		Assert.AreEqual(1, QuestManager.instance.NumOutstanding());
		Assert.True(QuestCompletionMenu.instance.isActiveAndEnabled);
		Assert.False(npc.GetInventory().ContainsItem(item));
	}

	[UnityTest]
	public IEnumerator CompleteSellQuest() {
		yield return null;

		NPC npc = SpawnNPC();
		npc.GetInventory().RemoveAll();

		SellingQuest sellingQuest = CreateNewSellingQuest(npc);

		for (int i = 0; i < SellingQuest.NUM_STAGES; i++) {
			ActivateQuest(sellingQuest);

			Item item = GetQuestItem(sellingQuest);
			Sell(npc, item);
			yield return null;

			Assert.False(npc.GetInventory().ContainsItem(item));
		}

		Assert.AreEqual(0, QuestManager.instance.NumActive());
		Assert.AreEqual(0, QuestManager.instance.NumOutstanding());
	}

	[UnityTest]
	public IEnumerator UnsuccessfulSellQuestWrongItem() {
		yield return null;

		NPC npc = SpawnNPC();
		npc.GetInventory().RemoveAll();

		SellingQuest sellingQuest = CreateNewSellingQuest(npc);
		ActivateQuest(sellingQuest);

		Item questItem = GetQuestItem(sellingQuest);
		Item otherItem = ItemManager.instance.GetRandomCommonItem(new Item[]{questItem});

		Sell(npc, otherItem);

		yield return null;
		Assert.True(npc.GetInventory().ContainsItem(otherItem));
		Assert.False(QuestCompletionMenu.instance.isActiveAndEnabled);

		Assert.AreEqual(1, QuestManager.instance.NumActive());
		Assert.AreEqual(1, QuestManager.instance.NumOutstanding());
	}

	[UnityTest]
	public IEnumerator UnsuccessfulSellQuestWrongNpc() {
		yield return null;

		NPC npc = SpawnNPC();
		npc.GetInventory().RemoveAll();

		NPC npc2 = SpawnNPC();
		npc2.GetInventory().RemoveAll();

		SellingQuest sellingQuest = CreateNewSellingQuest(npc);
		ActivateQuest(sellingQuest);

		Item item = GetQuestItem(sellingQuest);

		Sell(npc2, item);

		yield return null;
		Assert.True(npc2.GetInventory().ContainsItem(item));
		Assert.False(QuestCompletionMenu.instance.isActiveAndEnabled);

		Assert.AreEqual(1, QuestManager.instance.NumActive());
	}

	[TearDown]
	public void UnloadMainScene() {
		GameManager.instance.save = false;
		SceneManager.UnloadSceneAsync("MainScene");
	}
}
