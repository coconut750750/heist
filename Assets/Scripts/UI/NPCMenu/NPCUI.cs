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
	private Text moneyText;

	[SerializeField]
	private BuyController buyController;

	[SerializeField]
	private TradeController tradeController;

	[SerializeField]
	private SellController sellController;
	
	private NPC npc;
	private Inventory npcInventory;

	[SerializeField]
	private ItemSlot[] itemSlots;

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

		this.npc = npc;
		npcInventory = npc.GetInventory();
		npcInventory.SetDisplaying(true);

		nameText.text = npc.GetName();

		healthText.text = npc.GetHealth().ToString();
		strengthText.text = npc.GetStrength().ToString();
		expText.text = npc.GetExperience().ToString();

		UpdateInventoryUI();
	}

	public void Hide() {
		for (int i = 0; i < npcInventory.GetCapacity(); i++) {
            itemSlots[i].ClearItem();
        }

		npcInventory.SetDisplaying(false);
		npcInventory = null;
		npc = null;

		tradeController.HideTradingStash();
		sellController.HideSellingStash();

		gameObject.SetActive(false);
		GameManager.instance.UnpauseGame();
	}

	public void DeselectAll() {
		tradeController.DeselectAll();
		sellController.DeselectAll();
		
		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].Deselect();
        }
	}

	public Inventory GetNPCInventory() {
		return npcInventory;
	}

	public void OnClickBuy() {
		if (buyController.Buy(npc)) {
			UpdateInventoryUI();
			GameManager.instance.mainPlayer.UpdateUIInfo();
		}
	}

	public void OnClickTrade() {
		if (tradeController.Trade(npc)) {
			SetSelectedItem(null, -1);
			UpdateInventoryUI();
			GameManager.instance.mainPlayer.UpdateUIInfo();
		}
	}

	public void OnClickSell() {
		if (sellController.Sell(npc)) {
			sellController.UpdateButtons();
			UpdateInventoryUI();
			GameManager.instance.mainPlayer.UpdateUIInfo();
		}
	}

	private void UpdateInventoryUI() {
		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].ClearItem();
            itemSlots[i].SetItem(npcInventory.GetItem(i), npcInventory);
        }

		moneyText.text = npc.GetMoney().ToString();
	}

	private void OnSelectedItem(Item item, int index) {
		SetSelectedItem(item, index);

		UpdateButtons();
		UpdateTradingSlider();
	}

	private void OnDeselectedItem() {
		SetSelectedItem(null, -1);

		UpdateButtons();
		UpdateTradingSlider();
	}

	private void UpdateButtons() {
		buyController.UpdateButtons(!tradeController.IsEmpty());
		tradeController.UpdateButtons();
		sellController.UpdateButtons();
	}

	private void UpdateTradingSlider() {
		tradeController.UpdateTradingSlider();
	}

	private void SetSelectedItem(Item item, int index) {
		buyController.SetSelectedItem(item, index);
		tradeController.SetSelectedItem(item, index);
	}

	public NPC GetNPC() {
		return npc;
	}
}
