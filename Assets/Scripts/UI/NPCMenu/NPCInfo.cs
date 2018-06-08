using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInfo : MonoBehaviour {

	public static NPCInfo instance = null;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text healthText;
	[SerializeField]
	private Text expText;
	[SerializeField]
	private Text strengthText;
	[SerializeField]
	private Text moneyText;

	private NPC npc;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		gameObject.SetActive(false);
	}
	
	public void Display(NPC npc) {
		// dont need to pause game because initial interaction (npcinteractable)
		// will pause the game

		gameObject.SetActive(true);

		this.npc = npc;

		nameText.text = npc.GetName();

		healthText.text = npc.GetHealth().ToString();
		strengthText.text = npc.GetStrength().ToString();
		expText.text = npc.GetExperience().ToString();
	}

	public void Hide() {
		gameObject.SetActive(false);
		// dont need to unpause the game because, again, npcinteractable will unpause
	}
}
