using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCUI : MonoBehaviour {

	[SerializeField]
	private Text nameText;

	[SerializeField]
	private Text healthText;
	[SerializeField]
	private Text expText;
	[SerializeField]
	private Text strengthText;
	
	private Inventory npcInventory;

	[SerializeField]
	private ItemSlot[] itemSlots;

	void Start () {
		gameObject.SetActive(false);
	}
	
	public void Display(NPC npc) {
		GameManager.instance.PauseGame();

		gameObject.SetActive(true);

		npcInventory = npc.GetInventory();
		npcInventory.SetDisplaying(true);

		for (int i = 0; i < itemSlots.Length; i++) {
            itemSlots[i].Refresh();
            itemSlots[i].InsertItem(npcInventory.GetItem(i), npcInventory);
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
}
