using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ActionButton : Button {

	private List<UnityAction> currentActions = new List<UnityAction>();

	public void AddAction(UnityAction call) {
        if (!IsInteractable()) {
            Enable();
        }
        RemoveAllListeners();

        base.onClick.AddListener(call);
        currentActions.Add(call);
    }

    public void RemoveAction(UnityAction call) {
        currentActions.Remove(call);
        RemoveAllListeners();
        if (GetNumActions() > 0) {
            UnityAction next = currentActions[currentActions.Count - 1];
            base.onClick.AddListener(next);
        } else if (IsInteractable()) {
            Disable();
        }
    }

	private void RemoveAllListeners() {
		base.onClick.RemoveAllListeners();
	}

	public int GetNumActions() {
		return currentActions.Count;
	}

	public void Enable() {
		interactable = true;
	}

	public void Disable() {
		interactable = false;
	}
}
