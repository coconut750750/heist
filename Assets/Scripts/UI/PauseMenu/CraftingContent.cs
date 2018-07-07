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
		Item[] inputs = craftingStash.items.Where(item => item != null).ToArray();
		undoSafety = inputs;

		Item result = CraftingManager.instance.Craft(inputs);

		if (result != null) {			
			craftingStash.RemoveAll();
			craftingStash.SetOutput(result);
		}
	}

	public void Undo() {
		if (undoSafety != null) {
			craftingStash.RemoveAll();
			foreach (Item item in undoSafety) {
				craftingStash.AddItem(item);
			}
		}
	}
}
