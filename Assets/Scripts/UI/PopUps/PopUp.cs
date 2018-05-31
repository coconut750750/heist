using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUp : MonoBehaviour {

	private Vector3 offset;

	public PopUp(Vector3 offset) {
		this.offset = offset;
	}

	public virtual void Display(Transform parent) {
        transform.SetParent(parent, false);
	}

	public virtual void UpdatePosition(Vector3 rootPosition) {
		transform.position = Camera.main.WorldToScreenPoint(rootPosition + offset);
	}

	public virtual void Destroy() {
		Destroy(gameObject);
	}
}
