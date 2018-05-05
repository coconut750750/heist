using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

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

	// called when an item is selected
	public event Action<Item> OnSelected;

	public event Action<Item> OnDeselected;

	public event Action<Item> OnDropped;

	public event Action OnRemoved;

	void Awake() {
		Refresh();
	}

	public void Refresh() {
		if (nameText == null) {
			nameText = GameObject.Find(INVENTORY_ITEM_TEXT).GetComponent<Text>();
			qualityText = GameObject.Find(INVENTORY_ITEM_QUALITY).GetComponent<Text>();
		}
		itemImage = transform.Find(ITEM_IMAGE).gameObject.GetComponent<Image>();
		itemBack = transform.Find(BACKGROUND_IMAGE).gameObject.GetComponent<Image>();
		ResetItem();
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

	public void InsertItem(Item item, ItemStash parent) {
		SetParentStash(parent);
		SetItem(item);
	}

	public void SetItem(Item item) {
		if (item == null) {
			ResetItem();
			return;
		}
		
		itemImage.sprite = item.sprite;
		itemImage.enabled = true;
		this.item = item;
		
	}

	public Item GetItem() {
		return this.item;
	}

	public Image GetItemImage() {
		return this.itemImage;
	}

	public void Select() {
		if (parentStash != GameManager.instance.stashDisplayer) {
			GameManager.instance.stashDisplayer.DeselectAll();
		}
		if (parentStash != GameManager.instance.mainPlayer.GetPocket()) {
			GameManager.instance.mainPlayer.GetPocket().DeselectAll();
		}
		if (GameManager.instance.IsPaused()) {
			if (PauseMenu.instance.isActiveAndEnabled) {
				PauseMenu.instance.GetActiveStash().DeselectAll();
			} else if (NPCUI.instance.isActiveAndEnabled) {
				NPCUI.instance.DeselectAll();
			}
		}

		if (parentStash != null) {
			parentStash.DeselectAll();
		}
		
		nameText.text = item.name;
		qualityText.text = item.quality + "%";
		itemBack.color = SELECTED_COLOR;

		selected = true;
		
		if (OnSelected != null) {
			OnSelected(this.item);
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
			nameText.text = "";
			qualityText.text = "";
			itemBack.color = DEFAULT_COLOR;

			if (OnDeselected != null) {
				OnDeselected(this.item);
			}
		}
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

	public void ResetItem() {
		itemImage.sprite = null;
		itemImage.enabled = false;
		this.item = null;

		Deselect();
	}

	public bool IsEmpty() {
		return this.item == null;
	}

    public void OnDrop(PointerEventData eventData)
    {
		if (!inputAllowed) {
			return;
		}
		Image imageDragged = ItemDragger.itemBeingDragged.GetImage();
		ItemSlot itemSlotOther = imageDragged.GetComponent<ItemDragger>().GetParentSlot();
		
		if (!itemSlotOther.outputAllowed) {
			return;
		}

		// swap images
		Sprite tempSprite = itemImage.sprite;
		bool tempEnabled = itemImage.enabled;
		itemImage.sprite = imageDragged.sprite;
		itemImage.enabled = true;

		imageDragged.sprite = tempSprite;
		imageDragged.enabled = tempEnabled;

		// swap item slot items
		Item tempItem1 = GetItem();
		Item tempItem2 = itemSlotOther.GetItem();

		SetItem(tempItem2);
		itemSlotOther.SetItem(tempItem1);
		
		// swap parent stash positions
		int indexOther = itemSlotOther.GetIndex();
		if (parentStash == itemSlotOther.GetParentStash()) {
			parentStash.SwapItemPositions(indexOther, index);
		} else if (itemSlotOther.GetParentStash() != null && parentStash != null) { 
			// different parent stashes, so item slot other should be deselected
			itemSlotOther.GetParentStash().RemoveItemAtIndex(indexOther);
			itemSlotOther.GetParentStash().AddItemAtIndex(tempItem1, indexOther);

			parentStash.RemoveItemAtIndex(index);
			parentStash.AddItemAtIndex(tempItem2, index);
		}

		itemSlotOther.Deselect();
		Select();

		if (OnDropped != null) {
			OnDropped(this.item);
		}

		if (itemSlotOther.GetItem() == null && itemSlotOther.OnRemoved != null) {
			itemSlotOther.OnRemoved();
		}
    }
}
