using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class NPCTradeTest {

	public Player GetPlayer() {
		return GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

	public ItemSlot GetPocketItemSlot(int index) {
		return GameObject.Find("Pocket").GetComponentsInChildren<ItemSlot>()[index];
	}

	[SetUp]
	public void LoadMainScene() {
		SceneManager.LoadScene("MainScene");
	}

	[UnityTest]
	public IEnumerator NPCBuyEmpty() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		npc.GetInventory().RemoveAll();

		Assert.False(NPCInteractUtils.Buy(npc, 0));
	}

	[UnityTest]
	public IEnumerator NPCBuySimple() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Item randomItem = NPCUtils.AddRandomItem(npc, 0);

		Assert.True(NPCInteractUtils.Buy(npc, 0));
		Assert.True(GetPlayer().GetPocket().ContainsItem(randomItem));
	}

	[UnityTest]
	public IEnumerator NPCBuyCheckMoney() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Player player = GetPlayer();
		int before = player.GetMoney();

		Item randomItem = NPCUtils.AddRandomItem(npc, 0);
		int cost = randomItem.cost();
		Assert.True(NPCInteractUtils.Buy(npc, 0));

		int after = player.GetMoney();
		Assert.AreEqual(before, after + cost);
	}

	[UnityTest]
	public IEnumerator NPCBuyWithSellItem() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Item randomItem = ItemManager.instance.GetRandomItem();
		NPCInteractUtils.PutSellItem(npc, randomItem);

		Assert.False(NPCInteractUtils.BuyAvailable());
	}

	[UnityTest]
	public IEnumerator NPCBuyWithTradeItem() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Item randomItem = ItemManager.instance.GetRandomItem();
		NPCInteractUtils.PutTradeItem(npc, randomItem);

		Assert.False(NPCInteractUtils.BuyAvailable());
	}

	[UnityTest]
	public IEnumerator NPCBuyWithSellAndTradeItem() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Item randomItem = ItemManager.instance.GetRandomItem();
		NPCInteractUtils.PutTradeItem(npc, randomItem);
		NPCInteractUtils.PutSellItem(npc, randomItem);

		Assert.False(NPCInteractUtils.BuyAvailable());
	}

	[UnityTest]
	public IEnumerator NPCBuyWithFullPocket() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Player player = GetPlayer();
		Pocket pocket = player.GetPocket();
		pocket.RemoveAll();

		for (int i = 0; i < pocket.GetCapacity(); i++) {
			pocket.AddItem(ItemManager.instance.GetRandomItem());
		}

		Assert.False(NPCInteractUtils.BuyRandom(npc));
	}

	[UnityTest]
	public IEnumerator NPCSellEmpty() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCTrade npcTrade = NPCTrade.instance;
		npcTrade.Display(npc);
		Assert.False(npcTrade.sellController.Sell(npc));
	}

	[UnityTest]
	public IEnumerator NPCSellSimple() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Item randomItem = ItemManager.instance.GetRandomItem();
		
		Assert.True(NPCInteractUtils.Sell(npc, randomItem));
		Assert.True(npc.GetInventory().ContainsItem(randomItem));
	}

	[UnityTest]
	public IEnumerator NPCTradeLeaveWithExternalItems() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		GetPlayer().GetPocket().RemoveAll();
		Item randomItem = ItemManager.instance.GetRandomItem();

		NPCInteractUtils.PutTradeItem(npc, randomItem);
		NPCInteractUtils.PutSellItem(npc, randomItem);

		NPCTrade.instance.Hide();

		Assert.AreEqual(randomItem, GetPlayer().GetPocket().GetItem(0));
		Assert.AreEqual(randomItem, GetPlayer().GetPocket().GetItem(1));
		Assert.AreEqual(GetPlayer().GetPocket().GetNumItems(), 2);
	}

	[TearDown]
	public void UnloadMainScene() {
		GameManager.instance.save = false;
	}
}
