using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class DismantleContent : MonoBehaviour {

	private DismantleStash dismantleStash;
	private Item undoSafety;

	public Button dismantleButton;
	public Button undoButton;

	void Awake () {
		dismantleStash = gameObject.GetComponent<DismantleStash>();
		dismantleStash.SetOutputRemovedCallBack(EnableDismantle);
	}
	
	void OnEnable() {
		dismantleStash.Display();
		EnableDismantle();
	}

	void OnDisable() {
		undoSafety = null;
		dismantleStash.Hide();
	}

	void EnableDismantle() {
		if (dismantleStash.ReadyForDismantle()) {
			dismantleButton.interactable = true;
		}
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

	public void Undo() {
		if (undoSafety == null) {
			return;
		}
		Item reverse = CraftingManager.instance.Craft(dismantleStash.GetOutput());
		if (reverse == null) {
			return;
		}
		if (reverse.name == undoSafety.name) {
			dismantleStash.RemoveAll();
			dismantleStash.AddItem(undoSafety);
			EnableDismantle();
		}
	}
}
