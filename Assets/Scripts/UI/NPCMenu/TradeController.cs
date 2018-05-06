using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeController : MonoBehaviour {

	private Color YELLOW = Color.yellow;
	private Color GREEN = Color.green;
	private Color DARK_GREEN = new Color(0, 0.5f, 0);
	private Color RED = Color.red;

	[SerializeField]
	private Button tradeButton;

	[SerializeField]
	private TradingStash tradingStash;

	[SerializeField]
	private Slider tradingSlider;

	private Item selectedItem;
	private Item tradingItem;
	private bool willTrade;

	void Awake () {
		Disable();
		tradingItem = null;
		willTrade = false;

		tradingStash.OnAdded += TradeItemEntered;
		tradingStash.OnRemoved += TradeItemRemoved;
	}

	void Start() {
		UpdateButtons();
		UpdateTradingSlider();
	}

	public bool Trade(Inventory npcInventory) {
		if (selectedItem == null || tradingItem == null || !willTrade) {
			return false;
		}

		GameManager.instance.mainPlayer.AddItem(selectedItem);
		npcInventory.RemoveItem(selectedItem);
		npcInventory.AddItem(tradingItem);

		tradingStash.RemoveItem(tradingItem);
		selectedItem = null;
		tradingItem = null;
		willTrade = false;

		return true;
	}

	public void UpdateButtons() {
		Disable();

		if (!GameManager.instance.mainPlayer.CanAddItem()) {
			return;
		}

		if (selectedItem != null && tradingItem != null) {
			willTrade = tradingItem.GetValue() / selectedItem.GetValue() > NPC.LOWER_BOUND_TRADING_PERC;
			if (willTrade) {
				Enable();
			}
		}
	}

	public void UpdateTradingSlider() {
		ColorBlock cb = tradingSlider.colors;
		if (tradingItem == null || selectedItem == null) {
			tradingSlider.value = 0;
			cb.disabledColor = RED;
			tradingSlider.colors = cb;
			willTrade = false;
			return;
		}

		float itemValuePerc = tradingItem.GetValue() / selectedItem.GetValue();

		if (itemValuePerc < NPC.BOTTOM_END_TRADING_PERC) {
			tradingSlider.value = 0;
			cb.disabledColor = RED;
		} else if (itemValuePerc < NPC.LOWER_BOUND_TRADING_PERC) {
			tradingSlider.value = 1;
			cb.disabledColor = YELLOW;
		} else if (itemValuePerc < NPC.UPPER_BOUND_TRADING_PERC) {
			tradingSlider.value = 2;
			cb.disabledColor = GREEN;
		} else {
			tradingSlider.value = 3;
			cb.disabledColor = DARK_GREEN;
		} 
		tradingSlider.colors = cb;
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

	public void Enable() {
		tradeButton.interactable = true;
	}

	public void Disable() {
		tradeButton.interactable = false;
	}

	public Item GetSelectedItem() {
		return selectedItem;
	}

	public void SetSelectedItem(Item item) {
		selectedItem = item;
	}

	public void HideTradingStash() {
		tradingStash.Hide();
	}

	public void DeselectAll() {
		tradingStash.DeselectAll();
	}
}
