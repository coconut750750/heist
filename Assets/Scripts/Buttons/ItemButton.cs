using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemButton : Button {

	private UnityAction call;
	public Item item;
	public static Text text = null;

	protected override void Awake() {
		base.Awake();
		Reset();
		if (text == null) {
			text = GameObject.Find("SelectedInventoryItemText").GetComponent<Text>();
		}
	}

	public void SetItem(Item item) {
		enabled = true;
		this.item = item;
		call = delegate {
            text.text = item.name;
        };
		base.onClick.AddListener(call);
	}

	public void Reset() {
		this.item = null;
		call = null;
		base.onClick.RemoveAllListeners();
		enabled = false;
	}
}
