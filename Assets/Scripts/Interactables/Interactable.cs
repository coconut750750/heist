using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour {

    private static Stack<Interactable> currentInteractables;

    protected static ActionButton buttonA;
    private const string BUTTON_A_TAG = "ButtonA";

    protected static Player player;
    private const string PLAYER_TAG = "Player";

    private UnityAction call = null;

	// Use this for initialization
	void Awake () {
        if (buttonA == null) {
            GameObject buttonObj = GameObject.Find(BUTTON_A_TAG);
            buttonA = buttonObj.GetComponent<ActionButton>();
            buttonA.Disable();
        }
        if (player == null) {
            player = GameObject.Find(PLAYER_TAG).GetComponent<Player>();
        }

        if (currentInteractables == null) {
            currentInteractables = new Stack<Interactable>();
        }
	}

	// Update is called once per frame
	void Update () {
		
	}

    // when another "thing" enters the trigger area
    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            if (!buttonA.IsInteractable()) {
                buttonA.Enable();
            }
            PlayerInteract(player);
            EnterRange(player);
        }
    }

    // when "thing" exits the trigger area
    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            PlayerLeave(player);
            ExitRange(player);
            
            if (buttonA.IsInteractable() && buttonA.GetListeners() == 0) {
                buttonA.Disable();
            }
        }
    }

    // enable the button to be interactable with the new method
    public void PlayerInteract(Player player) {
        call = delegate {
            Interact(player);
        };

        Interactable.buttonA.RemoveAllListeners();
        Interactable.buttonA.AddListener(call);

        currentInteractables.Push(this);
    }

    // resets the button so it is cleared the next time "thing" enters
    public void PlayerLeave(Player player) {
        if (currentInteractables.Pop() == this) {
            Interactable.buttonA.RemoveAllListeners();

            if (currentInteractables.Count > 0) {
                Interactable next = currentInteractables.Peek();
                Interactable.buttonA.AddListener(next.call);
            }
        }
        
        call = null;
    }

    // called when player is in trigger area
    public abstract void EnterRange(Player player);

    // called when player leaves trigger area
    public abstract void ExitRange(Player player);

    // called when button is pressed
    public abstract void Interact(Player player);
}
