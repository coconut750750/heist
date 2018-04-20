using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler {
	private Image itemImage;
	private Image itemBack;
	public Item item;

	public static Text text = null;

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

	void Awake() {
		Refresh();
	}

	public void Refresh() {
		if (text == null) {
			text = GameObject.Find(INVENTORY_ITEM_TEXT).GetComponent<Text>();
		}
		itemImage = transform.Find(ITEM_IMAGE).gameObject.GetComponent<Image>();
		itemBack = transform.Find(BACKGROUND_IMAGE).gameObject.GetComponent<Image>();
		Reset();
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
			Reset();
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
			PauseMenu.instance.GetActiveStash().DeselectAll();
		}
		if (parentStash != null) {
			parentStash.DeselectAll();
		}
		
		text.text = item.name;
		itemBack.color = SELECTED_COLOR;

		selected = true;
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
			text.text = "";
			itemBack.color = DEFAULT_COLOR;
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

	public void Reset() {
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
		} else {
			itemSlotOther.GetParentStash().RemoveItemAtIndex(indexOther);
			itemSlotOther.GetParentStash().AddItemAtIndex(tempItem1, indexOther);

			parentStash.RemoveItemAtIndex(index);
			parentStash.AddItemAtIndex(tempItem2, index);
		}
		Select();
    }
}
