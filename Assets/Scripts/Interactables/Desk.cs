using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desk : Interactable {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void PlayerInteract() {
        Debug.Log("Player enter desk");
    }

    public override void PlayerLeave() {
        Debug.Log("Player leave desk");

    }
}
