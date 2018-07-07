using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour {

	public static CraftingManager instance = null;

	public RecipeManager recipeManager = null;

	public Item[] inputs;

	void Awake() {
		if (instance == null) {
			instance = this;
			recipeManager = new RecipeManager();
		} else {
			Destroy(gameObject);
		}
		print(recipeManager.ValidRecipe(inputs));
	}

	public Item TryCraft(Item[] items) {



		return null;
	}
}
