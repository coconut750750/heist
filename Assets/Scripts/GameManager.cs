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
		}

		DontDestroyOnLoad (gameObject);
		InitGame ();
	}

	void InitGame() {
		Physics2D.IgnoreLayerCollision (8, 9, true);
	}
}
