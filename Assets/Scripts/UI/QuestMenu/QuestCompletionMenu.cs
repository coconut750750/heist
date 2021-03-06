﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestCompletionMenu : MonoBehaviour {

	public static QuestCompletionMenu instance = null;

	[SerializeField]
	private QuestDetail questDetail;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
		gameObject.SetActive(false);
	}

	public void Display(Quest completedQuest) {
		gameObject.SetActive(true);
		questDetail.DisplayQuest(completedQuest);
	}
}
