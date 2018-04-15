using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	public GameObject[] menuContents;
	public Button[] menuButtons;

	private int openedMenu;

	void Awake () {
		gameObject.SetActive(false);
		for (int i = 0; i < menuButtons.Length; i++) {
			int index = i;
			menuButtons[index].onClick.AddListener(delegate {
				OpenContent(index);
			});
		}
	}

	public void Pause() {
		gameObject.SetActive(true);
		menuContents[0].SetActive(true);
		for (int i = 1; i < menuContents.Length; i++) {
			menuContents[i].SetActive(false);
		}
		openedMenu = 0;
	}

	public void UnPause(){
		gameObject.SetActive(false);
	}

	public void OpenContent(int index) {
		Debug.Log("length: " + menuContents.Length);
		Debug.Log("index: " + index);
		menuContents[openedMenu].SetActive(false);
		menuContents[index].SetActive(true);
		openedMenu = index;
	}
}
