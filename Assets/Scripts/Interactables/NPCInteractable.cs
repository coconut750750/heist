using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>  
///		This is the NPCInteractable class.
/// 	When players come into range with an NPC, a hover name object appears.
///     When players interact with an NPC, a speech bubble object appears that hides the hover name.
///     Only one pop up can be visible at a time.
/// 	SAVING AND LOADING: None
/// </summary>  
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
        npcObject.Pause();

        if (speechBubbleInstance == null) {
            GameObject instance = Instantiate(speechBubble);
            speechBubbleInstance = instance.GetComponent<SpeechBubble>();
        }

        speechBubbleInstance.Display(npcObject.Greet(), GameManager.instance.canvas.transform);

        if (hoverTextInstance != null) {
            hoverTextInstance.Destroy();
        }
    }

    public override void EnterRange(Player player)
    {
        if (speechBubbleInstance != null) {
            return;
        }
        GameObject instance = Instantiate(hoverNameText);

        hoverTextInstance = instance.GetComponent<HoverName>();
        hoverTextInstance.Display(npcObject.GetName(), GameManager.instance.canvas.transform);
    }

    public override void ExitRange(Player player)
    {
        if (speechBubbleInstance != null) {
            speechBubbleInstance.Destroy();
            npcObject.Resume();
        } else {
            hoverTextInstance.Destroy();
        }
    }
}
