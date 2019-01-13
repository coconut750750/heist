using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonB : MonoBehaviour {

	[SerializeField]
	private Player player;

	protected ActionButton action;

	void Start () {
		action = GetComponent<ActionButton>();
		action.AddAction(delegate {
			player.Attack();
        });
	}

	public void Enable() {
		action.enabled = true;
	}

	public void Disable() {
		action.enabled = false;
	}
}
