using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour {

    public GameObject buttonObj;
    public Player player;
    protected ButtonA button;

    private const string PLAYER_TAG = "Player";

	// Use this for initialization
	void Awake () {
        button = buttonObj.GetComponent<ButtonA>();
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
