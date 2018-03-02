using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Desk : Interactable {

    private UnityAction call;

	// Use this for initialization
	void Start () {
        Debug.Log("player restart");
        call = delegate {
            Interact();
        };
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void PlayerInteract() {
        base.button.onClick.AddListener(call);

    }

    public override void PlayerLeave() {
        Debug.Log("Player leave desk");
        base.button.onClick.RemoveListener(call);

    }

    public void Interact() {
        Debug.Log("Player interacted with desk");
    }
}
