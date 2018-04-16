using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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

		stashDisplayer.gameObject.SetActive(false);

		DontDestroyOnLoad (gameObject);
		InitGame ();
	}

	void InitGame() {
		Debug.Log("Game Started");
		Physics2D.IgnoreLayerCollision (8, 9, true);
	}

	public void DisplayInventory(Inventory stash) {
		stashDisplayer.gameObject.SetActive(true);
		StashDisplayer.SetInventory(stash);
	}

	public void HideInventory() {
		stashDisplayer.gameObject.SetActive(false);
		StashDisplayer.ClearInventory();
	}

	public static void Save(GameData data, string filename) {
		if (data != null && filename != null) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Create(filename);

			bf.Serialize(file, data);
			file.Close();
		}
	}

	public static T Load<T>(string filename) where T : GameData {
		if (File.Exists(filename)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(filename, FileMode.Open);

            T data = bf.Deserialize(file) as T;
            file.Close();

			return data;
        }

		return null;
	}
}

[System.Serializable]
public class GameData {

}