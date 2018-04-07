using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Desk : Interactable {

    private UnityAction call;

    private Inventory items;

    public Item[] itemsToAdd = new Item[4];

	// Use this for initialization
	void Start () {
        Debug.Log("player restart");
        items = gameObject.GetComponent<Inventory>();
        for (int i = 0; i < 4; i++) {
            items.AddItem(itemsToAdd[i]);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void PlayerInteract(Player player) {      
        call = delegate {
            Interact(player);
        };
        Interactable.button.RemoveAllListeners();
        Interactable.button.AddListener(call);
    }

    public override void PlayerLeave(Player player) {
        Interactable.button.RemoveListener(call);
        call = null;
    }

    public void Interact(Player player) {
        Debug.Log(player.GetName() + " interacted with " + gameObject.name + " " + Interactable.button.getListeners());
        items.Log();

        StashDisplayer.SetInventory(items);
        GameManager.stashDisplayer.gameObject.SetActive(true);
        // if (items.GetNumItems() > 0) {
        //     Item item = items.GetItem(0);
        //     player.AddItem(item);
        //     items.RemoveItem(item);
        // }
    }
}
