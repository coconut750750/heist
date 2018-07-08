using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CraftingContent : MonoBehaviour {

	private CraftingStash craftingStash;
	private Item[] undoSafety;

	void Awake () {
		craftingStash = gameObject.GetComponent<CraftingStash>();
	}

	void OnEnable() {
		craftingStash.Display();
	}

	void OnDisable() {
		undoSafety = null;
		craftingStash.Hide();
	}

	public CraftingStash GetStash() {
		return craftingStash;
	}

	public void TryCrafting() {
		Item[] inputs = craftingStash.GetInputs();
		undoSafety = inputs;

		Item[] results = CraftingManager.instance.CraftWithUnused(inputs);

		if (results != null) {
			craftingStash.RemoveAll();
			for (int i = 0; i < results.Length - 1; i++) {
				craftingStash.AddItem(results[i]);
			}
			craftingStash.SetOutput(results.Last());
		}
	}

	public void Undo() {
		if (undoSafety != null && craftingStash.GetOutput() != null) {
			craftingStash.RemoveAll();
			foreach (Item item in undoSafety) {
				craftingStash.AddItem(item);
			}
		}
	}
}
