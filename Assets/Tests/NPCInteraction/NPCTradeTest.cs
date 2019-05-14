using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

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
		npc.GetInventory().RemoveAll();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Assert.False(NPCInteractUtils.Buy(npc, 0));
	}

	[UnityTest]
	public IEnumerator NPCBuySimple() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		Item randomItem = NPCUtils.AddRandomItem(npc, 0);
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Assert.True(NPCInteractUtils.Buy(npc, 0));
		Assert.True(GetPlayer().GetPocket().ContainsItem(randomItem));
	}

	[UnityTest]
	public IEnumerator NPCBuyCheckMoney() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		Item randomItem = NPCUtils.AddRandomItem(npc, 0);
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Player player = GetPlayer();
		int before = player.GetMoney();

		int cost = randomItem.cost();
		Assert.True(NPCInteractUtils.Buy(npc, 0));

		int after = player.GetMoney();
		Assert.AreEqual(before, after + cost);
	}

	[UnityTest]
	public IEnumerator NPCBuyNotEnoughMoney() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		Item randomItem = NPCUtils.AddRandomItem(npc, 0);
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Player player = GetPlayer();
		player.SetMoney(0);

		Assert.False(NPCInteractUtils.Buy(npc, 0));

		Assert.True(npc.GetInventory().ContainsItem(randomItem));
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
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Assert.False(NPCTrade.instance.sellController.Sell(npc));
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

	[UnityTest]
	public IEnumerator NPCTradeSimple() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		npc.GetInventory().RemoveAll();
		Item npcItem = NPCUtils.AddRandomItem(npc, 0);
		float npcValue = npcItem.GetValue();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Player player = GetPlayer();
		player.GetPocket().RemoveAll();

		Item playerItem = ItemManager.instance.GetRandomItem();
		playerItem.quality = (int)Math.Ceiling(npcValue * NPC.LOWER_BOUND_TRADING_PERC * 100 / playerItem.price);

		Assert.True(NPCInteractUtils.Trade(npc, playerItem, 0));
		Assert.True(player.GetPocket().ContainsItem(npcItem));
		Assert.True(npc.GetInventory().ContainsItem(playerItem));
	}

	[UnityTest]
	public IEnumerator NPCTradeUnsuccessful() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		npc.GetInventory().RemoveAll();
		Item npcItem = NPCUtils.AddRandomItem(npc, 0);
		float npcValue = npcItem.GetValue();
		NPCInteractUtils.ShowNPC(npc);
		yield return null;

		Player player = GetPlayer();
		player.GetPocket().RemoveAll();

		Item playerItem = ItemManager.instance.GetRandomItem();
		playerItem.quality = (int)Math.Ceiling(npcValue * 100 / playerItem.price);

		Assert.False(NPCInteractUtils.Trade(npc, playerItem, 0));
		Assert.True(npc.GetInventory().ContainsItem(npcItem));
	}

	[TearDown]
	public void UnloadMainScene() {
		GameManager.instance.save = false;
	}
}
