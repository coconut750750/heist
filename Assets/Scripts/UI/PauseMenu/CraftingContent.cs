using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class CraftingContent : MonoBehaviour {

	private CraftingStash craftingStash;
	private Item[] undoSafety;

	public Button craftButton;
	public Button undoButton;

	void Awake () {
		craftingStash = gameObject.GetComponent<CraftingStash>();
		craftingStash.SetOutputRemovedCallBack(EnableCraft);
	}

	void OnEnable() {
		craftingStash.Display();
		undoButton.interactable = false;
	}

	void OnDisable() {
		undoSafety = null;
		craftingStash.Hide();
	}

	void EnableCraft() {
		craftButton.interactable = true;
		undoButton.interactable = false;
	}

	void EnableUndo() {
		craftButton.interactable = false;
		undoButton.interactable = true;
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

			EnableUndo();
		}
	}

	public void Undo() {
		if (undoSafety != null && craftingStash.GetOutput() != null) {
			craftingStash.RemoveAll();
			foreach (Item item in undoSafety) {
				craftingStash.AddItem(item);
			}
			EnableCraft();
		}
	}
}
