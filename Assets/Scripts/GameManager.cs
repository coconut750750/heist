using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	
	[SerializeField]
	public Player mainPlayer;

	[SerializeField]
	public StashDisplayer stashDisplayer;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		stashDisplayer.transform.parent.gameObject.SetActive(false);
		stashDisplayer.gameObject.SetActive(false);

		DontDestroyOnLoad (gameObject);
		InitGame ();
	}

	void InitGame() {
		Physics2D.IgnoreLayerCollision (8, 9, true);
	}

	public void DisplayInventory(Inventory stash) {
		stashDisplayer.transform.parent.gameObject.SetActive(true);
		stashDisplayer.gameObject.SetActive(true);
		StashDisplayer.SetInventory(stash);
	}

	public void HideInventory() {
		stashDisplayer.gameObject.SetActive(false);
		stashDisplayer.transform.parent.gameObject.SetActive(false);
	}
}
