using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : ItemStash {

    [SerializeField]
    private string inventoryName;

	private bool isDisplaying;

	public Inventory() {
		isDisplaying = false;
	}

	void Awake () {
        capacity = base.items.Length;
	}

    public string GetName() {
        return inventoryName;
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
            NPCUI.instance.DeselectAllItemSlots();
        }
    }

    public override void Save() {
        ItemStashData data = new ItemStashData(base.items, base.count, base.capacity);
        GameManager.Save(data, base.filename);
    }

    public override void Load() {
        ItemStashData data = GameManager.Load<ItemStashData>(base.filename);

		if (data != null) {
			base.LoadFromData(data);
            this.isDisplaying = false;
		}
    }
}