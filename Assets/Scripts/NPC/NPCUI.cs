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

	void Awake() {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		gameObject.SetActive(false);

		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].SetIndex(i);
        }
		priceText.text = EMPTY_PRICE_TEXT;
	}
	
	public void Display(NPC npc) {
		GameManager.instance.PauseGame();

		gameObject.SetActive(true);

		npcInventory = npc.GetInventory();
		npcInventory.SetDisplaying(true);

		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].Refresh();
            itemSlots[i].InsertItem(npcInventory.GetItem(i), npcInventory);
			itemSlots[i].OnSelected += DisplayItemPrice;
			itemSlots[i].OnDeselected += HideItemPrice;
        }
	}

	public void Hide() {
		for (int i = 0; i < npcInventory.GetCapacity(); i++) {
            itemSlots[i].Reset();
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

	public void DisplayItemPrice(Item item) {
		priceText.text = item.price.ToString();
	}

	public void HideItemPrice(Item item) {
		priceText.text = EMPTY_PRICE_TEXT;
	}
}
