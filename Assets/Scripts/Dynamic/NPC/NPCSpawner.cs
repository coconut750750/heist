using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>  
///		This is the NPCSpawner class.
/// 	This class contains a list of npcs including their metadata.
/// 	SPAWNING:
///			At first, the list of npcs is empty. As more and more npcs are spawned, new NPC instances are
/// 	created. When the max number of npcs are created (PEAK MAX), further npcs are spawned by awakening
/// 	npcs that are disable from a previous recall.
///
///		RECALLING:
///			Recalling an NPC is merely just disabling the NPC. The spawner should not keep spawning random
///		NPCs. NPCs that existed in the past should be able to be spawned again. When it is time to recall
///		an NPC, a random npcs is chosen. If it is awake and not in the range of the camera, it is recalled.
///
///		SAVING and LOADING:
/// 		saving: this class saves everything, and the NPCs spawned do not save themselves
///			loading: subclasses load, but this class contains a helper function to load base class members
///				When this class is loaded, every NPC is instantiated. Then, if an NPC is disabled, it is
///					recalled immediately.
/// </summary>  
public class NPCSpawner : MonoBehaviour {

	private const string NPC_NAME = "NPC-";

	public int PEAK_MAX = 1;
	public int PEAK_MIN = 1;

	public int BASE_MAX = 0;
	public int BASE_MIN = 0;

	public const int START_PEAK_HOUR = 9;
	public const int END_PEAK_HOUR = 17;

	public const int START_BASE_HOUR = 18;
	public const int END_BASE_HOUR = 8;

	public Nav2D polyNav;

	private List<NPC> npcs;
	private List<int> npcIndicies;
	private List<bool> npcAwake;

	private int numAwake; // number of npcs awake

	public int npcSize;
	public int spawnRange;

	public float alterDelay;
	private bool canAlterNpcCount;

	private string filename;

	IEnumerator AlterDelay() {
		canAlterNpcCount = false;
		yield return new WaitForSeconds(alterDelay);
		canAlterNpcCount = true;
	}

	void Start () {
		npcs = new List<NPC>();
		npcIndicies = new List<int>();
		npcAwake = new List<bool>();
		numAwake = 0;
		
		StartCoroutine(AlterDelay());

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
			if (numAwake <= PEAK_MIN) {
				Spawn();
			} else if (numAwake >= PEAK_MAX) {
				Recall();
			}
		} else if (hour < END_BASE_HOUR || END_PEAK_HOUR <= hour) {
			if (numAwake <= BASE_MIN) {
				Spawn();
			} else if (numAwake >= BASE_MAX) {
				Recall();
			}
		} 
	}

	void ActivateNPC(int npcIndex) {
		NPC npc = npcs[npcIndex];

		if (NpcIsInRange(npc)) {
			return;
		}

		npc.Spawn();
		npcAwake[npcIndex] = true;
		numAwake++;
	}

	NPC InstantiateNPC(int index, Vector2 pos) {	
		NPC instance = NPCManager.instance.InstantiateNPC(index, pos);	

		instance.SetAgentNav(polyNav);
		instance.transform.SetParent(transform);

		instance.name = NPC_NAME + npcs.Count;
		instance.SetIndependent(false);

		instance.OnDeath += Remove;

		npcs.Add(instance);
		npcIndicies.Add(index);
		npcAwake.Add(true);
		numAwake++;

		return instance;
	}

	void Spawn() {
		if (!canAlterNpcCount) {
			return;
		}

		Vector2? pos = GenerateRandomNPCPos();
		if (pos == null) {
			return;
		}
		
		if (npcs.Count <= PEAK_MAX) {
			InstantiateNPC(Random.Range(0, NPCManager.instance.npcTypes), (Vector2)pos);
			StartCoroutine(AlterDelay());
		} else {
			int npcIndex = Random.Range(0, npcAwake.Count);
			if (!npcAwake[npcIndex]) {
				ActivateNPC(npcIndex);
				StartCoroutine(AlterDelay());
			}
		}
	}

	// deactivates but doesn't delete
	void Recall() {
		if (!canAlterNpcCount) {
			return;
		}

		int npcIndex = Random.Range(0, npcAwake.Count);
		if (CanRecallNPC(npcIndex)) {
			Recall(npcIndex);
			StartCoroutine(AlterDelay());
		}
	}

	// recalls npc no matter where it is
	void Recall(int npcIndex) {
		npcs[npcIndex].gameObject.SetActive(false);
		npcAwake[npcIndex] = false;
		numAwake--;
	}

	// removes npc from npc list (maybe because they cease to exist)
	void Remove(NPC npc) {
		Debug.Log("removing...");
		int index = npcs.IndexOf(npc);
		npcs.RemoveAt(index);
		npcAwake.RemoveAt(index);
		npcIndicies.RemoveAt(index);
		numAwake--;
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

	bool NpcIsInRange(NPC npc) {
		Rect range = GameManager.instance.GetCurrentPlayerRange(spawnRange + 2 * npcSize);

		return range.Contains((Vector2)(npc.transform.position));
	}

	bool CanRecallNPC(int npcIndex) {
		return !NpcIsInRange(npcs[npcIndex]) && 
			   npcAwake[npcIndex] &&
			   !npcs[npcIndex].IsFighting();
	}

	public int NumNpcs() {
		return npcs.Count;
	}

	public int[] GetNpcIndicies() {
		return npcIndicies.ToArray();
	}

	public bool[] GetNpcAwake() {
		return npcAwake.ToArray();
	}

	public int GetNumAwake() {
		return numAwake;
	}

	public NPC[] GetNpcs() {
		return npcs.ToArray();
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
				NPC instance = InstantiateNPC(data.npcIndicies[i], Vector2.zero);
				instance.LoadFromData(data.npcDatas[i]);

				if (!data.npcAwake[i]) {
					Recall(i);
				}
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
	public bool[] npcAwake;

	public int numAwake;

	public NPCData[] npcDatas;

	public NPCSpawnerData(NPCSpawner spawner) {
		numNpcs = spawner.NumNpcs();
		npcIndicies = spawner.GetNpcIndicies();
		npcAwake = spawner.GetNpcAwake();

		numAwake = spawner.GetNumAwake();

		npcDatas = new NPCData[numNpcs];
		NPC[] npcs = spawner.GetNpcs();
		for (int i = 0; i < numNpcs; i++) {
			npcDatas[i] = new NPCData(npcs[i]);
		}
	}
}