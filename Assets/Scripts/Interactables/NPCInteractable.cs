using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractable : Interactable {

    public GameObject hoverNameText;
    private GameObject hoverTextInstance = null;
    private const float NAME_OFFSET = 0.75f; // In game tile space, not pixel space

	private NPC npcObject;

    // Use this for initialization
    void Start () {
		npcObject = gameObject.GetComponent<NPC>();
	}

    void Update () {
        if (hoverTextInstance != null) {
            hoverTextInstance.transform.position = Camera.main.WorldToScreenPoint(
                                                gameObject.transform.position + new Vector3(0, NAME_OFFSET, 0));

        }
    }
	
	public override void Interact(Player player) {
        // open inventory
        GameManager.instance.npcDisplayer.Display(npcObject);
    }

    public override void EnterRange(Player player)
    {
        hoverTextInstance = Instantiate(hoverNameText, gameObject.transform.position, Quaternion.identity);
    
        hoverTextInstance.GetComponentInChildren<Text>().text = GetComponent<NPC>().GetName();
        hoverTextInstance.transform.SetParent(GameManager.instance.canvas.transform, false);
    }

    public override void ExitRange(Player player)
    {
        Destroy(hoverTextInstance);
        hoverTextInstance = null;
    }
}
