using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerTest {
	[SetUp]
	public void LoadMainScene() {
		SceneManager.LoadScene("MainScene");		
	}

	public Player GetPlayer() {
		return GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

	[UnityTest]
	public IEnumerator SimpleLoadScene() {
		yield return null;

		Player player = GetPlayer();
		
		Assert.AreNotEqual(player, null);
	}

	// Movement

	// Interactions

	[UnityTest]
	public IEnumerator PlayerAttackDelay() {
		yield return null;

		Player player = GetPlayer();

		Assert.True(player.Attack());
		Assert.False(player.Attack());

		yield return new WaitForSeconds(player.GetAttackDelay());
		Assert.True(player.Attack());
	}

	// Inventory

	[UnityTest]
	public IEnumerator PlayerAddItem() {
		yield return null;

		Player player = GetPlayer();

		Item randomItem = ItemManager.instance.GetRandomItem();

		Assert.True(player.CanAddItem());

		player.AddItem(randomItem);

		yield return null;

		Assert.AreEqual(player.NumItems(), 1);

		Pocket pckt = player.GetPocket();
		Assert.AreEqual(randomItem, pckt.GetItem(0));
		Assert.AreNotEqual(pckt.itemSlots[0].GetItemImage(), null);

		yield return null;

		player.RemoveItemAtIndex(0);
	}

	[UnityTest]
	public IEnumerator PlayerFullInventory() {
		yield return null;

		Player player = GetPlayer();

		Item randomItem = ItemManager.instance.GetRandomItem();

		Assert.True(player.CanAddItem());

		for (int i = 0; i < Pocket.NUM_ITEMS; i++) {
			player.AddItem(randomItem);
		}

		Assert.False(player.CanAddItem());

		for (int i = 0; i < Pocket.NUM_ITEMS; i++) {
			player.RemoveItemAtIndex(i);
		}
	}

	[TearDown]
	public void UnloadMainScene() {
		GameManager.instance.save = false;
		SceneManager.UnloadSceneAsync("MainScene");
	}
}
