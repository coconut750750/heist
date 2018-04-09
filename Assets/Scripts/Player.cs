using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Player : MovingObject {

	private const string FORWARD = "PlayerForwardAnim";
	private const string LEFT = "PlayerLeftAnim";
	private const string BACK = "PlayerBackAnim";
	private const string RIGHT = "PlayerRightAnim";

	public GameObject floor1;
	public GameObject floor2;

	private Pocket mainItems;

	protected int money = 0;
	public GameObject moneyInfo;
	protected Text moneyText;
	protected int health = 0;
	protected int exp = 0;

	protected override void Start () {
		base.Start();
		mainItems = FindObjectOfType<Pocket>();

		moneyText = moneyInfo.GetComponent<Text>();
		UpdateInfo();
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
		if (GetFloor () == 1) {
			floor2.SetActive (false);
		} else if (GetFloor () == 2) {
			floor2.SetActive (true);
		}

		money += 1;
		UpdateInfo();
	}

	protected override void OnTriggerExit2D(Collider2D other) {
		base.OnTriggerExit2D (other);
	}

	public string GetName() {
		return "Player 1";
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

	private void UpdateInfo() {
		if (moneyText != null) {
			moneyText.text = "" + money;
		}
	}

    public override void Save() {
        PlayerData data = new PlayerData(base.onStairs, base.floor, base.rb2D.transform.position, money, health, exp);
		GameManager.Save(data, base.filename);
    }

    public override void Load() {
		PlayerData data = GameManager.Load<PlayerData>(base.filename);
        if (data != null) {
			base.LoadFromData(data);
			if (GetFloor () == 2) {
				floor2.SetActive (true);
				gameObject.layer = 17 - gameObject.layer;
			}
			this.money = data.money;
			this.health = data.health;
			this.exp = data.exp;
		}
    }
}

[System.Serializable]
public class PlayerData : MovingObjectData {
	public int money;
	public int health;
	public int exp;

	public PlayerData(int onStairs, int floor, Vector3 position, int money, int health, int exp) : 
		base(onStairs, floor, position) {
		this.money = money;
		this.health = health;
		this.exp = exp;
	}
}