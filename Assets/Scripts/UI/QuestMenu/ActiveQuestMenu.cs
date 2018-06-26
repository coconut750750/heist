using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveQuestMenu : MonoBehaviour {

	public static ActiveQuestMenu instance = null;

	[SerializeField]
	private GameObject questDetailPrefab;
	[SerializeField]
	private GameObject scrollViewContent;

	private List<QuestDetail> questDetailInstances = new List<QuestDetail>();

	void Awake () {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}

		gameObject.SetActive(false);
	}

	public void AddActiveQuest(Quest newQuest) {
		GameObject questDetailInstance = Instantiate(questDetailPrefab);
		questDetailInstance.transform.SetParent(scrollViewContent.transform);
		QuestDetail instance = questDetailInstance.GetComponent<QuestDetail>();
		instance.DisplayQuest(newQuest);
		questDetailInstances.Add(instance);
	}

	public void RemoveActiveQuest(Quest oldQuest) {
		foreach (QuestDetail instance in questDetailInstances) {
			if (instance.GetDisplayingQuest() != null && instance.GetDisplayingQuest() == oldQuest) {
				Destroy(instance.gameObject);
				questDetailInstances.Remove(instance);
				return;
			}
		}
	}
}
