using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour {

    protected static bool alreadyRemoved;

    protected static ButtonA buttonA;
    private const string BUTTON_A_TAG = "ButtonA";

    protected static Player player;
    private const string PLAYER_TAG = "Player";

    private UnityAction call = null;

	// Use this for initialization
	void Awake () {
        if (buttonA == null) {
            GameObject buttonObj = GameObject.Find(BUTTON_A_TAG);
            buttonA = buttonObj.GetComponent<ButtonA>();
            buttonA.Disable();
        }
        if (player == null) {
            player = GameObject.Find(PLAYER_TAG).GetComponent<Player>();
        }
        alreadyRemoved = false;
	}

	// Update is called once per frame
	void Update () {
		
	}

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            if (!buttonA.IsInteractable()) {
                buttonA.Enable();
            }
            PlayerInteract(player);
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            PlayerLeave(player);
            
            if (buttonA.IsInteractable() && buttonA.GetListeners() == 0) {
                buttonA.Disable();
            }
        }
    }

    public void PlayerInteract(Player player) {
        call = delegate {
            Interact(player);
        };

        if (buttonA.GetListeners() > 0) {
            alreadyRemoved = true;
            Interactable.buttonA.RemoveAllListeners();
        }
        
        Interactable.buttonA.AddListener(call);
    }

    public void PlayerLeave(Player player) {
        if (!alreadyRemoved) {
            Interactable.buttonA.RemoveListener(call);
        } else {
            alreadyRemoved = false;
        }
        
        call = null;
    }

    public abstract void Interact(Player player);
}
