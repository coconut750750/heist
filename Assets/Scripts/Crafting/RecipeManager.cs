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

	public Recipe GetRecipe(Item[] inputs) {
		string[] strInputs = inputs.Select(item => item.itemName).ToArray();
		foreach (Recipe recipe in recipes) {
			if (recipe.IsValidRequirements(strInputs)) {
				return recipe;
			}
		}

		return null;
	}
}
