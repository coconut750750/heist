using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour {

    protected static GameObject buttonObj;
    protected static ButtonA button;
    private const string BUTTON_A_TAG = "ButtonA";

    protected static Player player;
    private const string PLAYER_TAG = "Player";

	// Use this for initialization
	void Awake () {
        if (buttonObj == null) {
            buttonObj = GameObject.Find(BUTTON_A_TAG);
            button = buttonObj.GetComponent<ButtonA>();
            buttonObj.SetActive(false);
        }
        if (player == null) {
            player = GameObject.Find(PLAYER_TAG).GetComponent<Player>();
        }
	}

	// Update is called once per frame
	void Update () {
		
	}

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            if (!buttonObj.activeSelf) {
                buttonObj.SetActive(true);
            }
            PlayerInteract(player);
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            PlayerLeave(player);
            if (buttonObj.activeSelf && button.getListeners() == 0) {
                buttonObj.SetActive(false);
            }
        }
    }

    public abstract void PlayerInteract(Player player);

    public abstract void PlayerLeave(Player player);
}
