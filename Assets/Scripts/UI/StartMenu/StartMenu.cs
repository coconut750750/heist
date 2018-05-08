using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void StartGame() {
		SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
	}
}
