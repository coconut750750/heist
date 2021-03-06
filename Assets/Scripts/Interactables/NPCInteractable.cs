﻿using System.Collections;
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
    protected HoverName hoverTextInstance = null;

    public SpeechBubble speechBubble;
    protected SpeechBubble speechBubbleInstance = null;

    public NPCOptions npcOptions;
    protected NPCOptions npcOptionsInstance = null;

    public Alert exclaimIcon;
    protected Alert exclaimInstance = null;

    public Alert questIcon;
    protected Alert questInstance = null;

	protected NPC npc;
    private bool interacted = false;

    // Use this for initialization
    void Start () {
		npc = gameObject.GetComponent<NPC>();
	}

    public virtual void OnKnockout() {
        DestroyAllPopUps();
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

    public override void EnterRange(Player player)
    {
        if (speechBubbleInstance != null) {
            // if there is a speech bubble already, don't want to overlap it
            // with a hovername
            return;
        }

        InitHoverText();
    }

    // to be safe, when player exits range, finish interaction
    public override void ExitRange(Player player)
    {
        FinishInteraction();
        DestroyHoverText();
    }

    private void StartInteraction() {
        activeInstance = this;
        player.DisableButtonB();

        npc.Pause();
        player.Pause();

        if (!npc.IsKnockedOut()) {
            InitSpeechBubble();
        }
        InitNPCOptions();
        DestroyHoverText(); // avoid overlap with speech bubble
    }

    private void FinishInteraction() {
        if (activeInstance == this) {
            activeInstance = null;
        }
        player.EnableButtonB();

        DestroyInteractPopups();

        npc.Resume();
        player.Resume();
    }

    public void ShowFightAlert(Character opponent) {
        DestroyHoverText();
        DestroyInteractPopups();
        if (questInstance != null) {
            questInstance.Disable();
        }
        InitExclaimIcon();
    }

    public void HideFightAlert() {
        if (questInstance != null) {
            questInstance.Enable();
        }
        DestroyExclaimIcon();
    }

    public void ShowQuestIcon() {
        InitQuestIcon();
        if (exclaimInstance != null) {
            exclaimInstance.Disable();
        }
    }

    public void HideQuestIcon() {
        if (questInstance != null) {
            questInstance.Destroy();
            questInstance = null;
        }
        if (exclaimInstance != null) {
            exclaimInstance.Enable();
        }
    }

    protected void InitHoverText() {
        hoverTextInstance = Instantiate(hoverNameText);
        hoverTextInstance.Display(npc.GetName(), gameObject);
    }

    protected void DestroyHoverText() {
        if (hoverTextInstance != null) {
            hoverTextInstance.Destroy();
            hoverTextInstance = null;
        }
    }

    protected void InitSpeechBubble() {
        if (speechBubbleInstance == null) {
            speechBubbleInstance = Instantiate(speechBubble);
            speechBubbleInstance.Display();
            speechBubbleInstance.UpdateText(npc.Greet());
            speechBubbleInstance.UpdatePosition(gameObject.transform.position);
        }
    }

    protected void DestroySpeechBubble() {
        if (speechBubbleInstance != null) {
            speechBubbleInstance.Destroy();
            speechBubbleInstance = null;
        }
    }

    protected virtual void InitNPCOptions() {
        if (npcOptionsInstance == null) {
            npcOptionsInstance = Instantiate(npcOptions);
            npcOptionsInstance.Display();
            npcOptionsInstance.SetCallbacks(ShowInventory, ShowQuest, ShowInfo);
            npcOptionsInstance.UpdatePosition(gameObject.transform.position);
        }
    }

    protected void DestroyNPCOptions() {
        if (npcOptionsInstance != null) {
            npcOptionsInstance.Destroy();
            npcOptionsInstance = null;
        }
    }

    protected void InitExclaimIcon() {
        if (exclaimInstance != null) {
            exclaimInstance.Enable();
        } else {
            exclaimInstance = Instantiate(exclaimIcon);
            exclaimInstance.Display(gameObject);
        }
    }

    protected void DestroyExclaimIcon() {
        if (exclaimInstance != null) {
            exclaimInstance.Destroy();
            exclaimInstance = null;
        }
    }

    protected void InitQuestIcon() {
        if (questInstance != null) {
            questInstance.Enable();
        } else {
            questInstance = Instantiate(questIcon);
            questInstance.Display(gameObject);
        }
    }

    protected void DestroyQuestIcon() {
        if (questInstance != null) {
            questInstance.Destroy();
            questInstance = null;
        }
    }

    protected void DestroyInteractPopups() {
        DestroySpeechBubble();
        DestroyNPCOptions();
    }

    public void DestroyAllPopUps() {
        DestroyInteractPopups();
        DestroyHoverText();
        DestroyExclaimIcon();
        DestroyQuestIcon();
    }

    public override void Enable() {
        base.Enable();
        if (exclaimInstance != null) {
            exclaimInstance.Enable();
        } else if (questInstance != null) {
            questInstance.Enable();
        }
    }

    public override void Disable() {
        base.Disable();
        DestroyHoverText();
        DestroyInteractPopups();
        if (exclaimInstance != null) {
            exclaimInstance.Disable();
        }
        if (questInstance != null) {
            questInstance.Disable();
        }
    }

    public virtual void ShowInventory() {
        if (npc.IsKnockedOut()) {
            StashDisplayer.instance.DisplayInventory(npc.GetInventory());
        } else {
            NPCTrade.instance.Display(npc);
        }
    }

    public virtual void ShowQuest() {
        NPCQuest.instance.Display(npc);
    }

    public virtual void ShowInfo() {
        NPCInfo.instance.Display(npc);
    }
}
