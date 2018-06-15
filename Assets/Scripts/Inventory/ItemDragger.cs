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

	void Start() {
		parentSlot = transform.parent.gameObject.GetComponent<ItemSlot>();
		image = gameObject.GetComponent<Image>();
	}
	
	void OnApplicationPause() {
		EndDrag();
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
		if (!CanDrag()) {
			return;
		}

        itemBeingDragged = this;
		parentSlot.Select();
		GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
		if (!CanDrag()) {
			return;
		}
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {	
		if (!parentSlot.OutputAllowed()) {
			return;
		}
        EndDrag();
    }
	
	private void EndDrag() {
		itemBeingDragged = null;
		transform.localPosition = Vector3.zero;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
	}

	public bool CanDrag() {
		return !parentSlot.IsEmpty() && parentSlot.OutputAllowed();
	}
}
