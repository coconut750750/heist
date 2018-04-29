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

	private Rect cameraRect;
	public int NpcSize;

	void Start () {
		npcs = new List<NPC>();
	}
	
	void Update () {
		int hour = GameManager.instance.GetHour();

		cameraRect = GameManager.instance.GetCameraRect();
		Debug.Log(cameraRect.min + " " + cameraRect.max);

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
		Vector2 pos = GenerateRandomNPCPos();

		NPC toSpawn = npcsToSpawn[Random.Range(0, npcsToSpawn.Length)];
		
		NPC instance = Instantiate (toSpawn, pos, Quaternion.identity) as NPC;
		instance.SetAgentNav(polyNav);
		instance.transform.SetParent(transform);

		npcs.Add(instance);
	}

	void Recall() {
		foreach (NPC npc in npcs) {
			if (!NpcIsVisible(npc)) {
				npcs.Remove(npc);
				Destroy(npc);
				return;
			}
		}
	}

	// returns a vector 2 position one past the border of the camera to ensure it is close enough to player 
	// to be relevant
	Vector2 GenerateRandomNPCPos() {
		int side = Random.Range(0, 4);
		switch (side) {
			case 0: // left
				float y = Random.Range(cameraRect.min.y, cameraRect.max.y);
				return new Vector2(cameraRect.min.x - 1, y);
			case 1: // top
				float x = Random.Range(cameraRect.min.x, cameraRect.max.x);
				return new Vector2(x, cameraRect.max.y + 1);
			case 2: // right
				y = Random.Range(cameraRect.min.y, cameraRect.max.y);
				return new Vector2(cameraRect.max.x + 1, y);
			default: // down
				x = Random.Range(cameraRect.min.x, cameraRect.max.x);
				return new Vector2(x, cameraRect.min.y - 1);
			
		}
	}

	// checks to see if NPC is in the range of the camera
	bool NpcIsVisible(NPC npc) {
		Rect inflatedCamera = new Rect();
		inflatedCamera.position = cameraRect.position;
		inflatedCamera.width = cameraRect.width + 2 * NpcSize;
		inflatedCamera.height = cameraRect.height + 2 * NpcSize;
		return inflatedCamera.Contains((Vector2)(npc.transform.position));
	}
}
