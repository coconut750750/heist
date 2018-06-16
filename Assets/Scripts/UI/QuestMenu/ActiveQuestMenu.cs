using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveQuestMenu : MonoBehaviour {

	public static ActiveQuestMenu instance = null;

	[SerializeField]
	private GameObject questDetailPrefab;
	[SerializeField]
	private GameObject scrollViewContent;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}

		gameObject.SetActive(false);
	}
	
	public void Display(Quest[] activeQuests) {
		Clear();
		gameObject.SetActive(true);
		foreach (Quest quest in activeQuests) {
			GameObject questDetailInstance = Instantiate(questDetailPrefab);
			questDetailInstance.transform.SetParent(scrollViewContent.transform);
			questDetailInstance.GetComponent<QuestDetail>().DisplayQuest(quest);
		}
	}

	private void Clear() {
		foreach (Transform child in scrollViewContent.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}
}
