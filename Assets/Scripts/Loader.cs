using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Loader : MonoBehaviour {

	public GameObject gameManager;

	void Awake () {
		if (GameManager.instance == null) {
			Instantiate (gameManager);
		}

	}
}
