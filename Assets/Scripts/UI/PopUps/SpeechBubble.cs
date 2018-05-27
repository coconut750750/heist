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
	private bool startFade = false;
	private float DURATION_TO_DISAPPEAR = 5f;
	private float durationToDisappear;
	private Vector3 finalPos;
	private float startAlpha;

    public SpeechBubble() : base(speechOffset)
    {
		durationToDisappear = DURATION_TO_DISAPPEAR;
    }

	void Start() {
		startAlpha = GetComponent<Image>().color.a;
	}

	public void UpdateText(string speech) {
		GetComponentInChildren<Text>().text = speech;
	}

	void Update() {
		if (startFade) {
			UpdatePosition(finalPos);

			durationToDisappear -= Time.fixedDeltaTime;

			GetComponent<Image>().color = new Color(1f, 1f, 1f, startAlpha * durationToDisappear / DURATION_TO_DISAPPEAR);
			GetComponentInChildren<Text>().color = new Color(0, 0, 0, durationToDisappear / DURATION_TO_DISAPPEAR);
			if (durationToDisappear <= 0) {
				Destroy(gameObject);
			}
		}
	}

	public override void Destroy() {
		startFade = true;
		finalPos = Camera.main.ScreenToWorldPoint(transform.position) - speechOffset;
	}
}
