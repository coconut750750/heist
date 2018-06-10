using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour {

	private Vector3 offset;
	protected static Canvas popupCanvas = null;

	void Awake() {
		if (popupCanvas == null) {
			popupCanvas = GameObject.Find("PopupCanvas").GetComponent<Canvas>();
		}
	}

	public PopUp(Vector3 offset) {
		this.offset = offset;
	}

	public virtual void Display() {
        transform.SetParent(popupCanvas.transform, false);
	}

	public virtual void UpdatePosition(Vector3 rootPosition) {
		transform.position = Camera.main.WorldToScreenPoint(rootPosition + offset);
	}

	public virtual void Destroy() {
		Destroy(gameObject);
	}
}
