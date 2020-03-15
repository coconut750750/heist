using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class NPCInteractTest {

	public Player GetPlayer() {
		return GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
	}

	public void MoveNPC(NPC npc, Vector3 pos) {
		npc.transform.position = pos;
	}

	public void MoveNPCToPlayer(NPC npc) {
		MoveNPC(npc, GetPlayer().transform.position);
	}

	public void MoveNPCAwayFromPlayer(NPC npc) {
		MoveNPC(npc, GetPlayer().transform.position + new Vector3(10, 0, 0));
	}

	[SetUp]
	public void LoadMainScene() {
		SceneManager.LoadScene("MainScene");
	}

	[UnityTest]
	public IEnumerator NPCSimpleContact() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractable interactable = npc.GetComponent<NPCInteractable>();

		MoveNPCToPlayer(npc);
		yield return null;
		Assert.True(interactable.HoverTextActive());

		MoveNPCAwayFromPlayer(npc);
		yield return null;
		Assert.False(interactable.HoverTextActive());
	}

	[UnityTest]
	public IEnumerator NPCSimpleInteract() {
		yield return null;

		NPC npc = NPCUtils.SpawnNPC();
		NPCInteractable interactable = npc.GetComponent<NPCInteractable>();
		MoveNPCToPlayer(npc);
		yield return null;

		ActionButton buttonA = InteractUtils.GetButtonA();
		buttonA.onClick.Invoke();
		yield return null;

		Assert.False(interactable.HoverTextActive());
		Assert.True(interactable.SpeechBubbleActive());
		Assert.True(interactable.NPCOptionsActive());
		Assert.AreEqual(npc.GetComponent<Rigidbody2D>().velocity, Vector2.zero);
	}

	[TearDown]
	public void UnloadMainScene() {
		GameManager.instance.save = false;
	}
}
