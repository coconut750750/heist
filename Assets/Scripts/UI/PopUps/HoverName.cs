using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverName : PopUp {

    private static Vector3 NAME_OFFSET = new Vector3(0, 0.75f, 0); // In game tile space, not pixel space

    public HoverName() : base(NAME_OFFSET) {
    }

    public void Display(string name, Transform parent) {
		Display(parent);
		GetComponentInChildren<Text>().text = name;
	}
}
