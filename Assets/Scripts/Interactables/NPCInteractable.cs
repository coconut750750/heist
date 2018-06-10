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

    private static NPCInteractable activeInstance = null;

    public HoverName hoverNameText;
    private HoverName hoverTextInstance = null;

    public SpeechBubble speechBubble;
    private SpeechBubble speechBubbleInstance = null;

    public NPCOptions npcOptions;
    private NPCOptions npcOptionsInstance = null;

    public Alert exclaimIcon;
    private Alert exclaimInstance = null;

    public Alert quest;
    private Alert questInstance = null;
    private bool hasQuest;

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
    }
	
	public override void Interact(Player player) {
        // ensures that if another npc comes when player is interacting, the first one
        // is closed
        if (activeInstance != null && activeInstance != this) {
            activeInstance.Interact(player);
            return;
        }

        interacted = !interacted;
        if (!interacted) { 
            FinishInteraction();
        } else {
            StartInteraction();
        }
    }

    public void ShowFightAlert(Character opponent) {
        HideQuestIcon();
        ShowExclaimIcon();
    }

    public void HideFightAlert() {
        if (hasQuest) {
            ShowQuestIcon();
        }
        HideExclaimIcon();
    }

    public void ShowQuestAlert() {
        hasQuest = true;
        ShowQuestIcon();
    }

    public void HideQuestAlert() {
        hasQuest = false;
        HideQuestIcon();
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

    // disable button b so player can't attack
    // since there isnt a cover when player interacts with npc
    private void StartInteraction() {
        activeInstance = this;
        player.DisableButtonB();

        ShowSpeechBubble();
        ShowNPCOptions();
        HideHoverText(); // avoid overlap with speech bubble

        npc.Pause();
        player.Pause();
    }

     //hide pop ups, and enable button b and resumes npc and player
    private void FinishInteraction() {
        activeInstance = null;
        player.EnableButtonB();
        HideSpeechBubble();
        HideNPCOptions();

        npc.Resume();
        player.Resume();
    }

    private void ShowHoverText() {
        hoverTextInstance = Instantiate(hoverNameText);
        hoverTextInstance.Display(npc.GetName(), gameObject, GameManager.instance.canvas.transform);
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

    private void ShowExclaimIcon() {
        exclaimInstance = Instantiate(exclaimIcon);
        exclaimInstance.Display(gameObject, GameManager.instance.canvas.transform);
    }

    private void HideExclaimIcon() {
        if (exclaimInstance != null) {
            exclaimInstance.Destroy();
            exclaimInstance = null;
        }
    }

    private void ShowQuestIcon() {
        questInstance = Instantiate(quest);
        questInstance.Display(gameObject, GameManager.instance.canvas.transform);
    }

    private void HideQuestIcon() {
        if (questInstance != null) {
            questInstance.Destroy();
            questInstance = null;
        }
    }
            
    public void HideAllPopUps() {
        HideHoverText();
        HideSpeechBubble();
        HideNPCOptions();
        HideExclaimIcon();
        HideQuestIcon();
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
