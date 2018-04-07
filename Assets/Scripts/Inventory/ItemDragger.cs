using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
    
	private ItemSlot parentSlot;
	private Image image;
    
    public static ItemDragger itemBeingDragged = null;
	Vector3 startPosition;

	void Start() {
		parentSlot = transform.parent.gameObject.GetComponent<ItemSlot>();
		image = gameObject.GetComponent<Image>();
	}
	
	public Image GetImage() {
		return image;
	}

	public ItemSlot GetParentSlot() {
		return parentSlot;
	}

	public void OnPointerClick(PointerEventData eventData)
    {
		if (parentSlot.IsEmpty()) {
			return;
		}
		
        parentSlot.ToggleSelect();
    }

	public void OnBeginDrag(PointerEventData eventData)
    {
		if (parentSlot.IsEmpty()) {
			return;
		}

        itemBeingDragged = this;
		startPosition = transform.position;
		parentSlot.Select();
		GetComponent<CanvasGroup>().blocksRaycasts = false;		
    }

    public void OnDrag(PointerEventData eventData)
    {
		if (parentSlot.IsEmpty()) {
			return;
		}
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
		transform.position = startPosition;
        itemBeingDragged = null;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		
    }
}
