using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exclaim : PopUp {

	private static Vector3 NAME_OFFSET = new Vector3(0, 0.75f, 0); // In game tile space, not pixel space
	private GameObject baseObject;

    public Exclaim() : base(NAME_OFFSET) {
    }

	void LateUpdate() {
    	UpdatePosition(baseObject.transform.position);
    }

	public void Display(GameObject baseObject, Transform parent) {
      this.baseObject = baseObject;
      Display(parent);
	  transform.SetSiblingIndex(0);
    }
}
