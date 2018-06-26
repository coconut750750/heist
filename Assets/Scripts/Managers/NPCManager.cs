using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {

	public NPC npcPrefab;
	public RuntimeAnimatorController[] npcAnimators;
	[HideInInspector]
	public int npcTypes;

	[SerializeField]
	private TextAsset namesAsset;

	private List<string> names;

	public static NPCManager instance = null;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		RetrieveNames();
	}

	void Start() {
		npcTypes = npcAnimators.Length;
	}

	public NPC InstantiateNPC(int index, Vector2 pos) {		
		NPC instance = Instantiate (npcPrefab, (Vector2)pos, Quaternion.identity) as NPC;
		instance.GetComponent<Animator>().runtimeAnimatorController = npcAnimators[index];
		instance.SetName(GetRandomName());
		instance.Spawn();

		return instance;
	}

	private void RetrieveNames() {
		string[] namesArr = namesAsset.text.Split('\n');
		names = new List<string>(namesArr);
	}

	private string GetRandomName() {
		int index = Random.Range(0, names.Count - 1);
		string name = names[index];
		names.RemoveAt(index);
		return name;
	}
}
