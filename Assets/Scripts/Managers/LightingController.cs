using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingController : MonoBehaviour {

	private Light lightComponent;

	private const float min = 0.1f;
	private const float max = 0.9f;
	private const float diff = 0.8f;

	void Start () {
		lightComponent = gameObject.GetComponent<Light>();
	}
	
	void FixedUpdate () {
		float hour = GameManager.instance.GetHour();
		float minute = GameManager.instance.GetMinute();
		if (hour >= 20 || hour <= 6) {
			lightComponent.intensity = min;
		} else if (hour == 7) {
			lightComponent.intensity = min + minute / 60f * diff;
		} else if (hour == 19) {
			lightComponent.intensity = max - minute / 60f * diff;
		} else {
			lightComponent.intensity = max;
		}
	}
}
