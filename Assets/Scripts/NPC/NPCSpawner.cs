using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour {

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

	public int npcSize;
	public int spawnRange;

	public float spawnDelay;
	private bool canSpawn;

	IEnumerator SpawnDelay() {
		canSpawn = false;
		yield return new WaitForSeconds(spawnDelay);
		canSpawn = true;
	}

	void Start () {
		npcs = new List<NPC>();
		canSpawn = true;
	}
	
	void Update () {
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

	void Spawn() {
		if (!canSpawn) {
			return;
		}

		Vector2? pos = GenerateRandomNPCPos();
		if (pos == null) {
			return;
		}

		NPC toSpawn = npcsToSpawn[Random.Range(0, npcsToSpawn.Length)];
		
		NPC instance = Instantiate (toSpawn, (Vector2)pos, Quaternion.identity) as NPC;
		instance.SetAgentNav(polyNav);
		instance.transform.SetParent(transform);

		npcs.Add(instance);

		StartCoroutine(SpawnDelay());
	}

	void Recall() {
		foreach (NPC npc in npcs) {
			if (!NpcIsInRange(npc)) {
				npcs.Remove(npc);
				Destroy(npc);
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
				float y = Random.Range(range.min.y, range.max.y);
				pos = new Vector2(range.min.x - 1, y);
				break;
			case 1: // top
				float x = Random.Range(range.min.x, range.max.x);
				pos = new Vector2(x, range.max.y + 1);
				break;
			case 2: // right
				y = Random.Range(range.min.y, range.max.y);
				pos = new Vector2(range.max.x + 1, y);
				break;
			default: // down
				x = Random.Range(range.min.x, range.max.x);
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
}
