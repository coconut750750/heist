using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MovingObject {

	private const string FORWARD = "PlayerForwardAnim";
	private const string LEFT = "PlayerLeftAnim";
	private const string BACK = "PlayerBackAnim";
	private const string RIGHT = "PlayerRightAnim";

	public GameObject floor1;
	public GameObject floor2;

	protected override void Start (){
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
		if (GetFloor () == 1) {
			floor2.SetActive (false);
		} else if (GetFloor () == 2) {
			floor2.SetActive (true);
		}
	}

	protected override void OnTriggerExit2D(Collider2D other) {
		base.OnTriggerExit2D (other);
	}

	public string getName() {
		return "Player 1";
	}
}
