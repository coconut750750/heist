using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverName : PopUp {

    private static Vector3 NAME_OFFSET = new Vector3(0, 0.75f, 0); // In game tile space, not pixel space
    private GameObject baseObject;

    public HoverName() : base(NAME_OFFSET) {
    }

    void LateUpdate() {
      UpdatePosition(baseObject.transform.position);
    }

    public void Display(string name, GameObject baseObject, Transform parent) {
      this.baseObject = baseObject;
      Display(parent);
      GetComponentInChildren<Text>().text = name;
    }
}
