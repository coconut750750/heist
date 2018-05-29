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

    public HoverName hoverNameText;
    private HoverName hoverTextInstance = null;

    public SpeechBubble speechBubble;
    private SpeechBubble speechBubbleInstance = null;

    public NPCOptions npcOptions;
    private NPCOptions npcOptionsInstance = null;

	private NPC npcObject;
    private bool interacted;

    // Use this for initialization
    void Start () {
		npcObject = gameObject.GetComponent<NPC>();
        interacted = false;
	}

    void Update () {
        if (!base.enabled) {
            return;
        }
        if (hoverTextInstance != null) {
            hoverTextInstance.UpdatePosition(gameObject.transform.position);
        }
        if (speechBubbleInstance != null) {
            speechBubbleInstance.UpdatePosition(gameObject.transform.position);
        }
        if (npcOptionsInstance != null) {
            npcOptionsInstance.UpdatePosition(gameObject.transform.position);
        }
    }
	
	public override void Interact(Player player) {
        // greet player
        npcObject.Pause();

        if (!interacted) {
            speechBubbleInstance = Instantiate(speechBubble);
            speechBubbleInstance.Display(GameManager.instance.canvas.transform);

            npcOptionsInstance = Instantiate(npcOptions);
            npcOptionsInstance.Display(GameManager.instance.canvas.transform);
            npcOptionsInstance.SetCallbacks(ShowInventory, ShowQuest, ShowInfo);

            interacted = true;
        }

        speechBubbleInstance.UpdateText(npcObject.Greet());

        if (hoverTextInstance != null) {
            hoverTextInstance.Destroy();
        }
    }

    public override void EnterRange(Player player)
    {
        if (speechBubbleInstance != null) {
            return;
        }

        hoverTextInstance = Instantiate(hoverNameText);
        hoverTextInstance.Display(npcObject.GetName(), GameManager.instance.canvas.transform);
    }

    public override void ExitRange(Player player)
    {
        if (interacted) {
            speechBubbleInstance.Destroy();
            speechBubbleInstance = null;
            npcOptionsInstance.Destroy();

            npcObject.Resume();
            interacted = false;
        } else {
            hoverTextInstance.Destroy();
        }
    }

    public void ShowInventory() {
        GameManager.instance.npcDisplayer.Display(npcObject);
    }

    public void ShowQuest() {

    }

    public void ShowInfo() {

    }
}
