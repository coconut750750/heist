using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour {

	public static NPCQuest instance = null;

	[SerializeField]
	private Text questText;

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
		GameManager.instance.PauseGame();

		gameObject.SetActive(true);

		this.npc = npc;

		questText.text = npc.GetQuest();
	}

	public void Hide() {
		gameObject.SetActive(false);
		
		GameManager.instance.UnpauseGame();
	}
}
