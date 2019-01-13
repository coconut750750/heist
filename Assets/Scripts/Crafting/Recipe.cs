using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Recipe : ScriptableObject {

	public Item[] requirements;
	public int[] unusedIndicies;
	public Item result;
	public bool canDismantle;

	private string[] strReqs {
		get {
			string[] reqs = requirements.Select(item => item.itemName).ToArray();
			return reqs.OrderByDescending(req => req).ToArray();
		}
	}

	// Unused items are items that are required for crafting but are not exhausted
	public int NumUnused() {
		return unusedIndicies.Length;
	}

	public bool IsValidRequirements(string[] inputs) {
		if (requirements.Length != inputs.Length) {
			return false;
		}

		string[] ordered = inputs.OrderByDescending(input => input).ToArray();

		for (int i = 0; i < requirements.Length; i++) {
			if (strReqs[i] != ordered[i]) {
				return false;
			}
		}
		return true;
	}
}
