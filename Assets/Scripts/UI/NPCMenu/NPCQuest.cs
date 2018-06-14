using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCQuest : MonoBehaviour {

	public static NPCQuest instance = null;

	[SerializeField]
	private Text questText;
	[SerializeField]
	private Text rewardText;

	private NPC npc;
	private Quest quest;

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
		this.quest = npc.GetQuest();
		if (this.quest != null) {
			questText.text = this.quest.GetCurrentDetails();
			rewardText.text = this.quest.GetCurrentReward().ToString();
		} else {
			questText.text = "No quest at this time right now.";
			foreach (Button button in GetComponentsInChildren<Button>()) {
				button.interactable = false;
			}
		}
	}

	public void Hide() {
		gameObject.SetActive(false);
		
		GameManager.instance.UnpauseGame();
	}

	public void AcceptedQuest() {
		Debug.Log("accepted!");
	}

	public void RejectedQuest() {
		Debug.Log("rejected :(");
	}
}
