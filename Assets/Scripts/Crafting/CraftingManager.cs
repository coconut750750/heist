using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CraftingManager : MonoBehaviour {
	public const float DISMANTLE_PERCENT = 0.75f;
	public const int UNUSED_ITEM_DECREASE = 25;

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

	private Recipe GetRecipeByInput(Item[] inputs) {
		string[] strInputs = inputs.Select(item => item.itemName).ToArray();
		foreach (Recipe recipe in recipes) {
			if (recipe.IsValidRequirements(strInputs)) {
				return recipe;
			}
		}

		return null;
	}

	private Recipe GetRecipeByOutput(Item output) {
		foreach (Recipe recipe in recipes) {
			if (recipe.result.itemName == output.itemName) {
				return recipe;
			}
		}

		return null;
	}

	public Item Craft(Item[] inputs) {
		Item[] withUnused = CraftWithUnused(inputs);
		if (withUnused == null) {
			return null;
		}
		return withUnused.Last();
	}

	public Item[] CraftWithUnused(Item[] inputs) {
		inputs = inputs.Where(item => item != null).ToArray();
		inputs = inputs.OrderByDescending(item => item.itemName).ToArray();
		Recipe recipe = GetRecipeByInput(inputs);

		if (recipe == null) {
			return null;
		}

		Item resultItem = ItemManager.instance.GetItem(recipe.result.itemName);

		float totalQuality = 0;
		foreach (Item item in inputs) {
			totalQuality += item.quality;
		}
		int averageQuality = Mathf.RoundToInt(totalQuality / (float)(inputs.Length));
		resultItem.quality = averageQuality;

		Item[] resultItems = new Item[recipe.NumUnused() + 1];
		for (int i = 0; i < recipe.NumUnused(); i++) {
			Item unused = inputs[recipe.unusedIndicies[i]];
			unused.quality -= UNUSED_ITEM_DECREASE;
			if (unused.quality <= 0) {
				unused = null;
			}
			resultItems[i] = unused;
		}
		resultItems[recipe.NumUnused()] = resultItem;
		
		return resultItems;
	}

	public Item[] Dismantle(Item input) {
		Recipe recipe = GetRecipeByOutput(input);
		if (recipe == null || !recipe.canDismantle) {
			return null;
		}

		Item[] inputs = recipe.requirements;

		Item[] instantiatedItems = new Item[inputs.Length];
		for (int i = 0; i < inputs.Length; i++) {
			instantiatedItems[i] = ItemManager.instance.GetItem(inputs[i].itemName);
			instantiatedItems[i].quality = Mathf.RoundToInt((float)(input.quality) * DISMANTLE_PERCENT);
		}

		return instantiatedItems;
	}
}
