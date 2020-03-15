using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InteractUtils {

	public static ActionButton GetButtonA() {
		GameObject buttonObj = GameObject.Find(Constants.BUTTON_A_TAG);
		return buttonObj.GetComponent<ActionButton>();
	}

	public static ButtonB GetButtonB() {
		GameObject buttonObj = GameObject.Find(Constants.BUTTON_B_TAG);
		return buttonObj.GetComponent<ButtonB>();
	}
}
