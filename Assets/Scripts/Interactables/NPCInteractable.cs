using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable {

	private NPC npcObject;

    // Use this for initialization
    void Start () {
		npcObject = gameObject.GetComponent<NPC>();
	}
	
	public override void Interact(Player player)
    {
        Debug.Log("wassup");

        GameManager.instance.npcDisplayer.Display(npcObject);
    }
}
