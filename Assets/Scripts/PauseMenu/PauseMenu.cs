using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {

	public static PauseMenu instance = null;

	[SerializeField]
	private GameObject[] menuContents;
	[SerializeField]
	private Button[] menuButtons;

	private int openedMenu;

	[SerializeField]
	private int craftingIndex;
	[SerializeField]
	private CraftingStash craftingStash;
	[SerializeField]
	private int dismantleIndex;
	[SerializeField]
	private DismantleStash dismantleStash;

	private Color WHITE = new Color(255, 255, 255);
	[SerializeField]
	private Color SELECTED = new Color(200, 200, 200);

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
		
		gameObject.SetActive(false);
		for (int i = 0; i < menuButtons.Length; i++) {
			int index = i;
			menuButtons[index].onClick.AddListener(delegate {
				OpenContent(index);
			});
		}
	}

	public void Pause() {
		GameManager.instance.PauseGame();
		
		// doesn't set active if only set once?!?
		gameObject.SetActive(true);
		gameObject.SetActive(true);

		// open first menu content
		menuContents[0].SetActive(true);
		menuButtons[0].GetComponent<Image>().color = SELECTED;
		openedMenu = 0;

		// disable all other menu contents
		for (int i = 1; i < menuContents.Length; i++) {
			menuContents[i].SetActive(false);
			menuButtons[i].GetComponent<Image>().color = WHITE;
		}
	}

	public void Unpause(){
		GameManager.instance.UnpauseGame();

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

	public ItemStash GetActiveStash() {
		if (openedMenu == craftingIndex) {
			return craftingStash;
		} else if (openedMenu == dismantleIndex) {
			return dismantleStash;
		} else {
			return null;
		}
	}
}
