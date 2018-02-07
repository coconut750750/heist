using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.IO;

public class BoardManager : MonoBehaviour {

	public GameObject[] shadows;

	public GameObject[] grassTiles;
	public GameObject[] roadTiles;
	public GameObject[] floorTiles;

	public GameObject[] sidewalkTiles;
	public GameObject[] treeTiles;
	public GameObject yellowLineHorizontal;
	public GameObject yellowLineVertical;
	public GameObject[] brickWalls;
	public GameObject[] doors;
	public String mapPath;
	public int mapNumber;

	private string[,] map;
	private int mapWidth;
	private int mapHeight;
	private Dictionary<Vector3, string> items;
	private int numItems;
	private Transform boardHolder;

	private const int MAP_ITEM_LEN = 3;

	private const string WALL_TYPE = "w";
	private const string DOOR_TYPE = "d";

	private const string PAVEMENT_ROAD = "p";
	private const string GRASS = "g";
	private const string FLOOR = "f";

	private const string TREE = "tr";
	private const string SIDEWALK = "sw";
	private const string YELLOW_STRIPE = "ys";
	private const string BRICK_WALL = "wb";
	private const string DOOR = "d";

	#if UNITY_ANDROID
	IEnumerator DownloadFiles (string mapUrl, string itemsUrl) {
		WWW mapWWW = new WWW(mapUrl);
		yield return mapWWW;

		WWW itemsWWW = new WWW(itemsUrl);
		yield return itemsWWW;

		String[] mapText = mapWWW.text.Split(new char[]{ '\n' });
		ParseMap(mapText);

		String[] itemsText = itemsWWW.text.Split(new char[]{ '\n' });
		ParseItems(itemsText);

		BoardSetup();
	}
	#endif

	void MapSetup() {
		char[] delimiterLines = { '\n' };

		string mapFile;
		string itemFile;
		String[] mapText;
		String[] itemsText;
		items = new Dictionary<Vector3, string>();
		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		mapFile = Application.dataPath + "/StreamingAssets/Maps/map" + mapNumber;
		itemFile = Application.dataPath + "/StreamingAssets/Maps/items" + mapNumber;

		#elif UNITY_IOS || UNITY_IPHONE

		mapFile = Application.dataPath + "/Raw/Maps/map" + mapNumber;
		itemFile = Application.dataPath + "/Raw/Maps/items" + mapNumber

		#elif UNITY_ANDROID

		mapFile = "jar:file://" + Application.dataPath + "!/assets/Maps/map" + mapNumber;
		itemFile = "jar:file://" + Application.dataPath + "!/assets/Maps/items" + mapNumber;
		StartCoroutine(DownloadFiles (mapFile, itemFile));

		#endif

		#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IOS || UNITY_IPHONE

		using (StreamReader mapReader = new StreamReader(mapFile)) {
		mapText = mapReader.ReadToEnd ().Split (delimiterLines);
		}

		using (StreamReader mapReader = new StreamReader(itemFile)) {
		itemsText = mapReader.ReadToEnd ().Split (delimiterLines);
		}

		ParseMap(mapText);
		ParseItems(itemsText);
		BoardSetup();

		#endif
	} 

