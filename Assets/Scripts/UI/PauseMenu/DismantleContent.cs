using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DismantleContent : MonoBehaviour {

	private DismantleStash dismantleStash;
	private Item undoSafety;

	void Awake () {
		dismantleStash = gameObject.GetComponent<DismantleStash>();
	}
	
	void OnEnable() {
		dismantleStash.Display();
	}

	void OnDisable() {
		undoSafety = null;
		dismantleStash.Hide();
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
		}
	}
}
