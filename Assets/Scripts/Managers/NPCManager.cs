using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {

	public NPC npcPrefab;
	public RuntimeAnimatorController[] npcAnimators;
	public int npcTypes;

	public static NPCManager instance = null;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
	}

	void Start() {
		npcTypes = npcAnimators.Length;
	}

	public NPC InstantiateNPC(int index, Vector2 pos) {		
		NPC instance = Instantiate (npcPrefab, (Vector2)pos, Quaternion.identity) as NPC;
		instance.GetComponent<Animator>().runtimeAnimatorController = npcAnimators[index];
		instance.Spawn();

		return instance;
	}

	// TODO: randomize npc names
}
