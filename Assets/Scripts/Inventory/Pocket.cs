using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Pocket : ItemStash {

	public const int NUM_ITEMS = 7;

	public Image[] itemImages = new Image[NUM_ITEMS];

	public Pocket() : base(NUM_ITEMS) {

	}

	void Awake() {
		SetItemSlots();
	}

	protected void SetItemSlots() {
		for (int i = 0; i < capacity; i++) {
			itemImages[i].transform.parent.GetComponent<ItemSlot>().SetIndex(i);
			itemImages[i].transform.parent.GetComponent<ItemSlot>().SetParentStash(this);
		}
	}
	
	public override bool AddItemAtIndex(Item itemToAdd, int index) {
		bool success = base.AddItemAtIndex(itemToAdd, index);

		if (success) {
			itemImages[index].transform.parent.GetComponent<ItemSlot>().SetItem(itemToAdd);
		}

		return success;
	}

	public override bool RemoveItemAtIndex(int index) {
		bool success = base.RemoveItemAtIndex(index);

		if (success) {
			itemImages[index].transform.parent.GetComponent<ItemSlot>().Reset();
		}
			
		return success;
	}

	public override void DeselectAll() {
		for (int i = 0; i < capacity; i++) {
			itemImages[i].transform.parent.GetComponent<ItemSlot>().Deselect();
		}
	}

	public override void SetDisplaying(bool isDisplaying) {
		// do nothing, its always displaying
	}

    public override bool IsDisplaying() {
        return true;
    }

    public override void Save() {
		ItemStashData data = new ItemStashData(base.items, base.count, base.capacity);

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(base.filename);
		Debug.Log(File.Exists(base.filename));

		bf.Serialize(file, data);
		file.Close();
    }

    public override void Load() {
        if (File.Exists(base.filename)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(base.filename, FileMode.Open);

            ItemStashData data = bf.Deserialize(file) as ItemStashData;
            file.Close();

            base.LoadFromData(data);
			for (int i = 0; i < base.capacity; i++) {
				itemImages[i].transform.parent.GetComponent<ItemSlot>().SetItem(base.items[i]);
			}
        }
    }
}