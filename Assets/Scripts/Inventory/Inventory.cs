using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Inventory : ItemStash {

	public const int NUM_ITEMS = 24;

	private bool isDisplaying;

	public Inventory() : base(NUM_ITEMS) {
		isDisplaying = false;
	}

	void Awake () {

	}

    public override void SetDisplaying(bool isDisplaying) {
        this.isDisplaying = isDisplaying;
    }

    public override bool IsDisplaying() {
        return isDisplaying;
    }

    public override void DeselectAll() {
        if (isDisplaying) {
			GameManager.instance.stashDisplayer.DeselectAll();
		}
    }

    public override void Save() {
        ItemStashData data = new ItemStashData(base.items, base.count, base.capacity);

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(base.filename);

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
            this.isDisplaying = false;
        }
    }
}