using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using UnityEngine.Events;

/// <summary>  
///		This is the Item Slot class.
/// 	It holds the data of the item (the name and image) the background color and parent stash.
/// </summary>  
public class ItemSlot : MonoBehaviour, IDropHandler {
	private Image itemImage;
	private Image itemBack;
	public Item item;

	public static Text nameText = null;
	public static Text qualityText = null;

	private ItemStash parentStash;
	[SerializeField]
	private int index;
	private bool selected;
	private bool inputAllowed = true; // true if player can move stuff into this slot
	private bool outputAllowed = true; // true if player can move stuff out of this slot

	private static Color SELECTED_COLOR = new Color(255/255f, 147/255f, 76/255f);
	private static Color DEFAULT_COLOR = new Color(1, 1, 1);

	private const string ITEM_IMAGE = "ItemImage";
	private const string BACKGROUND_IMAGE = "BackgroundImage";
	private const string INVENTORY_ITEM_TEXT = "SelectedInventoryItemText";
	private const string INVENTORY_ITEM_QUALITY = "SelectedInventoryItemQuality";

	public UnityAction<Item, int> OnSelected;
	public UnityAction OnDeselected;
	public UnityAction<Item> OnDropped;
	public UnityAction OnRemoved;

	void Awake() {
		if (nameText == null) {
			nameText = GameObject.Find(INVENTORY_ITEM_TEXT).GetComponent<Text>();
			qualityText = GameObject.Find(INVENTORY_ITEM_QUALITY).GetComponent<Text>();
		}
		itemImage = transform.Find(ITEM_IMAGE).gameObject.GetComponent<Image>();
		itemBack = transform.Find(BACKGROUND_IMAGE).gameObject.GetComponent<Image>();
		ClearItem();
	}

	void OnDestroy() {
		nameText.text = "";
        qualityText.text = "";
	}

	public void SetIndex(int i) {
		index = i;
	}

	public int GetIndex() {
		return index;
	}

	public void SetParentStash(ItemStash parent) {
		parentStash = parent;
	}

	public ItemStash GetParentStash() {
		return parentStash;
	}

	public void SetItem(Item item, ItemStash parent) {
		SetParentStash(parent);
		SetItem(item);
	}

	public void SetItem(Item item) {
		if (item == null) {
			ClearItem();
			return;
		}
		
		itemImage.sprite = item.sprite;
		itemImage.enabled = true;
		this.item = item;
	}

	public void ClearItem() {
		itemImage.sprite = null;
		itemImage.enabled = false;
		this.item = null;

		Deselect();
	}

	public Item GetItem() {
		return this.item;
	}

	public Image GetItemImage() {
		return this.itemImage;
	}

	private void DeselectAllSlots() {
		ItemSlot[] itemSlots = GameObject.FindObjectsOfType<ItemSlot>();
		foreach (ItemSlot slot in itemSlots) {
			slot.Deselect();
		}
	}

	public void Select() {
		if (!selected && item != null) {
			DeselectAllSlots();
			
			selected = true;
			SetSelectedItemInfo(item.itemName, item.quality + "%");
			itemBack.color = SELECTED_COLOR;

			if (OnSelected != null) {
				OnSelected(this.item, index);
			}
		}
	}

	public void ToggleSelect() {
		if (selected) {
			Deselect();
		} else {
			Select();
		}
	}

	public void Deselect() {
		if (selected) {
			selected = false;
			SetSelectedItemInfo("", "");
			itemBack.color = DEFAULT_COLOR;

			if (OnDeselected != null) {
				OnDeselected();
			}
		}
	}

	private void SetSelectedItemInfo(string name, string quality) {
		nameText.text = name;
		qualityText.text = quality;
	}

	public bool InputAllowed() {
		return this.inputAllowed;
	}

	public void SetInputAllowed(bool allowed) {
		this.inputAllowed = allowed;
	}

	public bool OutputAllowed() {
		return this.outputAllowed;
	}

	public void SetOutputAllowed(bool allowed) {
		this.outputAllowed = allowed;
	}

	public bool IsEmpty() {
		return this.item == null;
	}

    public void OnDrop(PointerEventData eventData)
    {
		if (!inputAllowed || ItemDragger.itemBeingDragged == null) {
			return;
		}

		ItemSlot itemSlotOther = ItemDragger.itemBeingDragged.GetParentSlot();

		SwapItemsInSlots(itemSlotOther);
		SwapParentStashPositions(itemSlotOther);
		SwapSlotSelection(itemSlotOther);

		// trigger callbacks
		if (OnDropped != null) {
			OnDropped(this.item);
		}

		if (itemSlotOther.OnRemoved != null && itemSlotOther.GetItem() == null) {
			itemSlotOther.OnRemoved();
		}
    }

	private void SwapItemsInSlots(ItemSlot other) {
		Item prevItem = GetItem();
		Item prevOtherItem = other.GetItem();

		SetItem(prevOtherItem);
		other.SetItem(prevItem);
	}

	private void SwapParentStashPositions(ItemSlot other) {
		int indexOther = other.GetIndex();
		if (parentStash == other.GetParentStash()) {
			parentStash.SwapItemPositions(indexOther, index);
		} else if (other.GetParentStash() != null && parentStash != null) {			
			other.GetParentStash().SetItemAtIndex(other.GetItem(), indexOther);
			parentStash.SetItemAtIndex(GetItem(), index);
		}
	}

	private void SwapSlotSelection(ItemSlot other) {
		other.Deselect();
		Select();
	}
}
