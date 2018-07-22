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

    public ItemSlot itemSlotPrefab;

    private Inventory displayInventory;
    private ItemSlot[] itemSlots;
    private int capacity;
    private Text nameText;
    private Transform parentInventory;

    // set up the itemSlots
    void Awake() {
        if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}
        
        parentInventory = transform.Find("Inventory");
        nameText = transform.Find("InventoryName").GetComponent<Text>();

        gameObject.SetActive(false);
    }

    // injects an inventory into the item slots
    public void SetInventory(Inventory displayInventory) {
        capacity = displayInventory.GetCapacity();
        itemSlots = new ItemSlot[capacity];
        for (int i = 0; i < capacity; i++) {
            ItemSlot newItemSlow = Instantiate(itemSlotPrefab, parentInventory);
            itemSlots[i] = newItemSlow;
            itemSlots[i].SetIndex(i);
            itemSlots[i].SetItem(displayInventory.GetItem(i), displayInventory);
        }
        
        this.displayInventory = displayInventory;
        displayInventory.SetDisplaying(true);

        nameText.text = displayInventory.GetName();
    }

    // removes the inventory data from item slots
    public void ClearInventory() {
        capacity = 0;
        itemSlots = null;
        foreach (Transform child in parentInventory) {
            Destroy(child.gameObject);
        }

        displayInventory.SetDisplaying(false);
        this.displayInventory = null;

        nameText.text = "";
    }

    // deselects all the item slots
    public void DeselectAll() {
        for (int i = 0; i < capacity; i++) {
            itemSlots[i].Deselect();
        }
    }

    public void DisplayInventory(Inventory stash) {
		gameObject.SetActive(true);
		this.SetInventory(stash);
	}

	public void HideInventory() {
		gameObject.SetActive(false);
		this.ClearInventory();
	}
}
