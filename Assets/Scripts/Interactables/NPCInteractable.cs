using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractable : Interactable {

    public GameObject hoverNameText;
    private HoverName hoverTextInstance = null;

    public GameObject speechBubble;
    private SpeechBubble speechBubbleInstance = null;

	private NPC npcObject;

    // Use this for initialization
    void Start () {
		npcObject = gameObject.GetComponent<NPC>();
	}

    void Update () {
        if (hoverTextInstance != null) {
            hoverTextInstance.UpdatePosition(gameObject.transform.position);
        }
        if (speechBubbleInstance != null) {
            speechBubbleInstance.UpdatePosition(gameObject.transform.position);
        }
    }
	
	public override void Interact(Player player) {
        // open inventory
        // GameManager.instance.npcDisplayer.Display(npcObject);

        // greet player
        GetComponent<NPC>().Pause();
        GameObject instance = Instantiate(speechBubble);

        speechBubbleInstance = instance.GetComponent<SpeechBubble>();
        speechBubbleInstance.Display(GetComponent<NPC>().Greet(), GameManager.instance.canvas.transform);
    }

    public override void EnterRange(Player player)
    {
        GameObject instance = Instantiate(hoverNameText);

        hoverTextInstance = instance.GetComponent<HoverName>();
        hoverTextInstance.Display(GetComponent<NPC>().GetName(), GameManager.instance.canvas.transform);
    }

    public override void ExitRange(Player player)
    {
        hoverTextInstance.Destroy();
        hoverTextInstance = null;

        if (speechBubbleInstance != null) {
            speechBubbleInstance.Destroy();
            GetComponent<NPC>().Resume();
        }
    }
}
