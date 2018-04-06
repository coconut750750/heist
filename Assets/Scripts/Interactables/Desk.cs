using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Desk : Interactable {

    private UnityAction call;

    [SerializeField]
    private List<Item> items;

	// Use this for initialization
	void Start () {
        Debug.Log("player restart");
        		
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
        GameManager.mainPlayer.GetInventory().Log();
        if (items.Count > 0) {
            Item item = items[0];
            player.AddItem(item);
            items.Remove(item);
        }
        GameManager.mainPlayer.GetInventory().Log();
    }
}
