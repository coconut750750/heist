using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CraftingManager : MonoBehaviour {

	public static CraftingManager instance = null;

	public RecipeManager recipeManager = null;

	void Awake() {
		if (instance == null) {
			instance = this;
			recipeManager = new RecipeManager();
		} else {
			Destroy(gameObject);
		}
	}

	public Item Craft(Item[] inputs) {
		Recipe recipe = recipeManager.GetRecipe(inputs);

		if (recipe == null) {
			return null;
		}

		Item res = ItemManager.instance.GetItem(recipe.result);

		float totalQuality = 0;
		foreach (Item item in inputs) {
			totalQuality += item.quality;
		}

		int averageQuality = Mathf.RoundToInt(totalQuality / (float)(inputs.Length));

		res.quality = averageQuality;
		return res;
	}
}
