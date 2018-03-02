using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Interactable : MonoBehaviour {

    public GameObject buttonObj;
    protected Button button;

    private const string PLAYER_TAG = "Player";

	// Use this for initialization
	void Awake () {
        button = buttonObj.GetComponent<Button>();
	}

	// Update is called once per frame
	void Update () {
		
	}

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            PlayerInteract();
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(PLAYER_TAG))
        {
            PlayerLeave();
        }
    }

    public abstract void PlayerInteract();

    public abstract void PlayerLeave();
}
