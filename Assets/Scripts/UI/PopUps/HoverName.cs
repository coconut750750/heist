using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverName : MonoBehaviour {

    private Vector3 NAME_OFFSET = new Vector3(0, 0.75f, 0); // In game tile space, not pixel space

	public void Display(string name, Transform parent) {
		GetComponentInChildren<Text>().text = name;
        transform.SetParent(parent, false);
	}

	public void UpdatePosition(Vector3 rootPosition) {
		transform.position = Camera.main.WorldToScreenPoint(rootPosition + NAME_OFFSET);
	}

	public void Destroy() {
		Destroy(gameObject);
	}
}
