using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public abstract class MovingObject : MonoBehaviour {

	private GameObject doorTilemapGameObj;

	private Rigidbody2D rb2D;
	private bool paused = false;

	private const string DOOR_TAG = "Door";
	private const float DOOR_DELAY_SECONDS = 0.02f;
	private Sprite currentDoorSprite;

	private const string STAIRS_TAG = "Stairs";
	private int onStairs = 0;

	private int floor = 0;

	public float moveSpeed;

	private const string FORWARD = "PlayerForwardAnim";
	private const string LEFT = "PlayerLeftAnim";
	private const string BACK = "PlayerBackAnim";
	private const string RIGHT = "PlayerRightAnim";
	private Animator animator;

	IEnumerator doorDelay(Collider2D door) {
		paused = true;
		yield return new WaitForSeconds(DOOR_DELAY_SECONDS);
		paused = false;
		door.gameObject.GetComponent<SpriteRenderer> ().sprite = null;
	}

	void Awake() {
		rb2D = GetComponent<Rigidbody2D>();
		rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb2D.interpolation = RigidbodyInterpolation2D.Extrapolate;
		rb2D.freezeRotation = true;

		animator = GetComponent<Animator> ();

	}

	// Use this for initialization
	protected virtual void Start () {
	}

	protected void FixedUpdate() {
		float moveHorizontal = 0;
		float moveVertical = 0;

		Vector3 movement;
		#if UNITY_STANDALONE || UNITY_WEBPLAYER

		moveHorizontal = Input.GetAxis ("Horizontal");
		moveVertical = Input.GetAxis ("Vertical");
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

		#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

		moveHorizontal = CrossPlatformInputManager.GetAxis("Horizontal");
		moveVertical = CrossPlatformInputManager.GetAxis("Vertical");
		movement = new Vector3(moveHorizontal, moveVertical, 0f);

		#endif

		Move(movement.normalized, moveSpeed);

        if (movement.sqrMagnitude == float.Epsilon) {
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

	protected void Move(Vector3 movement, float moveSpeed) {
		if (!paused) {
			rb2D.velocity = movement * moveSpeed;
		} else {
			rb2D.velocity = new Vector3(0, 0, 0);
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag (DOOR_TAG)) {
			StartCoroutine (doorDelay (other));
			currentDoorSprite = other.gameObject.GetComponent<SpriteRenderer> ().sprite; 
		} else if (other.gameObject.CompareTag (STAIRS_TAG)) {
			if (onStairs == 0) {
				floor = 1 - floor;

				rb2D.transform.position = new Vector3 (rb2D.transform.position.x, rb2D.transform.position.y, 
					0 - (float)floor / 10);
				gameObject.layer = 17 - gameObject.layer;
				onStairs += 2;
			}
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag (DOOR_TAG)) {
			other.gameObject.GetComponent<SpriteRenderer> ().sprite = currentDoorSprite;
		} else if (other.gameObject.CompareTag (STAIRS_TAG) && onStairs > 0) {
			onStairs -= 1;
		}
	}

	protected int GetFloor() {
		return floor + 1;
	}
}
