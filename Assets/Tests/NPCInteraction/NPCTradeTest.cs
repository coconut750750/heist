using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class NPCTradeTest {

	[SetUp]
	public void LoadMainScene() {
		SceneManager.LoadScene("MainScene");
	}

	[UnityTest]
	public IEnumerator NPCBuyEmpty() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		npc.GetInventory().RemoveAll();

		Assert.False(NPCInteractUtils.Buy(npc, null, 0));
	}

	[UnityTest]
	public IEnumerator NPCBuyFirst() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		Item item = npc.GetInventory().GetItem(0);

		Assert.True(NPCInteractUtils.Buy(npc, item, 0));
	}

	[TearDown]
	public void UnloadMainScene() {
		GameManager.instance.save = false;
	}
}
