using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DismantleContent : MonoBehaviour {

	private DismantleStash dismantleStash;
	private Item undoSafety; // used to save the item in case user wants to undo

	public Button dismantleButton;
	public Button undoButton;

	void Awake () {
		dismantleStash = gameObject.GetComponent<DismantleStash>();
		dismantleStash.SetOutputRemovedCallBack(EnableDismantle);
	}
	
	void OnEnable() {
		EnableDismantle();
	}

	void OnDisable() {
		undoSafety = null;
		dismantleStash.Hide();
	}

	void EnableDismantle() {
		dismantleButton.interactable = dismantleStash.ReadyForDismantle();
		undoButton.interactable = false;
	}

	void EnableUndo() {
		dismantleButton.interactable = false;
		undoButton.interactable = true;
	}

	public DismantleStash GetStash() {
		return dismantleStash;
	}

	public void TryDismantle() {
		Item input = dismantleStash.GetInput();
		undoSafety = input;

		Item[] result = CraftingManager.instance.Dismantle(input);

		if (result != null) {			
			dismantleStash.RemoveAll();
			dismantleStash.SetOutput(result);
			EnableUndo();
		}
	}

	// STATE: 3 outputs, 1 empty input
	// if user decides to under, we first try to craft whats in the output
	// if the result is the same as undoSafety, we return the item to the user and clear the output
	public void Undo() {
		if (undoSafety == null) {
			return;
		}

		Item reverse = CraftingManager.instance.CraftIgnoreUnused(dismantleStash.GetOutput());
		if (reverse != null && reverse.name == undoSafety.name) {
			dismantleStash.RemoveAll();
			dismantleStash.AddItem(undoSafety);
			EnableDismantle();
		}
	}
}
