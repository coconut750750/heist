using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI : MonoBehaviour {

	public const float LOWER_BOUND_TRADING_PERC = 1.00f;
	public const float UPPER_BOUND_TRADING_PERC = 1.25f;

	public Color RED = Color.red;
	public Color YELLOW = Color.yellow;
	public Color GREEN = Color.green;
	public Color WHITE = Color.white;

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
	private TradingStash tradingStash;

	[SerializeField]
	private Slider tradingSlider;
	
	private Inventory npcInventory;

	[SerializeField]
	private ItemSlot[] itemSlots;

	private Item selectedItem = null;
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
		priceText.text = EMPTY_PRICE_TEXT;

		tradingStash.OnAdded += TradeItemEntered;
		tradingStash.OnRemoved += TradeItemRemoved;
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

		tradingStash.Hide();

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
		tradingStash.DeselectAll();
	}

	public Inventory GetNPCInventory() {
		return npcInventory;
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

	public void OnClickTrade() {
		if (selectedItem == null || tradingItem == null || !willTrade) {
			return;
		}

		GameManager.instance.mainPlayer.AddItem(selectedItem);
		npcInventory.RemoveItem(selectedItem);
		npcInventory.AddItem(tradingItem);

		tradingStash.RemoveItem(tradingItem);
		selectedItem = null;
		tradingItem = null;
		willTrade = false;

		UpdateInventoryUI();
	}

	private void UpdateInventoryUI() {
		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].Refresh();
            itemSlots[i].InsertItem(npcInventory.GetItem(i), npcInventory);
        }
	}

	private void OnSelectedItem(Item item) {
		priceText.text = item.price.ToString();
		selectedItem = item;

		UpdateTradingSlider();
	}

	private void OnDeselectedItem(Item item) {
		priceText.text = EMPTY_PRICE_TEXT;
		selectedItem = null;

		tradingSlider.value = 0;
	}

	private void TradeItemEntered(Item item) {
		tradingItem = item;
		UpdateTradingSlider();
	}

	private void TradeItemRemoved() {
		tradingItem = null;

		tradingSlider.value = 0;
		willTrade = false;
	}

	private void UpdateTradingSlider() {
		if (tradingItem == null || selectedItem == null) {
			tradingSlider.value = 0;
			tradingSlider.targetGraphic.color = WHITE;
			return;
		}

		float itemValuePerc = tradingItem.GetValue() / selectedItem.GetValue();

		willTrade = false;
		ColorBlock cb = tradingSlider.colors;
		if (itemValuePerc < LOWER_BOUND_TRADING_PERC) {
			tradingSlider.value = 1;
			cb.disabledColor = RED;
		} else if (itemValuePerc > UPPER_BOUND_TRADING_PERC) {
			tradingSlider.value = 3;
			cb.disabledColor = GREEN;
			willTrade = true;
		} else {
			tradingSlider.value = 2;
			cb.disabledColor = YELLOW;
			cb.normalColor = YELLOW;

			Debug.Log(YELLOW);
		}
		tradingSlider.colors = cb;
	}
}
