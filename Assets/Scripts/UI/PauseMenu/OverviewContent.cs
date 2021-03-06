﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverviewContent : MonoBehaviour {

	public Text goalText;
	public Text timeText;

	public GameObject goalPanel;
	public GameObject timePanel;
	public GameObject[] infoPanels;
	
	public int healthPanel;
	public int expPanel;
	public int strengthPanel;

	void Start () {
		timeText.text = GameManager.instance.GetTimeString();
		SetInfoPanels();
	}

	private void SetInfoPanels() {
		Player p = GameManager.instance.mainPlayer;
		infoPanels[healthPanel].transform.Find("Slider").GetComponent<Slider>().value = p.GetHealth();
		infoPanels[expPanel].transform.Find("Slider").GetComponent<Slider>().value = p.GetExperience();
		infoPanels[strengthPanel].transform.Find("Slider").GetComponent<Slider>().value = p.GetStrength();
	}
}
