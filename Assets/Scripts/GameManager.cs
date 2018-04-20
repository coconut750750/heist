using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	
	[SerializeField]
	public Player mainPlayer;

	[SerializeField]
	public StashDisplayer stashDisplayer;

	// in game clock
	private int day = 1;
	private int hour;
	private int minute;
	private int second;
	private float lastChange = 0f;
	private const float CHANGE_RATE_SECS = 0.5f; // in seconds
	public Text timeText;

	private bool isPaused;

	public string filename;
	
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		stashDisplayer.gameObject.SetActive(false);

		DontDestroyOnLoad (gameObject);
		InitGame ();

		filename = Application.persistentDataPath + "/" + "gamestate.dat";
		ManagerData data = GameManager.Load<ManagerData>(filename);
		if (data != null) {
			day = data.day;
			hour = data.hour;
			minute = data.minute;
			second = data.second;
		}
		timeText.text = "" + GetTime();	
	}

	void InitGame() {
		Debug.Log("Game Started");
		Physics2D.IgnoreLayerCollision (8, 9, true);
	}

	#if UNITY_EDITOR || UNITY_STANDALONE
	protected void OnApplicationQuit() {
		ManagerData data = new ManagerData(day, hour, minute, second);
        GameManager.Save(data, filename);
	}
	#elif UNITY_ANDROID || UNITY_IOS
	protected void OnApplicationPause() {
		ManagerData data = new ManagerData(day, hour, minute, second);
        GameManager.Save(data, filename);
	}
	#endif

	void FixedUpdate() {
		if (Time.time - lastChange > CHANGE_RATE_SECS && !isPaused) { 
			minute++;
			if (minute == 60) {
				minute = 0;
				hour++;
				if (hour == 24) {
					day++;
					hour = 0;
					// new day here!
				}
			}
			timeText.text = "" + GetTime();
			lastChange = Time.time;
    	}
	}

	public string GetTime() {
		string hourStr;
		string minuteStr = "" + minute;
		string ampm;
		if (minuteStr.Length == 1) {
			minuteStr = "0" + minuteStr;
		}
		if (hour >= 12) {
			hourStr = "" + hour % 12;
			ampm = "PM";
		} else {
			hourStr = "" + hour;
			ampm = "AM";
		}
		if (hour % 12 == 0) {
			hourStr = "12";
		}
		return "Day " + day + ". " + hourStr + ":" + minuteStr + " " + ampm;
	}

	public void DisplayInventory(Inventory stash) {
		stashDisplayer.gameObject.SetActive(true);
		StashDisplayer.SetInventory(stash);
	}

	public void HideInventory() {
		stashDisplayer.gameObject.SetActive(false);
		StashDisplayer.ClearInventory();
	}

	public void PauseGame() {
		isPaused = true;
	}

	public void UnpauseGame() {
		isPaused = false;
	}

	public bool IsPaused() {
		return isPaused;
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

[System.Serializable]
public class ManagerData : GameData {
	public int day;
	public int hour;
	public int minute;
	public int second;

	public ManagerData(int day, int hour, int minute, int second) {
		this.day = day;
		this.hour = hour;
		this.minute = minute;
		this.second = second;
	}
}