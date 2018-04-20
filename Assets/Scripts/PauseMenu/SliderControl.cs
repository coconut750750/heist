using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour {

	public Slider[] sliders;
	public Text[] sliderTexts;

	void Awake () {
		for (int i = 0; i < sliders.Length; i++) {
			int index = i;
			sliders[i].onValueChanged.AddListener(delegate { 
				SetSliderText(index); 
			});
		}
	}

	public void SetSliderText(int i) {
		sliderTexts[i].text = "" + sliders[i].value;
	}
}
