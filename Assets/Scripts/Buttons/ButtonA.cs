using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ButtonA : Button {

	private int listeners = 0;

	public void AddListener(UnityAction call) {
		base.onClick.AddListener(call);
		listeners++;
	}

	public void RemoveListener(UnityAction call) {
		base.onClick.RemoveListener(call);
		listeners--;
	}

	public void RemoveAllListeners() {
		base.onClick.RemoveAllListeners();
		listeners = 0;
	}

	public int GetListeners() {
		return listeners;
	}
}
