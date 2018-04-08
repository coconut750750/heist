using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StashDisplayer : MonoBehaviour {
    private static Inventory displayInventory;
    private static ItemSlot[] itemSlots;
    private static int capacity;

    void Awake() {
        capacity = transform.childCount;
        itemSlots = new ItemSlot[capacity];
        for (int i = 0; i < capacity; i++) {
            itemSlots[i] = transform.GetChild(i).gameObject.GetComponent<ItemSlot>();
            itemSlots[i].SetIndex(i);
        }
    }

    public static void SetInventory(Inventory displayInventory) {
        StashDisplayer.displayInventory = displayInventory;
        displayInventory.SetDisplaying(true);

        for (int i = 0; i < capacity; i++) {
            itemSlots[i].Refresh();
            itemSlots[i].InsertItem(displayInventory.GetItem(i), displayInventory);
        }
    }

    public static void ClearInventory() {
        for (int i = 0; i < displayInventory.GetCapacity(); i++) {
            itemSlots[i].Reset();
        }

        displayInventory.SetDisplaying(false);
        StashDisplayer.displayInventory = null;
    }

    public void DeselectAll() {
        for (int i = 0; i < capacity; i++) {
            itemSlots[i].Deselect();
        }
    }

    public void Close() {
        GameManager.instance.HideInventory();
        ClearInventory();
    }
}
