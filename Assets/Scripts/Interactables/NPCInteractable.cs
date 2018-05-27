using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractable : Interactable {

    public GameObject hoverNameText;
    private GameObject hoverTextInstance = null;
    private Vector3 NAME_OFFSET = new Vector3(0, 0.75f, 0); // In game tile space, not pixel space

    public GameObject speechBubble;
    private GameObject speechBubbleInstance = null;
    private Vector3 SPEECH_OFFSET = new Vector3(0, 0, 0);

	private NPC npcObject;

    // Use this for initialization
    void Start () {
		npcObject = gameObject.GetComponent<NPC>();
	}

    void Update () {
        if (hoverTextInstance != null) {
            hoverTextInstance.transform.position = Camera.main.WorldToScreenPoint(
                                                gameObject.transform.position + NAME_OFFSET);
        }
        if (speechBubbleInstance != null) {
            speechBubbleInstance.transform.position = Camera.main.WorldToScreenPoint(
                                                gameObject.transform.position + NAME_OFFSET);
        }
    }
	
	public override void Interact(Player player) {
        // open inventory
        // GameManager.instance.npcDisplayer.Display(npcObject);

        // greet player
        speechBubbleInstance = Instantiate(speechBubble);

        speechBubbleInstance.GetComponentInChildren<Text>().text = GetComponent<NPC>().Greet();
        speechBubbleInstance.transform.SetParent(GameManager.instance.canvas.transform, false);
    }

    public override void EnterRange(Player player)
    {
        hoverTextInstance = Instantiate(hoverNameText);
    
        hoverTextInstance.GetComponentInChildren<Text>().text = GetComponent<NPC>().GetName();
        hoverTextInstance.transform.SetParent(GameManager.instance.canvas.transform, false);
    }

    public override void ExitRange(Player player)
    {
        Destroy(hoverTextInstance);
        hoverTextInstance = null;
    }
}
