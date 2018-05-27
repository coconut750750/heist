using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractable : Interactable {

    public GameObject hoverNameText;
    private HoverName hoverTextInstance = null;

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
            hoverTextInstance.UpdatePosition(gameObject.transform.position);
        }
        if (speechBubbleInstance != null) {
            speechBubbleInstance.transform.position = Camera.main.WorldToScreenPoint(
                                                gameObject.transform.position);
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
        GameObject instance = Instantiate(hoverNameText);
        hoverTextInstance = instance.GetComponent<HoverName>();
        hoverTextInstance.Display(GetComponent<NPC>().GetName(),GameManager.instance.canvas.transform);
    
    }

    public override void ExitRange(Player player)
    {
        hoverTextInstance.Destroy();
        hoverTextInstance = null;
    }
}
