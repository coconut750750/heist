using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInfo : MonoBehaviour {

	public static NPCInfo instance = null;

	private Color YELLOW = Color.yellow;
	private Color GREEN = Color.green;
	private Color RED = Color.red;

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
	private Slider friendlinessSlider;

	private NPC npc;

	void Awake () {
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		gameObject.SetActive(false);
	}
	
	public void Display(NPC npc) {
		gameObject.SetActive(true);

		this.npc = npc;

		nameText.text = npc.GetName();

		healthText.text = npc.GetHealth().ToString();
		strengthText.text = npc.GetStrength().ToString();
		expText.text = npc.GetExperience().ToString();

		SetFriendlinessSlider(npc.GetFriendliness());
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	private void SetFriendlinessSlider(int friendliness) {
		friendlinessSlider.value = friendliness;

		ColorBlock cb = friendlinessSlider.colors;
		if (friendliness > 66) {
			cb.disabledColor = GREEN;
		} else if (friendliness > 33) {
			cb.disabledColor = YELLOW;
		} else {
			cb.disabledColor = RED;
		}

		friendlinessSlider.colors = cb;
	}
}
