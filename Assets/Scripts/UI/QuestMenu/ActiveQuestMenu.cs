using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveQuestMenu : MonoBehaviour {

	public static ActiveQuestMenu instance = null;

	[SerializeField]
	private GameObject questDetail;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
	}
	
	public void Display(Quest[] activeQuests) {

	}
}
