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
public class SpeechBubble : MonoBehaviour {

	// In game tile space, not pixel space
	// will set X offset later
	private Vector3 speechOffset = new Vector3(-0.25f, 0.5f, 0);
	private bool startFade = false;
	private float durationToDisappear = 5f;

	public void Display(string speech, Transform parent) {
		GetComponentInChildren<Text>().text = speech;
        transform.SetParent(parent, false);
	}

	public void UpdatePosition(Vector3 rootPosition) {
		transform.position = Camera.main.WorldToScreenPoint(rootPosition + speechOffset);
	}

	void Update() {
		if (startFade) {
			durationToDisappear -= Time.fixedDeltaTime;
			if (durationToDisappear <= 0) {
				Destroy(gameObject);
			}
		}
	}

	public void Destroy() {
		startFade = true;
	}
}
