using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
///		an NPC, a random npc is chosen. If it is awake and not in the range of the camera, it is recalled.
///
///		SAVING and LOADING:
/// 		saving: this class saves everything, and the NPCs spawned do not save themselves
///			loading: subclasses load, but this class contains a helper function to load base class members
///				When this class is loaded, every NPC is instantiated. Then, if an NPC is disabled, it is
///					recalled immediately.
/// </summary>  
public class NPCSpawner : MonoBehaviour {

	public static NPCSpawner instance = null;

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
	private List<int> npcAnimIndexes;
	private List<bool> npcAwake;

	private int numAwakeNpcs;

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

	IEnumerator WaitForRecall(NPC npc) {
		while (NPCIsInRange(npc)) {
			yield return null;
		}

		RecallUnconditionally(npc);
	}

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);
		}

		npcs = new List<NPC>();
		npcAnimIndexes = new List<int>();
		npcAwake = new List<bool>();
		numAwakeNpcs = 0;
	}

	void Start () {
		StartCoroutine(AlterDelay());
        InitNPCs();
	}

    void InitNPCs() {
        for (int i = npcs.Count; i < PEAK_MAX; i++) {
		    Vector2 pos = polyNav.GetRandomValidPoint();
            SpawnUnconditionally(pos);
            RecallUnconditionally(i);
        }
    }
	
	void Update () {
		if (GameManager.instance.IsPaused() || !canAlterNpcCount) {
			return;
		}

		int hour = GameManager.instance.GetHour();

		int min, max;
		min = max = numAwakeNpcs;

		if (END_BASE_HOUR <= hour && hour < END_PEAK_HOUR) {
			min = PEAK_MIN;
			max = PEAK_MAX;
		} else if (hour < END_BASE_HOUR || END_PEAK_HOUR <= hour) {
			min = BASE_MIN;
			max = BASE_MAX;
		} 

		if (numAwakeNpcs < max) {
			Spawn();
		} else if (numAwakeNpcs > min) {
			Recall();
		}
	}

	private void ActivateNPC(int npcIndex) {
		NPC npc = npcs[npcIndex];

		if (NPCIsInRange(npc)) {
			return;
		}

		npc.Spawn();
		npcAwake[npcIndex] = true;
		numAwakeNpcs++;
	}

	private NPC InstantiateNPC(int index, Vector2 pos) {	
		NPC instance = NPCManager.instance.InstantiateNPC(index, pos);
		instance.InstantiateBySpawner(polyNav, transform);

		instance.OnKnockout += OnNPCKnockOut;

		npcs.Add(instance);
		npcAnimIndexes.Add(index);
		npcAwake.Add(true);
		numAwakeNpcs++;

		return instance;
	}

	private void Spawn() {
		int[] unawakeIndexes = Enumerable.Range(0, npcAwake.Count).Where(i => !npcAwake[i]).ToArray();

        int unawakeIndex = Random.Range(0, unawakeIndexes.Length);
        int npcIndex = unawakeIndexes[unawakeIndex];

        ActivateNPC(npcIndex);
		StartCoroutine(AlterDelay());
	}

	public void SpawnUnconditionally(Vector2 pos) {
		InstantiateNPC(Random.Range(0, NPCManager.instance.npcTypes), (Vector2)pos);
	}

	// deactivates but doesn't delete
	private void Recall() {
		int npcIndex = Random.Range(0, npcs.Count);
		if (CanRecallNPC(npcIndex)) {
			RecallUnconditionally(npcIndex);
			StartCoroutine(AlterDelay());
		}
	}

	// recalls npc no matter where it is
	private void RecallUnconditionally(int npcIndex) {
		npcs[npcIndex].Recall();
		npcAwake[npcIndex] = false;
		numAwakeNpcs--;
	}

	private void RecallUnconditionally(NPC npc) {
		int index = npcs.IndexOf(npc);
		RecallUnconditionally(index);
	}

	private void OnNPCKnockOut(NPC npc) {
		StartCoroutine(WaitForRecall(npc));
	}

	private bool NPCIsInRange(NPC npc) {
		Rect range = GameManager.instance.GetCameraRange(spawnRange + 2 * npcSize);

		return range.Contains((Vector2)(npc.transform.position));
	}

	private bool CanRecallNPC(int npcIndex) {
		return !NPCIsInRange(npcs[npcIndex]) && 
			   npcAwake[npcIndex] &&
			   !npcs[npcIndex].IsFighting() &&
			   !npcs[npcIndex].questActive;
	}

	public int NumNPCs() {
		return npcs.Count;
	}

	public int[] GetNpcIndicies() {
		return npcAnimIndexes.ToArray();
	}

	public NPC[] GetNPCs() {
		return npcs.ToArray();
	}

	// TODO: move next two methods into npc manager
	/// <summary> Gets an NPC object by name </summary>
	public NPC GetNpcByName(string name) {
		foreach (NPC npc in npcs) {
			if (npc.GetName() == name) {
				return npc;
			}
		}
		return null;
	}

	public NPC GetRandomNPC() {
		return GetRandomNPCs(1, new string[]{})[0];
	}

	/// <summary> Gets { count } random NPCs excluing { exclude }</summary>
	public NPC[] GetRandomNPCs(int count, IEnumerable<string> excludeNames) {
		if (count > npcs.Count - excludeNames.Count()) {
			return null;
		}
		
		System.Random rnd = new System.Random();
		NPC[] shuffledNpcs = npcs.OrderBy(x => rnd.Next()).ToArray();    
		NPC[] afterExclude = shuffledNpcs.Where(npc => !excludeNames.Contains(npc.GetName())).ToArray();
		shuffledNpcs = null; // useless beyond this point

		count = Mathf.Min(count, afterExclude.Length);
		NPC[] randomNpcs = new NPC[count];
		for (int i = 0; i < count; i++) {
			randomNpcs[i] = afterExclude[i];
		}
		afterExclude = null; // useless beyond this point
		return randomNpcs;
	}

	public void Save() {
        NPCSpawnerData data = new NPCSpawnerData(this);
		GameManager.Save(data, filename);
    }

    public void Load() {
		filename = Application.persistentDataPath + "/" + gameObject.name + ".dat";
        NPCSpawnerData data = GameManager.Load<NPCSpawnerData>(filename);
		
        if (data != null) {
			int count = data.numNpcs;

			for (int i = 0; i < count; i++) {
				NPC instance = InstantiateNPC(data.npcAnimIndexes[i], Vector2.zero);
				instance.LoadFromData(data.npcDatas[i]);

				if (!data.npcAwake[i] || instance.IsKnockedOut()) {
					RecallUnconditionally(i);
				}
			}
		}
    }

	[System.Serializable]
	public class NPCSpawnerData : GameData {
		public int numNpcs;
		public int[] npcAnimIndexes;
		public bool[] npcAwake;

		public int numAwakeNpcs;

		public NPC.NPCData[] npcDatas;

		public NPCSpawnerData(NPCSpawner spawner) {
			numNpcs = spawner.NumNPCs();
			npcAnimIndexes = spawner.GetNpcIndicies();
			npcAwake = spawner.npcAwake.ToArray();

			numAwakeNpcs = spawner.numAwakeNpcs;

			npcDatas = new NPC.NPCData[numNpcs];
			NPC[] npcs = spawner.GetNPCs();
			for (int i = 0; i < numNpcs; i++) {
				npcDatas[i] = new NPC.NPCData(npcs[i]);
			}
		}
	}
}