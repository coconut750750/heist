using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>  
///		This is the Inventory class.
/// 	This item stash is a non-displaying item stash. The player may only interact with this inventory
///  when it is displayed. When this inventory is displayed, the StashDisplayer retrieves its data to display.
///     SAVING and LOADING:
///         saving and loading: will save data IF NOT attached to an NPC (will save if it represents a desk)
/// </summary>  
public class Inventory : ItemStash {

    [SerializeField]
    private string inventoryName;

	private bool isDisplaying;

    // independent if inventory not attached to an npc
    private bool independent;

	public Inventory() {
		isDisplaying = false;
        independent = true;
	}

	void Awake () {
        capacity = base.items.Length;
	}

    public string GetName() {
        return inventoryName;
    }

    public void SetName(string name) {
        this.inventoryName = name;
    }

    public bool IsIndependent() {
        return independent;
    }

    public void SetIndependent(bool independent) {
        this.independent = independent;
    }

    public override void SetDisplaying(bool isDisplaying) {
        this.isDisplaying = isDisplaying;
    }

    public override bool IsDisplaying() {
        return isDisplaying;
    }
    
    public override void Save() {
        if (independent) {
            ItemStashData data = new ItemStashData(this);
            GameManager.Save(data, base.filename);
        }
    }

    public override void Load() {
        base.Load();
        if (independent) {
            ItemStashData data = GameManager.Load<ItemStashData>(base.filename);

            LoadFromInventoryData(data);
        }
    }

    public void LoadFromInventoryData(ItemStashData data) {
        if (data != null) {
			base.LoadFromData(data);
            this.isDisplaying = false;
		}
    }
}