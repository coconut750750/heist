using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestAlertMenu : MonoBehaviour {

	public static QuestAlertMenu instance = null;

	[SerializeField]
	private Text nameText;
	[SerializeField]
	private Text questDetailsText;
	[SerializeField]
	private Text rewardText;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
		gameObject.SetActive(false);
	}

	public void Display(QuestStage completedQuestStage, NPC reporter) {
		gameObject.SetActive(true);

		nameText.text = reporter.GetName();
		questDetailsText.text = completedQuestStage.GetDetails();
		rewardText.text = completedQuestStage.GetReward().ToString();
	}

	public void Hide() {
		gameObject.SetActive(false);
	}
	
	public void OnAccept() {
		Hide();
	}
}
