using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.IO;

public class BoardManager : MonoBehaviour {

	public GameObject[] doors;

	public String mapPath;
	public int mapNumber;

	private Dictionary<Vector3, string> items;
	private int numItems;
	private Transform boardHolder;

	private const string DOOR = "d";

	#if UNITY_ANDROID
	IEnumerator DownloadFiles (string itemsUrl) {
		WWW itemsWWW = new WWW(itemsUrl);
		yield return itemsWWW;

		String[] itemsText = itemsWWW.text.Split(new char[]{ '\n' });
		ParseItems(itemsText);

		BoardSetup();
	}
	#endif

	void MapSetup() {
		char[] delimiterLines = { '\n' };

		string itemFile;
		String[] itemsText;
		items = new Dictionary<Vector3, string>();
		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		itemFile = Application.dataPath + "/StreamingAssets/Maps/items" + mapNumber;

		#elif UNITY_IOS || UNITY_IPHONE

		itemFile = Application.dataPath + "/Raw/Maps/items" + mapNumber

		#elif UNITY_ANDROID

		itemFile = "jar:file://" + Application.dataPath + "!/assets/Maps/items" + mapNumber;
		StartCoroutine(DownloadFiles (itemFile));

		#endif

		#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IOS || UNITY_IPHONE

		using (StreamReader mapReader = new StreamReader(itemFile)) {
		itemsText = mapReader.ReadToEnd ().Split (delimiterLines);
		}

		ParseItems(itemsText);
		BoardSetup();

		#endif
	} 

	void ParseItems(String[] itemsText) {
		char[] delimiterChars = {' '};

		numItems = Int32.Parse (itemsText [0]);
		for (int i = 0; i < numItems; i++) {
			String[] line = itemsText[i + 1].Split (delimiterChars);
			Vector3 itemPos = new Vector3 (Int32.Parse (line [1]), Int32.Parse (line [2]), 0f);
			items.Add (itemPos, line [0]);
		}
	}

	void InstantiateGameObject(GameObject obj, Vector3 pos) {
		GameObject instance = Instantiate (obj, pos, Quaternion.identity);
		instance.transform.SetParent (boardHolder);
	}

	void BoardSetup() {		
		foreach (KeyValuePair<Vector3, string> entry in items) {
			string gameObject = entry.Value;
			Vector3 pos = entry.Key;
			string type = gameObject.Substring (0, gameObject.Length - 1);
			int num = Int32.Parse(gameObject.Substring (gameObject.Length - 1));

			if (type == DOOR) {
				InstantiateGameObject (doors[num - 1], pos);

			}
		}
	}

	public void SetupScene() {
		boardHolder = new GameObject ("Board").transform;
		MapSetup ();
	}
}
