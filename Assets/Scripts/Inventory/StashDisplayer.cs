using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashDisplayer : MonoBehaviour {
    private static Inventory displayInventory;
    private static ItemSlot[] itemSlots;
    private int capacity;

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
    }

    void OnEnable() {
        if (displayInventory == null) {
            return;
        }        

        for (int i = 0; i < capacity; i++) {
            itemSlots[i].Refresh();
            if (displayInventory.GetItem(i) != null) {
                itemSlots[i].SetItem(displayInventory.GetItem(i));
            }
            itemSlots[i].SetParentStash(displayInventory);
        }
    }

    void OnDisable() {
        if (displayInventory == null) {
            return;
        }

        for (int i = 0; i < displayInventory.GetCapacity(); i++) {
            itemSlots[i].Reset();
        }
    }

    public void DeselectAll() {
        for (int i = 0; i < capacity; i++) {
            itemSlots[i].Deselect();
        }
    }

    public void Close() {
        GameManager.HideStash();
        displayInventory.SetDisplaying(false);
        StashDisplayer.displayInventory = null;
    }
}
