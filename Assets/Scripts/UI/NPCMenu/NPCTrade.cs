using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NPCTrade : MonoBehaviour {

	public static NPCTrade instance = null;

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text moneyText;

	[SerializeField]
	public BuyController buyController;

	[SerializeField]
	public TradeController tradeController;

	[SerializeField]
	public SellController sellController;
	
	private NPC npc;
	private Inventory npcInventory;

	[SerializeField]
	private ItemSlot[] itemSlots;

	public ItemSlot[] ItemSlots { get { return itemSlots; } private set {} }

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
		gameObject.SetActive(true);

		this.npc = npc;
		npcInventory = npc.GetInventory();

		nameText.text = npc.GetName();

		UpdateInventoryUI();
	}

	// invoked by Canvases > NPCAndQuestCanvas > NPCTrade > TradeCover > Event Trigger 
	public void Hide() {
		for (int i = 0; i < npcInventory.GetCapacity(); i++) {
            itemSlots[i].ClearItem();
        }

		npcInventory = null;
		npc = null;

		HideStashes();

		gameObject.SetActive(false);
		ResetAllControllers();
	}

	#if UNITY_EDITOR || UNITY_STANDALONE
	protected void OnApplicationQuit() {
		HideStashes();
	}
	#elif UNITY_ANDROID || UNITY_IOS
	protected void OnApplicationPause() {
		HideStashes();
	}
	#endif

	private void HideStashes() {
		tradeController.HideTradingStash();
		sellController.HideSellingStash();
	}

	private void ResetAllControllers() {
		tradeController.Reset();
		sellController.Reset();
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

	public void UpdateButtons() {
		int externalItems = new bool[] { tradeController.ContainsItem(), sellController.ContainsItem() }.Count(t => t);
		buyController.UpdateButtons(externalItems);
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
