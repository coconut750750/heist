using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert : PopUp {

	protected static Canvas alertCanvas = null;

	private static Vector3 NAME_OFFSET = new Vector3(0, 1f, 0); // In game tile space, not pixel space
	private GameObject baseObject;

    public Alert() : base(NAME_OFFSET) {
    }

    protected override void Awake() {
        if (alertCanvas == null) {
			alertCanvas = GameObject.Find("AlertCanvas").GetComponent<Canvas>();
		}
    }

	void LateUpdate() {
    	UpdatePosition(baseObject.transform.position);
    }

    public override void Display() {
        transform.SetParent(alertCanvas.transform, false);
	}

	public void Display(GameObject baseObject) {
      this.baseObject = baseObject;
      Display();
	  transform.SetSiblingIndex(0);
    }

    public void Enable() {
        gameObject.SetActive(true);
    }

    public void Disable() {
        gameObject.SetActive(false);
    }
}
