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

	private NPC npc;
    private bool interacted = false;

    // Use this for initialization
    void Start () {
		npc = gameObject.GetComponent<NPC>();
	}

    void Update () {
        if (!base.enabled) {
            return;
        }

        // updates hovername's position so that it follows the npc
        if (hoverTextInstance != null) {
            hoverTextInstance.UpdatePosition(gameObject.transform.position);
        }
    }
	
	public override void Interact(Player player) {
        interacted = !interacted;
        if (!interacted) { 
            // interacted twice so resume game, hide pop ups, and enable button b
            FinishInteraction();
        } else {
            // disable button b so player can't attack
            // there isnt a cover when player interacts with npc
            StartInteraction();
        }
    }

    public override void EnterRange(Player player)
    {
        if (speechBubbleInstance != null) {
            // if there is a speech bubble already, don't want to overlap it
            // with a hovername
            return;
        }

        ShowHoverText();
    }

    public override void ExitRange(Player player)
    {
        FinishInteraction();
        HideHoverText();
    }

    private void StartInteraction() {
        player.DisableButtonB();

        npc.Pause();
        player.Pause();

        // greet player
        ShowSpeechBubble();
        ShowNPCOptions();

        // destory hover name so that it doesn't overlap with speech bubble
        HideHoverText();
    }

    private void FinishInteraction() {
        player.EnableButtonB();
        HideSpeechBubble();
        HideNPCOptions();

        npc.Resume();
        player.Resume();
    }

    private void ShowHoverText() {
        hoverTextInstance = Instantiate(hoverNameText);
        hoverTextInstance.Display(npc.GetName(), GameManager.instance.canvas.transform);
    }

    private void HideHoverText() {
        if (hoverTextInstance != null) {
            hoverTextInstance.Destroy();
            hoverTextInstance = null;
        }
    }

    private void ShowSpeechBubble() {
        if (speechBubbleInstance == null) {
            speechBubbleInstance = Instantiate(speechBubble);
            speechBubbleInstance.Display(GameManager.instance.canvas.transform);
            speechBubbleInstance.UpdateText(npc.Greet());
            speechBubbleInstance.UpdatePosition(gameObject.transform.position);
        }
    }

    private void HideSpeechBubble() {
        if (speechBubbleInstance != null) {
            speechBubbleInstance.Destroy();
            speechBubbleInstance = null;
        }
    }

    private void ShowNPCOptions() {
        if (npcOptionsInstance == null) {
            npcOptionsInstance = Instantiate(npcOptions);
            npcOptionsInstance.Display(GameManager.instance.canvas.transform);
            npcOptionsInstance.SetCallbacks(ShowInventory, ShowQuest, ShowInfo);
            npcOptionsInstance.UpdatePosition(gameObject.transform.position);
        }
    }

    private void HideNPCOptions() {
        if (npcOptionsInstance != null) {
            npcOptionsInstance.Destroy();
            npcOptionsInstance = null;
        }
    }
            
    public void HideAllPopUps() {
        HideHoverText();
        HideSpeechBubble();
        HideNPCOptions();
    }

    public override void Disable() {
        base.Disable();
        HideAllPopUps();
    }

    public void ShowInventory() {
        NPCTrade.instance.Display(npc);
    }

    public void ShowQuest() {
        NPCQuest.instance.Display(npc);
    }

    public void ShowInfo() {
        NPCInfo.instance.Display(npc);
    }
}
