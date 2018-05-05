using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StashDisplayer : MonoBehaviour {
    private static Inventory displayInventory;
    private static ItemSlot[] itemSlots;
    private static int capacity;
    private static Text nameText;

    void Awake() {
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

    public static void SetInventory(Inventory displayInventory) {
        StashDisplayer.displayInventory = displayInventory;
        displayInventory.SetDisplaying(true);

        for (int i = 0; i < capacity; i++) {
            itemSlots[i].Refresh();
            itemSlots[i].InsertItem(displayInventory.GetItem(i), displayInventory);
        }

        nameText.text = displayInventory.GetName();
    }

    public static void ClearInventory() {
        for (int i = 0; i < displayInventory.GetCapacity(); i++) {
            itemSlots[i].ResetItem();
        }

        displayInventory.SetDisplaying(false);
        StashDisplayer.displayInventory = null;

        nameText.text = "";
    }

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
