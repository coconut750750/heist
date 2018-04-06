using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
    
	public Item item;
	public bool selected;
	public Image itemBack;

	public static Text text = null;
	private static Color SELECTED_COLOR = new Color(255/255f, 147/255f, 76/255f);
	private static Color DEFAULT_COLOR = new Color(1, 1, 1);
    
    public static GameObject itemBeingDragged = null;
	Vector3 startPosition;

	private const string BACKGROUND_IMAGE = "BackgroundImage";

	void Start() {
		Reset();
		if (text == null) {
			text = GameObject.Find("SelectedInventoryItemText").GetComponent<Text>();
		}
		itemBack = transform.parent.Find(BACKGROUND_IMAGE).gameObject.GetComponent<Image>();
	}

	public void SetItem(Item item) {
		this.item = item;
	}

	public void Reset() {
		this.item = null;
		this.selected = false;
	}

	public void SetSelected(bool selected) {
		this.selected = selected;
		if (item != null) {
			if (selected) {
				text.text = item.name;
				itemBack.color = SELECTED_COLOR;
			} else {
				text.text = "";
				itemBack.color = DEFAULT_COLOR;
			}
		}
	}

	public void OnPointerClick(PointerEventData eventData)
    {
		if (item == null) {
			return;
		}

        SetSelected(!selected);
    }

	public void OnBeginDrag(PointerEventData eventData)
    {
		if (item == null) {
			return;
		}
        itemBeingDragged = gameObject;
		startPosition = transform.position;
		GetComponent<CanvasGroup>().blocksRaycasts = false;		
    }

    public void OnDrag(PointerEventData eventData)
    {
		if (item == null) {
			return;
		}
        transform.position = Input.mousePosition;
		if (!selected) {
			SetSelected(true);
		}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
		transform.position = startPosition;
        itemBeingDragged = null;
		GetComponent<CanvasGroup>().blocksRaycasts = true;
		
    }
}
