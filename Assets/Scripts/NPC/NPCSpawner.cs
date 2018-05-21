using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour {

	private const string NPC_NAME = "NPC-";

	public const int PEAK_MAX = 25;
	public const int PEAK_MIN = 20;

	public const int BASE_MAX = 10;
	public const int BASE_MIN = 5;

	public const int START_PEAK_HOUR = 9;
	public const int END_PEAK_HOUR = 17;

	public const int START_BASE_HOUR = 18;
	public const int END_BASE_HOUR = 8;

	public Nav2D polyNav;

	public NPC[] npcsToSpawn;

	private List<NPC> npcs;
	private List<int> npcIndicies;

	public int npcSize;
	public int spawnRange;

	public float spawnDelay;
	private bool canSpawn;

	private string filename;

	IEnumerator SpawnDelay() {
		canSpawn = false;
		yield return new WaitForSeconds(spawnDelay);
		canSpawn = true;
	}

	void Start () {
		npcs = new List<NPC>();
		npcIndicies = new List<int>();
		StartCoroutine(SpawnDelay());

		filename = Application.persistentDataPath + "/" + gameObject.name + ".dat";
		Load();
	}

	#if UNITY_EDITOR || UNITY_STANDALONE
	protected void OnApplicationQuit() {
		Save();
	}
	#elif UNITY_ANDROID || UNITY_IOS
	protected void OnApplicationPause() {
		Save();
	}
	#endif
	
	void Update () {
		if (GameManager.instance.IsPaused()) {
			return;
		}

		int hour = GameManager.instance.GetHour();

		if (END_BASE_HOUR <= hour && hour < END_PEAK_HOUR) {
			if (npcs.Count < PEAK_MIN) {
				Spawn();
			} else if (npcs.Count > PEAK_MAX) {
				Recall();
			}
		} else if (hour < END_BASE_HOUR || END_PEAK_HOUR <= hour) {
			if (npcs.Count < BASE_MIN) {
				Spawn();
			} else if (npcs.Count > BASE_MAX) {
				Recall();
			}
		} 
	}

	NPC InstantiateNPC(int index, Vector2 pos) {
		NPC toSpawn = npcsToSpawn[index];
		
		NPC instance = Instantiate (toSpawn, (Vector2)pos, Quaternion.identity) as NPC;
		instance.SetAgentNav(polyNav);
		instance.transform.SetParent(transform);
		instance.name = NPC_NAME + npcs.Count;

		npcs.Add(instance);
		npcIndicies.Add(index);

		return instance;
	}

	void Spawn() {
		if (!canSpawn) {
			return;
		}

		Vector2? pos = GenerateRandomNPCPos();
		if (pos == null) {
			return;
		}
		
		InstantiateNPC(Random.Range(0, npcsToSpawn.Length), (Vector2)pos);

		StartCoroutine(SpawnDelay());
	}

	void Recall() {
		for (int i = 0; i < npcs.Count; i++) {
			if (!NpcIsInRange(npcs[i])) {
				npcs.RemoveAt(i);
				Destroy(npcs[i]);
				npcIndicies.RemoveAt(i);
				return;
			}
		}
	}

	// returns a vector 2 position out of range 
	// to be relevant
	Vector2? GenerateRandomNPCPos() {
		Rect range = GameManager.instance.GetCurrentPlayerRange(spawnRange + 2 * npcSize);
		int side = Random.Range(0, 4);
		Vector2 pos;

		switch (side) {
			case 0: // left
				float y = Mathf.Round(Random.Range(range.min.y, range.max.y));
				pos = new Vector2(range.min.x - 1, y);
				break;
			case 1: // top
				float x = Mathf.Round(Random.Range(range.min.x, range.max.x));
				pos = new Vector2(x, range.max.y + 1);
				break;
			case 2: // right
				y = Mathf.Round(Random.Range(range.min.y, range.max.y));
				pos = new Vector2(range.max.x + 1, y);
				break;
			default: // down
				x = Mathf.Round(Random.Range(range.min.x, range.max.x));
				pos = new Vector2(x, range.min.y - 1);
				break;
		}

		if (polyNav.PointIsValid(pos)) {
			return pos;
		} else {
			return null;
		}
	}

	// checks to see if NPC is in the range of the camera
	bool NpcIsInRange(NPC npc) {
		Rect range = GameManager.instance.GetCurrentPlayerRange(spawnRange + 2 * npcSize);

		return range.Contains((Vector2)(npc.transform.position));
	}

	public int NumNpcs() {
		return npcs.Count;
	}

	public int[] GetNpcIndicies() {
		return npcIndicies.ToArray();
	}

	public void Save()
    {
        NPCSpawnerData data = new NPCSpawnerData(this);
		GameManager.Save(data, filename);
    }

    public void Load()
    {
        NPCSpawnerData data = GameManager.Load<NPCSpawnerData>(filename);
		
        if (data != null) {
			int count = data.numNpcs;

			for (int i = 0; i < count; i++) {
				NPC toSpawn = npcsToSpawn[data.npcIndicies[i]];
				toSpawn.LoadFromFile(NPC_NAME + i);
		
				NPC instance = Instantiate (toSpawn, (Vector2)toSpawn.transform.position, Quaternion.identity) as NPC;
				instance.SetAgentNav(polyNav);
				instance.transform.SetParent(transform);
				instance.name = NPC_NAME + npcs.Count;

				npcs.Add(instance);
				npcIndicies.Add(data.npcIndicies[i]);
			}
		} else {
			//Destroy(this);
		}
    }
}

[System.Serializable]
public class NPCSpawnerData : GameData {
	public int numNpcs;
	public int[] npcIndicies;

	public NPCSpawnerData(NPCSpawner spawner) {
		numNpcs = spawner.NumNpcs();
		npcIndicies = spawner.GetNpcIndicies();
	}
}