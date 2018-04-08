using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Desk : Interactable {

    private UnityAction call;

    private Inventory items;

	// Use this for initialization
	void Start () {
        items = gameObject.GetComponent<Inventory>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Interact(Player player) {
        Debug.Log(Interactable.player.GetName() + " interacted with " + gameObject.name + " " + Interactable.button.getListeners());

        GameManager.instance.DisplayInventory(items);
    }
}
