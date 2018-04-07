using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StashDisplayer : MonoBehaviour {
    private static Inventory displayInventory;

    void OnEnable() {
        for (int i = 0; i < displayInventory.GetCapacity(); i++) {
            transform.GetChild(i).GetComponent<ItemSlot>().SetItem(displayInventory.GetItem(i));
        }
    }
}
