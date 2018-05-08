using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingContent : MonoBehaviour {
	
	public GameObject quitSafetyMenu;
	public GameObject loadSafetyMenu;

	void Awake() {
		quitSafetyMenu.SetActive(false);
		loadSafetyMenu.SetActive(false);
	}

	public void OpenQuitSafety() {
		quitSafetyMenu.SetActive(true);
	}

	public void CloseQuitSafety() {
		quitSafetyMenu.SetActive(false);
	}

	public void OpenLoadSafety() {
		loadSafetyMenu.SetActive(true);
	}

	public void CloseLoadSafety() {
		loadSafetyMenu.SetActive(false);
	}
}
