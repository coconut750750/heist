using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemButton : Button {

	private UnityAction call;
	public Item item;
	public static Text text = null;
	private bool selected;
	private Image itemBack;

	private static Color SELECTED_COLOR = new Color(255/255f, 147/255f, 76/255f);
	private static Color DEFAULT_COLOR = new Color(1, 1, 1);

	protected override void Awake() {
		base.Awake();
		Reset();
		if (text == null) {
			text = GameObject.Find("SelectedInventoryItemText").GetComponent<Text>();
		}
		itemBack = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
	}

	public void SetItem(Item item) {
		enabled = true;
		this.item = item;
		call = delegate {
			if (!selected) {
				text.text = item.name;
				selected = true;
				itemBack.color = SELECTED_COLOR;
			} else {
				text.text = "";
				selected = false;
				itemBack.color = DEFAULT_COLOR;
			}
        };
		base.onClick.AddListener(call);
	}

	public new void Reset() {
		this.item = null;
		call = null;
		base.onClick.RemoveAllListeners();
		enabled = false;
		this.selected = false;
	}
}
