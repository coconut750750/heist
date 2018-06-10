using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.IO;
using UnityEngine.Events;

public class Player : Character {

	private const float PUNCH_DELAY_SECONDS = 0.5f;

	// Button B Interaction
	protected static ActionButton buttonB;

	public static Vector3 START_POS = new Vector3(0.5f, 0, 0);

	private Pocket mainItems;

	[SerializeField]
	private Text moneyText;
	
	[SerializeField]
	private Text healthText;

	[SerializeField]
	private Text expText;

	private bool canPunch = true;
	IEnumerator punchDelay() {
		canPunch = false;
		yield return new WaitForSeconds(PUNCH_DELAY_SECONDS);
		canPunch = true;
	}

	protected override void Start () {
		base.Start();
		mainItems = FindObjectOfType<Pocket>();

		GameObject buttonObj = GameObject.Find(Constants.BUTTON_B_TAG);
		buttonB = buttonObj.GetComponent<ActionButton>();
		buttonB.AddListener(delegate {
            Punch();
        });

		UpdateUIInfo();
		UpdateVisibleFloorWithGameManager();
	}

	protected override void FixedUpdate() {
		float moveHorizontal = 0;
		float moveVertical = 0;

		Vector3 movement;
		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		moveHorizontal = Input.GetAxis (Constants.HORIZONTAL);
		moveVertical = Input.GetAxis (Constants.VERTICAL);
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		moveHorizontal = CrossPlatformInputManager.GetAxis(Constants.HORIZONTAL);
		moveVertical = CrossPlatformInputManager.GetAxis(Constants.VERTICAL);
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

		#endif

		Move(movement.normalized);
	}

	/// INTERACTABLES ///

	public void DisableButtonB() {
		buttonB.enabled = false;
	}

	public void EnableButtonB() {
		buttonB.enabled = true;
	}

	public void Punch() {
		if (!canPunch) {
			return;
		}

		base.Punch(Constants.NPC_ONLY_LAYER);
		StartCoroutine(punchDelay());
	}

	public override void GetHitBy(Character other) {
		base.GetHitBy(other);
		UpdateUIInfo();
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
		if (other.gameObject.CompareTag(Constants.STAIRS_TAG)) {
			UpdateVisibleFloorWithGameManager();
		} else if (other.gameObject.CompareTag(Constants.NPC_TAG)) {
			if (!other.gameObject.GetComponent<NPC>().visibleByCamera) {
				return;
			}
			Debug.Log("herlloo");
		}

		// TODO: remove
		money += 10;
		UpdateUIInfo(); 
	}

	protected override void OnTriggerExit2D(Collider2D other) {
		base.OnTriggerExit2D (other);
	}

	private void UpdateVisibleFloorWithGameManager() {
		if (GetFloor() == 1) {
			GameManager.instance.HideFloor2();
		} else if (GetFloor() == 2) {
			GameManager.instance.ShowFloor2();
		}
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

	public void UpdateUIInfo() {
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
		} else {
			base.LoadFromData(new PlayerData());
		}

		UpdateUIInfo();
    }
}

[System.Serializable]
public class PlayerData : CharacterData {

	public PlayerData() {
		base.SetPositionalData(false, Player.START_POS);
		base.SetStats(0, 0, 0, 0);
	}

	public PlayerData(Player player) : base(player) {

	}
}