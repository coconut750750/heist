using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.IO;
using UnityEngine.Events;

public class Player : MovingObject {

	private const string NPC_TAG = "NPC";

	// Button B Interaction
	protected static ActionButton buttonB;
    private const string BUTTON_B_TAG = "ButtonB";

	private Vector3 START_POS = new Vector3(0.5f, 0, 0);

	private Pocket mainItems;

	[SerializeField]
	private Text moneyText;
	
	[SerializeField]
	private Text healthText;

	[SerializeField]
	private Text expText;

	protected override void Start () {
		base.Start();
		mainItems = FindObjectOfType<Pocket>();

		GameObject buttonObj = GameObject.Find(BUTTON_B_TAG);
		buttonB = buttonObj.GetComponent<ActionButton>();
		buttonB.AddListener(delegate {
            Punch();
        });

		UpdateInfo();

		if (GetFloor() == 1) {
			GameManager.instance.HideFloor2();
		} else if (GetFloor() == 2) {
			GameManager.instance.ShowFloor2();
		}

		//GetComponent<Animator>().runtimeAnimatorController = CharAnimationManager.instance.GetNewAnimator();
	}

	protected override void FixedUpdate() {
		float moveHorizontal = 0;
		float moveVertical = 0;

		Vector3 movement;
		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		moveHorizontal = Input.GetAxis ("Horizontal");
		moveVertical = Input.GetAxis ("Vertical");
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		moveHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		moveVertical = CrossPlatformInputManager.GetAxis("Vertical");
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

		#endif

		Move(movement.normalized);
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
		if (GetFloor () == 1) {
			GameManager.instance.HideFloor2();
		} else if (GetFloor () == 2) {
			GameManager.instance.ShowFloor2();
		}

		money += 10;
		UpdateInfo(); 
	}

	protected override void OnTriggerExit2D(Collider2D other) {
		base.OnTriggerExit2D (other);
	}

	public string GetName() {
		return gameObject.name;
	}

	public Pocket GetPocket() {
		return mainItems;
	}
	
	public void AddItem(Item item) {
		mainItems.AddItem(item);
	}

	public Item RemoveItemAtIndex(int index) {
		if (index >= 0 || index < mainItems.GetNumItems()) {
			Item itemToRemove = mainItems.GetItem(index);
			mainItems.RemoveItem(itemToRemove);
			return itemToRemove;
		}
		return null;
	}

	public void RemoveItem(Item item) {
		mainItems.RemoveItem(item);
	}

	public int NumItems() {
		return mainItems.GetNumItems();
	}

	public bool CanAddItem() {
		return !mainItems.IsFull();
	}

	public void UpdateInfo() {
		if (moneyText != null) {
			moneyText.text = money.ToString();
		}

		if (healthText != null) {
			healthText.text = health.ToString();
		}

		if (expText != null) {
			expText.text = exp.ToString();
		}
	}

    public override void Save() {
        PlayerData data = new PlayerData(this);
		GameManager.Save(data, base.filename);
    }

    public override void Load() {
		PlayerData data = GameManager.Load<PlayerData>(base.filename);
		
        if (data != null) {
			base.LoadFromData(data);
			if (GetFloor () == 2) {
				GameManager.instance.ShowFloor2();
				gameObject.layer = 17 - gameObject.layer;
			}
			
		} else {
			base.LoadFromData(new PlayerData(
				0, 0, START_POS, 0, 0, 0, 0
			));
		}

		UpdateInfo();
    }
}

[System.Serializable]
public class PlayerData : MovingObjectData {

	public PlayerData(int onStairs, int floor, Vector3 position, int money, int health, int exp, int strength) {
		base.SetPositionalData(onStairs, floor, position);
		base.SetStats(money, health, exp, strength);
	}

	public PlayerData(Player player) : base(player) {

	}
}