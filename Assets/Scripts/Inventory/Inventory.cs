using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public override void DeselectAll() {
        if (isDisplaying && GameManager.instance.stashDisplayer.isActiveAndEnabled) {
            GameManager.instance.stashDisplayer.DeselectAll();
		} else if (isDisplaying && NPCUI.instance.isActiveAndEnabled) {
            NPCUI.instance.DeselectAll();
        }
    }

    public override void Save() {
        if (independent) {
            ItemStashData data = new ItemStashData(this);
            GameManager.Save(data, base.filename);
        }
    }

    public override void Load() {
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