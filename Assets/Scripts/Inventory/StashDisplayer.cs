using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>  
///		This is the Stash Displayer class.
/// 	It manages the display of inventories when the player interacts with inventories. It takes
///  an inventory and injects the data into the ItemSlot objects it holds.
/// </summary>  
public class StashDisplayer : MonoBehaviour {

    public static StashDisplayer instance = null;

    private static Inventory displayInventory;
    private static ItemSlot[] itemSlots;
    private static int capacity;
    private static Text nameText;

    // set up the itemSlots
    void Awake() {
        if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
        
        Transform inventory = transform.Find("Inventory");
        capacity = inventory.childCount;
        itemSlots = new ItemSlot[capacity];
        for (int i = 0; i < capacity; i++) {
            itemSlots[i] = inventory.GetChild(i).gameObject.GetComponent<ItemSlot>();
            itemSlots[i].SetIndex(i);
        }

        nameText = transform.Find("InventoryName").GetComponent<Text>();

        gameObject.SetActive(false);
    }

    // injects an inventory into the item slots
    public static void SetInventory(Inventory displayInventory) {
        StashDisplayer.displayInventory = displayInventory;
        displayInventory.SetDisplaying(true);

        for (int i = 0; i < capacity; i++) {
            itemSlots[i].ClearItem();
            itemSlots[i].SetItem(displayInventory.GetItem(i), displayInventory);
        }

        nameText.text = displayInventory.GetName();
    }

    // removes the inventory data from item slots
    public static void ClearInventory() {
        for (int i = 0; i < displayInventory.GetCapacity(); i++) {
            itemSlots[i].ClearItem();
        }

        displayInventory.SetDisplaying(false);
        StashDisplayer.displayInventory = null;

        nameText.text = "";
    }

    // deselects all the item slots
    public void DeselectAll() {
        for (int i = 0; i < capacity; i++) {
            itemSlots[i].Deselect();
        }
    }

    public void DisplayInventory(Inventory stash) {
        GameManager.instance.PauseGame();

		gameObject.SetActive(true);
		StashDisplayer.SetInventory(stash);
	}

	public void HideInventory() {
        GameManager.instance.UnpauseGame();

		gameObject.SetActive(false);
		StashDisplayer.ClearInventory();
	}
}
