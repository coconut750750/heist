using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	
	[SerializeField]
	public Player mainPlayer;

	[SerializeField]
	public Canvas canvas;

	[SerializeField]
	private Map map;

	// in game clock
	private int day = 1;
	private int hour;
	private int minute;
	private int second;
	private float lastChange = 0f;
	private const float CHANGE_RATE_SECS = 0.5f; // in seconds
	public Text timeText;

	private bool isPaused;

	private string filename;
	
	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		print("Game Started");
		timeText.text = GetTimeString();	
	}

	void Start() {
		filename = Application.persistentDataPath + "/" + "gamestate.dat";
		LoadAll();
	}

	#if UNITY_EDITOR || UNITY_STANDALONE
	protected void OnApplicationQuit() {
		SaveAll();
	}
	#elif UNITY_ANDROID || UNITY_IOS
	protected void OnApplicationPause() {
		SaveAll();
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
			timeText.text = GetTimeString();
			lastChange = Time.time;
    	}
	}

	public string GetTimeString() {
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

	public int GetHour() {
		return hour;
	}

	public int GetMinute() {
		return minute;
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

	public void ShowFloor2() {
		map.SetGroundFloorActive(false);
		map.SetSecondFloorActive(true);
	}

	public void HideFloor2() {
		map.SetGroundFloorActive(true);
		map.SetSecondFloorActive(false);
	}

	public int GetVisibleFloor() {
		return mainPlayer.GetFloor();
	}

	public Rect GetCameraRange(int range) {
		Rect rect = new Rect();
		rect.position = mainPlayer.transform.position - new Vector3((float)range / 2f, (float)range / 2f);
		rect.width = range;
		rect.height = range;

		return rect;
	}

	public bool CanSeePlayer(Vector3 pos, float squaredVisionDist) {
		if (pos.z != mainPlayer.transform.position.z) {
			return false;
		}

		Vector2 displacement = mainPlayer.transform.position - pos;
		return displacement.sqrMagnitude <= squaredVisionDist;
	}

	public void QuitToStartMenu() {
		SceneManager.LoadScene("StartScene", LoadSceneMode.Single);
	}

	public void Save() {
		ManagerData data = new ManagerData(day, hour, minute, second);
        GameManager.Save(data, filename);
	}

	public void SaveAll() {
		// item stashes
		ItemStash[] itemStashes = FindObjectsOfType<ItemStash>();
		foreach (ItemStash itemStash in itemStashes) {
			itemStash.Save();
		}

		// NPC spawners
		NPCSpawner.instance.Save();

		// Police spawner


		// moving objects including player
		Character[] characterObjs = FindObjectsOfType<Character>();
		foreach (Character c in characterObjs) {
			c.Save();
		}

		// quest manager
		QuestManager.instance.Save();

		// manager itself
		Save();
	}

	public void Load() {
		ManagerData data = GameManager.Load<ManagerData>(filename);
		if (data != null) {
			day = data.day;
			hour = data.hour;
			minute = data.minute;
			second = data.second;
		}
	}

	public void LoadAll() {
		// item stashes
		ItemStash[] itemStashes = FindObjectsOfType<ItemStash>();
		foreach (ItemStash itemStash in itemStashes) {
			itemStash.Load();
		}

		// NPC spawner
		NPCSpawner.instance.Load();

		// Police spawner

		// quest manager
		QuestManager.instance.Load();

		// moving objects including player
		Character[] characterObjs = FindObjectsOfType<Character>();
		foreach (Character c in characterObjs) {
			c.Load();
		}

		// manager itself
		Load();
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