using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MovingObject {

	public float moveSpeed;
	private float MovementRange = 25f;

	private const string FORWARD = "PlayerForwardAnim";
	private const string LEFT = "PlayerLeftAnim";
	private const string BACK = "PlayerBackAnim";
	private const string RIGHT = "PlayerRightAnim";

	private Animator animator;

	protected override void Start (){
		animator = GetComponent<Animator> ();
	}

	protected override void FixedUpdate() {
		float moveHorizontal = 0;
		float moveVertical = 0;

		Vector3 movement;
		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		moveHorizontal = Input.GetAxis ("Horizontal");
		moveVertical = Input.GetAxis ("Vertical");
		movement = Vector3.ClampMagnitude(new Vector3(moveHorizontal, moveVertical, 0f), MovementRange);
		
		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		moveHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		moveVertical = CrossPlatformInputManager.GetAxis("Vertical");
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

		#endif
		
		movement *= moveSpeed;

		base.Move(movement);

		if (movement.sqrMagnitude == 0) {
			return;
		}

		string currentAnim = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

		if (Mathf.Abs (moveVertical) >= Mathf.Abs (moveHorizontal)) {
			if (moveVertical <= 0) {
				if (currentAnim != FORWARD) {
					animator.Play(FORWARD);
				}
				animator.Play(FORWARD);
			} else {
				if (currentAnim != BACK) {
					animator.Play(BACK);
				}
			}
		} else {
			if (moveHorizontal <= 0) {
				if (currentAnim != LEFT) {
					animator.Play(LEFT);
				}
			} else {
				if (currentAnim != RIGHT) {
					animator.Play(RIGHT);
				}
			}
		}
	}
}
