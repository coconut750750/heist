using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	public GameObject[] menuContents;
	public Button[] menuButtons;

	private int openedMenu;

	public int craftingIndex;
	public CraftingStash craftingStash;

	public int dismantleIndex;
	public DismantleStash dismantleStash;

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
		HidePauseStashes();
		gameObject.SetActive(false);
	}

	public void OpenContent(int index) {
		menuContents[openedMenu].SetActive(false);
		menuContents[index].SetActive(true);
		openedMenu = index;
		HidePauseStashes();
		if (index == craftingIndex) {
			DisplayCraftingStash();
		} else if (index == dismantleIndex) {
			DisplayDismantleStash();
		}
	}

	public void DisplayCraftingStash() {
		craftingStash.Display();
	}

	public void HidePauseStashes() {
		craftingStash.Hide();
		dismantleStash.Hide();
	}

	public void DisplayDismantleStash() {
		dismantleStash.Display();
	}
}
