using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Recipe : ScriptableObject {

	public Item[] requirements;
	public Item result;

	private string[] strReqs {
		get {
			return requirements.Select(item => item.itemName).ToArray();
		}
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
