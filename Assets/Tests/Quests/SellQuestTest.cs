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

	[UnityTest]
	public IEnumerator SimpleSellQuest() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		npc.GetInventory().RemoveAll();

		QuestUtils.CreateNewSellingQuest(npc);
		SellingQuest sellingQuest = (SellingQuest) QuestManager.instance.outstandingQuests[0];

		QuestUtils.ActivateQuest(sellingQuest);
		Assert.AreEqual(1, QuestManager.instance.NumOutstanding());
		Assert.AreEqual(1, QuestManager.instance.NumActive());
		Item item = QuestUtils.GetQuestItem(sellingQuest);

		NPCInteractUtils.Sell(npc, item);

		yield return null;

		Assert.AreEqual(0, QuestManager.instance.NumActive());
		Assert.AreEqual(1, QuestManager.instance.NumOutstanding());
		Assert.True(QuestCompletionMenu.instance.isActiveAndEnabled);
		Assert.False(npc.GetInventory().ContainsItem(item));
	}

	[UnityTest]
	public IEnumerator CompleteSellQuest() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		npc.GetInventory().RemoveAll();

		QuestUtils.CreateNewSellingQuest(npc);

		Quest quest = QuestManager.instance.outstandingQuests[0];
		for (int i = 0; i < SellingQuest.NUM_STAGES; i++) {
			QuestUtils.ActivateQuest(quest);
			Assert.AreEqual(1, QuestManager.instance.NumActive());
			yield return null;

			// Item item = QuestUtils.GetQuestItem(quest);
			// NPCInteractUtils.Sell(npc, item);
			quest.CompleteQuestStage();
			yield return null;

			Assert.AreEqual(i + 1, quest.GetCurrentStage());
		}

		Assert.AreEqual(0, QuestManager.instance.NumActive());
	}

	[UnityTest]
	public IEnumerator UnsuccessfulSellQuestWrongItem() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		npc.GetInventory().RemoveAll();

		QuestUtils.CreateNewSellingQuest(npc);
		SellingQuest sellingQuest = (SellingQuest) QuestManager.instance.outstandingQuests[0];
		QuestUtils.ActivateQuest(sellingQuest);

		Item questItem = QuestUtils.GetQuestItem(sellingQuest);
		Item otherItem = ItemManager.instance.GetRandomCommonItem(new Item[]{questItem});

		NPCInteractUtils.Sell(npc, otherItem);

		yield return null;
		Assert.True(npc.GetInventory().ContainsItem(otherItem));
		Assert.False(QuestCompletionMenu.instance.isActiveAndEnabled);

		Assert.AreEqual(1, QuestManager.instance.NumActive());
		Assert.AreEqual(1, QuestManager.instance.NumOutstanding());
	}

	[UnityTest]
	public IEnumerator UnsuccessfulSellQuestWrongNpc() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		npc.GetInventory().RemoveAll();

		NPC npc2 = NPCUtils.SpawnNPC();
		npc2.GetInventory().RemoveAll();

		QuestUtils.CreateNewSellingQuest(npc);
		SellingQuest sellingQuest = (SellingQuest) QuestManager.instance.outstandingQuests[0];
		QuestUtils.ActivateQuest(sellingQuest);

		Item item = QuestUtils.GetQuestItem(sellingQuest);

		NPCInteractUtils.Sell(npc2, item);

		yield return null;
		Assert.True(npc2.GetInventory().ContainsItem(item));
		Assert.False(QuestCompletionMenu.instance.isActiveAndEnabled);

		Assert.AreEqual(1, QuestManager.instance.NumActive());
	}

	[TearDown]
	public void UnloadMainScene() {
		GameManager.instance.save = false;
	}
}
