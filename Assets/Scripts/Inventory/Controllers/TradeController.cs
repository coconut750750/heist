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
	private int selectedIndex;
	private Item tradingItem;
	private bool willTrade;

	void Awake () {
		Reset();

		tradingStash.OnAdded += TradeItemEntered;
		tradingStash.OnRemoved += TradeItemRemoved;
	}

	public void Reset() {
		Disable();
		tradingItem = null;
		willTrade = false;

		selectedItem = null;
		selectedIndex = -1;
	}

	public bool Trade(NPC npc) {
		if (selectedItem == null || tradingItem == null || !willTrade) {
			return false;
		}

		GameManager.instance.mainPlayer.AddItem(selectedItem);
		selectedItem.ChangedHands();
		npc.GetInventory().RemoveItemAtIndex(selectedIndex);

		npc.GetInventory().AddItem(tradingItem);
		tradingItem.ChangedHands();

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

	public void SetSelectedItem(Item item, int index) {
		selectedItem = item;
		selectedIndex = index;
	}

	public bool IsEmpty() {
		return tradingItem == null;
	}

	public void HideTradingStash() {
		tradingStash.Hide();
	}

	public void DeselectAll() {
		tradingStash.DeselectAll();
	}
}