	void ParseMap(String[] mapText) {
		char[] delimiterChars = {' '};
		String[] size = mapText[0].Split (delimiterChars);
		mapWidth = Int32.Parse(size[0]);
		mapHeight = Int32.Parse (size [1]);

		map = new string[mapHeight, mapWidth];
		for (int i = 0; i < mapHeight; i++) {
			String[] line = mapText[i + 1].Split (delimiterChars);
			for (int j = 0; j < mapWidth; j++) {
				map [mapHeight - i - 1, j] = line[j];
			}
		}
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

	bool[] getAdjacents(String gameObject, int r, int c) {
		bool left = map [r, c - 1].StartsWith (gameObject);
		bool up = map [r + 1, c].StartsWith (gameObject);
		bool right = map [r, c + 1].StartsWith (gameObject);
		bool down = map [r - 1, c].StartsWith (gameObject);
		return new bool[] { left, up, right, down };
	}

	void BoardSetup() {		
		for (int r = 0; r < mapHeight; r++) {
			for (int c = 0; c < mapWidth; c++) {
				String gameObject = map [r, c];
				String type = gameObject.Substring (0, gameObject.Length - 1);
				int num = Int32.Parse(gameObject.Substring (gameObject.Length - 1));

				Vector3 pos = new Vector3 (c, r, 0f);
				bool[] adjacents;

				if (type.Contains (PAVEMENT_ROAD)) {
					InstantiateGameObject(roadTiles [Random.Range (0, roadTiles.Length)], pos);

				} else if (type.Contains(GRASS)) {
					InstantiateGameObject(grassTiles [Random.Range (0, grassTiles.Length)], pos);

				} else if (type.Contains(FLOOR)) {
					InstantiateGameObject(floorTiles [Random.Range (0, floorTiles.Length)], pos);

				} else if (type.Equals(SIDEWALK)) {
					InstantiateGameObject (sidewalkTiles [num - 1], pos);

				} else if (type.Equals(YELLOW_STRIPE)) {
					adjacents = getAdjacents (YELLOW_STRIPE, r, c);
					InstantiateGameObject(roadTiles [Random.Range (0, roadTiles.Length)], pos);
					if (adjacents[0] || adjacents[2]) {
						InstantiateGameObject(yellowLineHorizontal, pos);
					} else if (adjacents[1] || adjacents[3]) {
						InstantiateGameObject(yellowLineVertical, pos);
					}

				} else if (type.Equals(TREE)) {
					InstantiateGameObject (treeTiles [num - 1], pos);
					InstantiateGameObject (grassTiles [Random.Range (0, grassTiles.Length)], pos);

				} else if (type.Equals(BRICK_WALL)) {
					adjacents = getAdjacents (BRICK_WALL, r, c);
					bool downRight = map [r - 1, c + 1].Substring (0, gameObject.Length - 1) != BRICK_WALL;
					GameObject wall = getBrickWall (adjacents);
					if (wall != null) {
						InstantiateGameObject (wall, pos);
						putShadow (adjacents, downRight, pos);
					}

				}
			}
		}

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

	private void putShadow(bool[] adj, bool dr, Vector3 pos) {
		bool l = adj [0]; bool u = adj [1]; bool r = adj [2]; bool d = adj [3];
		if (dr) {
			if (r && d) {
				InstantiateGameObject (shadows [3], new Vector3 (pos.x + 1, pos.y - 1, 0f));
			} else if (r) {
				InstantiateGameObject (shadows [1], new Vector3 (pos.x + 1, pos.y - 1, 0f));
			} else if (d) {
				InstantiateGameObject (shadows [2], new Vector3 (pos.x + 1, pos.y - 1, 0f));
			} else {
				InstantiateGameObject (shadows[0], new Vector3(pos.x + 1, pos.y - 1, 0f));
			}
		}
		if (!l && !d) {
			InstantiateGameObject (shadows [5], new Vector3 (pos.x, pos.y - 1, 0f));
		}
		if (!u && !r) {
			InstantiateGameObject (shadows [4], new Vector3 (pos.x + 1, pos.y, 0f));
		}
	}
	
	private GameObject getBrickWall(bool[] adj) {
		bool l = adj [0]; bool u = adj [1]; bool r = adj [2]; bool d = adj [3];
		if (!l && !u && !r && d) {
			return brickWalls [4];
		} else if (!l && !u && r && !d) {
			return brickWalls [1];
		} else if (!l && !u && r && d) {
			return brickWalls [8];
		} else if (!l && u && !r && !d) {
			return brickWalls [5];
		} else if (!l && u && !r && d) {
			return brickWalls [3];
		} else if (!l && u && r && !d) {
			return brickWalls [7];
		} else if (!l && u && r && d) {
			return brickWalls [11];
		} else if (l && !u && !r && !d) {
			return brickWalls [2];
		} else if (l && !u && !r && d) {
			return brickWalls [9];
		} else if (l && !u && r && !d) {
			return brickWalls [0];
		} else if (l && !u && r && d) {
			return brickWalls [12];
		} else if (l && u && !r && !d) {
			return brickWalls [6];
		} else if (l && u && !r && d) {
			return brickWalls [13];
		} else if (l && u && r && !d) {
			return brickWalls [10];
		} else if (l && u && r && d) {
			return brickWalls [14];
		} else {
			return null;
		}
	}

	public void SetupScene() {
		boardHolder = new GameObject ("Board").transform;
		MapSetup ();
	}
}
