using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeManager {

	private List<Recipe> recipes;

	public RecipeManager() {
		recipes = new List<Recipe>();

		Recipe recipe = new Recipe(new string[]{"Scrap Plastic","Scrap Plastic","Lighter"}, "Blank Card");
		recipes.Add(recipe);
	}

	public bool ValidRecipe(Item[] requirements) {
		foreach (Recipe recipe in recipes) {
			if (recipe.IsValidRequirements(requirements.Select(item => item.itemName).ToArray())) {
				return true;
			}
		}

		return false;
	}
}
