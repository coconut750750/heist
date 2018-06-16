using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour {
    private static List<Interactable> currentInteractables = new List<Interactable>();
    protected static ActionButton buttonA;
    protected static Player player;

    private UnityAction call = null;
    private new bool enabled = true;

	void Awake () {
        if (buttonA == null) {
            GameObject buttonObj = GameObject.Find(Constants.BUTTON_A_TAG);
            buttonA = buttonObj.GetComponent<ActionButton>();
            buttonA.Disable();
        }
        if (player == null) {
            player = GameObject.Find(Constants.PLAYER_TAG).GetComponent<Player>();
        }
	}

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled) {
            return;
        }
        if (other.gameObject.CompareTag(Constants.PLAYER_TAG))
        {
            if (!buttonA.IsInteractable()) {
                buttonA.Enable();
            }
            EnablePlayerInteract(player);
            EnterRange(player);
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (!enabled) {
            return;
        }
        if (other.gameObject.CompareTag(Constants.PLAYER_TAG))
        {
            DisablePlayerInteract(player);
            ExitRange(player);
            
            if (buttonA.IsInteractable() && buttonA.GetListeners() == 0) {
                buttonA.Disable();
            }
        }
    }

    private void EnablePlayerInteract(Player player) {
        call = delegate {
            Interact(player);
        };

        Interactable.buttonA.RemoveAllListeners();
        Interactable.buttonA.AddListener(call);

        currentInteractables.Add(this);
    }

    private void DisablePlayerInteract(Player player) {
        currentInteractables.Remove(this);
        Interactable.buttonA.RemoveAllListeners();
        if (currentInteractables.Count > 0) {
            Interactable next = currentInteractables[currentInteractables.Count - 1];
            Interactable.buttonA.AddListener(next.call);
        }
        
        call = null;
    }

    public void SetEnabled(bool enabled) {
        if (enabled) {
            Enable();
        } else {
            Disable();
        }
    }

    public virtual void Enable() {
        enabled = true;
    }

    public virtual void Disable() {
        enabled = false;
    }

    public abstract void EnterRange(Player player);

    public abstract void ExitRange(Player player);

    public abstract void Interact(Player player);
}
