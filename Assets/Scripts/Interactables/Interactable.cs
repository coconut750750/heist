using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour {
    public static ActionButton buttonA;
    public static ButtonB buttonB;

    protected static Player player;

    private UnityAction call = null;
    private new bool enabled = true;

	void Awake () {
        if (buttonA == null) {
            GameObject buttonObj = GameObject.Find(Constants.BUTTON_A_TAG);
            buttonA = buttonObj.GetComponent<ActionButton>();
            buttonA.Disable();
        }

        if (buttonB == null) {
            GameObject buttonObj = GameObject.Find(Constants.BUTTON_B_TAG);
            buttonB = buttonObj.GetComponent<ButtonB>();
        }

        if (player == null) {
            player = GameObject.Find(Constants.PLAYER_TAG).GetComponent<Player>();
        }

        call = delegate {
            Interact(player);
        };
	}

    protected void OnTriggerEnter2D(Collider2D other) {
        if (!enabled) {
            return;
        }
        if (other.gameObject.CompareTag(Constants.PLAYER_TAG)) {
            Interactable.buttonA.AddAction(call);
            EnterRange(player);
        }
    }

    protected void OnTriggerExit2D(Collider2D other) {
        if (!enabled) {
            return;
        }
        if (other.gameObject.CompareTag(Constants.PLAYER_TAG)) {
            Interactable.buttonA.RemoveAction(call);
            ExitRange(player);
        }
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
