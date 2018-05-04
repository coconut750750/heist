using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI : MonoBehaviour {

	private const string EMPTY_PRICE_TEXT = "---";

	public static NPCUI instance = null;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text healthText;
	[SerializeField]
	private Text expText;
	[SerializeField]
	private Text strengthText;

	[SerializeField]
	private Text priceText;

	[SerializeField]
	private ItemSlot tradingSlot;
	
	private Inventory npcInventory;

	[SerializeField]
	private ItemSlot[] itemSlots;

	private Item selectedItem;

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		gameObject.SetActive(false);

		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].SetIndex(i);
			itemSlots[i].SetInputAllowed(false);
			itemSlots[i].SetOutputAllowed(false);
			itemSlots[i].OnSelected += OnSelectedItem;
			itemSlots[i].OnDeselected += OnDeselectedItem;
        }
		priceText.text = EMPTY_PRICE_TEXT;
		selectedItem = null;
	}
	
	public void Display(NPC npc) {
		GameManager.instance.PauseGame();

		gameObject.SetActive(true);

		npcInventory = npc.GetInventory();
		npcInventory.SetDisplaying(true);

		UpdateInventoryUI();
	}

	public void Hide() {
		for (int i = 0; i < npcInventory.GetCapacity(); i++) {
            itemSlots[i].ResetItem();
        }

		npcInventory.SetDisplaying(false);
		npcInventory = null;

		gameObject.SetActive(false);

		GameManager.instance.UnpauseGame();
	}

	public void DeselectAll() {
		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].Deselect();
        }
	}

	public Inventory GetNPCInventory() {
		return npcInventory;
	}

	public void OnSelectedItem(Item item) {
		priceText.text = item.price.ToString();
		selectedItem = item;
	}

	public void OnDeselectedItem(Item item) {
		priceText.text = EMPTY_PRICE_TEXT;
		selectedItem = null;
	}

	public void OnClickBuy() {
		if (selectedItem == null) {
			return;
		}
		int playerMoney = GameManager.instance.mainPlayer.GetMoney();
		int price = selectedItem.price;

		if (playerMoney >= price) {
			GameManager.instance.mainPlayer.SetMoney(playerMoney - price);
			GameManager.instance.mainPlayer.AddItem(selectedItem);
			npcInventory.RemoveItem(selectedItem);

			UpdateInventoryUI();
		}
	}

	private void UpdateInventoryUI() {
		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].Refresh();
            itemSlots[i].InsertItem(npcInventory.GetItem(i), npcInventory);
        }
	}
}
