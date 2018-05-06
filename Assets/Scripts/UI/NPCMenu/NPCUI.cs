using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI : MonoBehaviour {

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
	private BuyController buyController;

	[SerializeField]
	private TradeController tradeController;
	
	private Inventory npcInventory;

	[SerializeField]
	private ItemSlot[] itemSlots;

	private Item tradingItem = null;

	private bool willTrade = false;

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

		tradeController.HideTradingStash();

		gameObject.SetActive(false);
		GameManager.instance.UnpauseGame();
	}

	public void DeselectAllItemSlots() {
		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].Deselect();
        }
	}

	// Doesn't deselect item slots because player might be inputing a trading item so,
	// don't want to clear price
	public void DeselectAll() {
		tradeController.DeselectAll();
	}

	public Inventory GetNPCInventory() {
		return npcInventory;
	}

	public void OnClickBuy() {
		if (buyController.Buy(npcInventory)) {
			UpdateInventoryUI();
		}
	}

	public void OnClickTrade() {
		if (tradeController.Trade(npcInventory)) {
			SetSelectedItem(null);
			UpdateInventoryUI();
		}
	}

	private void UpdateInventoryUI() {
		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].Refresh();
            itemSlots[i].InsertItem(npcInventory.GetItem(i), npcInventory);
        }
	}

	private void OnSelectedItem(Item item) {
		buyController.SetPriceText(item.price);
		SetSelectedItem(item);

		UpdateButtons();
		UpdateTradingSlider();
	}

	private void OnDeselectedItem(Item item) {
		buyController.ResetPriceText();
		SetSelectedItem(null);

		UpdateButtons();
		UpdateTradingSlider();
	}

	private void TradeItemEntered(Item item) {
		tradingItem = item;

		UpdateButtons();
		UpdateTradingSlider();
	}

	private void TradeItemRemoved() {
		tradingItem = null;

		UpdateButtons();
		UpdateTradingSlider();
	}

	private void UpdateButtons() {
		buyController.UpdateButtons(tradingItem != null);
		tradeController.UpdateButtons();
	}

	private void UpdateTradingSlider() {
		tradeController.UpdateTradingSlider();
	}

	private void SetSelectedItem(Item item) {
		buyController.SetSelectedItem(item);
		tradeController.SetSelectedItem(item);
	}
}
