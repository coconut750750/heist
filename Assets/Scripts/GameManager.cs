using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	public static Player mainPlayer = null;
	public static StashDisplayer stashDisplayer = null;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		if (mainPlayer == null) {
			mainPlayer = GameObject.Find("Player").gameObject.GetComponent<Player>();
		}

		if (stashDisplayer == null) {
			stashDisplayer = GameObject.FindObjectOfType<StashDisplayer>();
			stashDisplayer.transform.parent.gameObject.SetActive(false);
			stashDisplayer.gameObject.SetActive(false);
		}

		DontDestroyOnLoad (gameObject);
		InitGame ();
	}

	void InitGame() {
		Physics2D.IgnoreLayerCollision (8, 9, true);
	}

	public static void DisplayStash() {
		stashDisplayer.transform.parent.gameObject.SetActive(true);
		stashDisplayer.gameObject.SetActive(true);
	}

	public static void HideStash() {
		stashDisplayer.gameObject.SetActive(false);
		stashDisplayer.transform.parent.gameObject.SetActive(false);
	}
}
