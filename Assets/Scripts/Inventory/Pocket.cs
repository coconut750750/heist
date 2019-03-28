using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>  
///		This is the Pocket class.
/// 	The Pocket contains the 7 items that are readily accessible to the player.
/// 	This item stash is a single displaying item stash attached to the player. When it displays (always is) 
///  there is only one inventory to display, so it extends the singleton stash.
/// 	SAVING and LOADING: done by this class
/// </summary>  
public class Pocket : SingletonStash {

	public const int NUM_ITEMS = 7;

	private UnityAction<Item, int> selectedConsume;
	private UnityAction deselected;

	public Pocket() : base(NUM_ITEMS) {
	}

	protected override void Start() {
		base.Start();
		foreach (ItemSlot slot in itemSlots) {
			slot.OnSelected += OnSelectedItem;
			slot.OnDeselected += OnDeselectedItem;
		}
	}

	public void SetSelectedConsumeCallback(UnityAction<Item, int> selectedConsume) {
		this.selectedConsume = selectedConsume;
	}

	private void OnSelectedItem(Item item, int index) {
		if (item.consumable && selectedConsume != null) {
			selectedConsume(item, index);
		}
	}

	public void SetDeselectedCallback(UnityAction onDeselected) {
		this.deselected = onDeselected;
	}

	private void OnDeselectedItem() {
		this.deselected();
	}

	public override void Save() {
		ItemStashData data = new ItemStashData(this);
		GameManager.Save(data, base.filename);
	}

	public override void Load() {
		base.Load();
		ItemStashData data = GameManager.Load<ItemStashData>(base.filename);

		if (data != null) {
			base.LoadFromData(data);
			for (int i = 0; i < base.capacity; i++) {
				itemSlots[i].SetItem(base.items[i], this);
			}
		}
	}
}