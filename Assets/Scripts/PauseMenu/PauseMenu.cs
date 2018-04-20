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

	private Color WHITE = new Color(255, 255, 255);
	[SerializeField]
	private Color SELECTED = new Color(200, 200, 200);

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

		// open first menu content
		menuContents[0].SetActive(true);
		menuButtons[0].GetComponent<Image>().color = SELECTED;
		openedMenu = 0;

		// disable all other menu contents
		for (int i = 1; i < menuContents.Length; i++) {
			menuContents[i].SetActive(false);
		}
	}

	public void Unpause(){
		HidePauseStashes();
		gameObject.SetActive(false);
	}

	public void OpenContent(int index) {
		// set button color
		menuButtons[openedMenu].GetComponent<Image>().color = WHITE;
		menuButtons[index].GetComponent<Image>().color = SELECTED;

		// open menu content
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
