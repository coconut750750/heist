using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>  
///		This is the SpeechBubble class.
/// 	Pops up when players interact with an NPC. Will cover the hover name object.
///
/// 	SAVING AND LOADING: None
/// </summary>  
public class SpeechBubble : PopUp {

	// In game tile space, not pixel space
	private static Vector3 speechOffset = new Vector3(-0.25f, 0.5f, 0);
	private const float FADE_RATE = 0.01f;
	private Vector3 finalPos;
	private float startAlpha;

	IEnumerator Fade() {
		for (float f = 1f; f >= 0; f -= FADE_RATE) {
			UpdatePosition(finalPos);
			GetComponent<Image>().color = new Color(1f, 1f, 1f, startAlpha * f);
			GetComponentInChildren<Text>().color = new Color(0, 0, 0, f);
			yield return null;
		}
		Destroy(gameObject);
	}

    public SpeechBubble() : base(speechOffset) {
    }

	public override void Display() {
        base.Display();
	}

	void Start() {
		startAlpha = GetComponent<Image>().color.a;
	}

	public void UpdateText(string speech) {
		GetComponentInChildren<Text>().text = speech;
	}

	public override void Destroy() {
		finalPos = Camera.main.ScreenToWorldPoint(transform.position) - speechOffset;
		StartCoroutine(Fade());
	}
}
