using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI : MonoBehaviour {

	public const float LOWER_BOUND_TRADING_PERC = 1.15f;
	public const float UPPER_BOUND_TRADING_PERC = 1.30f;

	private Color YELLOW = Color.yellow;
	private Color GREEN = Color.green;
	private Color DARK_GREEN = new Color(0, 0.5f, 0);
	private Color WHITE = Color.white;

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
	private Button buyButton;

	[SerializeField]
	private Button tradeButton;

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

		buyButton.interactable = false;
		tradeButton.interactable = false;

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

		UpdateButtons();
		UpdateTradingSlider();
	}

	private void OnDeselectedItem(Item item) {
		priceText.text = EMPTY_PRICE_TEXT;
		selectedItem = null;

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

		Debug.Log(GameManager.instance.mainPlayer.GetPocket().ToString());
		UpdateButtons();
		UpdateTradingSlider();
	}

	private void UpdateButtons() {
		buyButton.interactable = false;
		tradeButton.interactable = false;

		if (!GameManager.instance.mainPlayer.CanAddItem()) {
			return;
		}

		if (selectedItem != null) {
			if (GameManager.instance.mainPlayer.GetMoney() >= selectedItem.price) {
				buyButton.interactable = true;
			}

			if (tradingItem != null) {
				willTrade = tradingItem.GetValue() / selectedItem.GetValue() > LOWER_BOUND_TRADING_PERC;
				if (willTrade) {
					tradeButton.interactable = true;
				}
			}
		}
	}

	private void UpdateTradingSlider() {
		if (tradingItem == null || selectedItem == null) {
			tradingSlider.value = 0;
			tradingSlider.targetGraphic.color = WHITE;
			willTrade = false;
			return;
		}

		float itemValuePerc = tradingItem.GetValue() / selectedItem.GetValue();

		ColorBlock cb = tradingSlider.colors;
		if (itemValuePerc < LOWER_BOUND_TRADING_PERC) {
			tradingSlider.value = 1;
			cb.disabledColor = YELLOW;
		} else if (itemValuePerc >= UPPER_BOUND_TRADING_PERC) {
			tradingSlider.value = 3;
			cb.disabledColor = DARK_GREEN;
		} else {
			tradingSlider.value = 2;
			cb.disabledColor = GREEN;
		}
		tradingSlider.colors = cb;
	}
}
