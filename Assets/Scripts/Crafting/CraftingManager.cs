using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CraftingManager : MonoBehaviour {

	public static CraftingManager instance = null;

	[SerializeField]
	private List<Recipe> recipes;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	public Recipe GetRecipe(Item[] inputs) {
		string[] strInputs = inputs.Select(item => item.itemName).ToArray();
		foreach (Recipe recipe in recipes) {
			if (recipe.IsValidRequirements(strInputs)) {
				return recipe;
			}
		}

		return null;
	}

	public Item Craft(Item[] inputs) {
		Recipe recipe = GetRecipe(inputs);

		if (recipe == null) {
			return null;
		}

		Item res = ItemManager.instance.GetItem(recipe.result.itemName);

		float totalQuality = 0;
		foreach (Item item in inputs) {
			totalQuality += item.quality;
		}

		int averageQuality = Mathf.RoundToInt(totalQuality / (float)(inputs.Length));

		res.quality = averageQuality;
		return res;
	}
}
