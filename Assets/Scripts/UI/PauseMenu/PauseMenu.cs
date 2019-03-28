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
	private int dismantleIndex;

	private Color defaultColor;
	private Color SELECTED = new Color(1, 0.3960784314f, 0);

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
		defaultColor = menuButtons[0].GetComponent<Image>().color;
	}

	public void Pause() {
		GameManager.instance.PauseGame();
		
		gameObject.SetActive(true);

		// open first menu content
		menuContents[0].SetActive(true);
		menuButtons[0].GetComponent<Image>().color = SELECTED;
		openedMenu = 0;

		// disable all other menu contents
		for (int i = 1; i < menuContents.Length; i++) {
			menuContents[i].SetActive(false);
			menuButtons[i].GetComponent<Image>().color = defaultColor;
		}
	}

	public void Unpause(){
		GameManager.instance.UnpauseGame();

		gameObject.SetActive(false);
	}

	#if UNITY_EDITOR || UNITY_STANDALONE
	protected void OnApplicationQuit() {
		gameObject.SetActive(false);
	}
	#elif UNITY_ANDROID || UNITY_IOS
	protected void OnApplicationPause() {
		gameObject.SetActive(false);
	}
	#endif

	public void OpenContent(int index) {
		// set button color
		menuButtons[openedMenu].GetComponent<Image>().color = defaultColor;
		menuButtons[index].GetComponent<Image>().color = SELECTED;

		// open menu content
		menuContents[openedMenu].SetActive(false);
		menuContents[index].SetActive(true);
		openedMenu = index;
	}
}
