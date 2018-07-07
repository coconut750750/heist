using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Recipe {

	public string[] requirements;
	public string result;

	public Recipe(string[] requirements, string result) {
		this.requirements = requirements.OrderByDescending(req => req).ToArray();
		this.result = result;
	}

	public bool IsValidRequirements(string[] inputs) {
		if (requirements.Length != inputs.Length) {
			return false;
		}

		string[] ordered = inputs.OrderByDescending(input => input).ToArray();

		for (int i = 0; i < requirements.Length; i++) {
			if (requirements[i] != ordered[i]) {
				return false;
			}
		}
		return true;
	}
}
