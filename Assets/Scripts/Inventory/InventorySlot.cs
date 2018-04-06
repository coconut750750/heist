using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler {
	public Image itemImage;

	void Awake() {
		itemImage = transform.Find("ItemImage").gameObject.GetComponent<Image>();
	}

    public void OnDrop(PointerEventData eventData)
    {
		if (!itemImage.enabled && ItemDragger.itemBeingDragged != null) {
			Image imageDragged = ItemDragger.itemBeingDragged.gameObject.GetComponent<Image>();
			itemImage.sprite = imageDragged.sprite;
			itemImage.enabled = true;

			imageDragged.sprite = null;
			imageDragged.enabled = false;

			ItemDragger itemDragOld = imageDragged.GetComponent<ItemDragger>();
			ItemDragger itemDragNew = itemImage.GetComponent<ItemDragger>();

			itemDragNew.SetItem(itemDragOld.item);
			itemDragOld.SetSelected(false);
			itemDragNew.SetSelected(true);
			itemDragOld.Reset();
		}
    }
}
