using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public abstract class Character : MonoBehaviour {

	private const string CLASS_NAME = "movingobj";

	private const float DOOR_DELAY_SECONDS = 0.05f;

	protected Rigidbody2D rb2D;
	private bool paused = false;

	protected int onStairs = 0;

	protected int floor = 0;

	public float moveSpeed;

	protected int forwardStateHash = Animator.StringToHash("Base Layer.Forward");
	protected int backStateHash = Animator.StringToHash("Base Layer.Back");
	protected int leftStateHash = Animator.StringToHash("Base Layer.Left");
	protected int rightStateHash = Animator.StringToHash("Base Layer.Right");

	protected int forwardHash = Animator.StringToHash("Forward");
	protected int backHash = Animator.StringToHash("Back");
	protected int leftHash = Animator.StringToHash("Left");
	protected int rightHash = Animator.StringToHash("Right");
	protected int punchHash = Animator.StringToHash("Punch");

	protected Animator animator;

	protected string filename;

	protected int health = 100;
	protected int money = 100;
	protected int exp = 0;
	protected int strength = 0;

	IEnumerator DoorDelay() {
		paused = true;
		yield return new WaitForSeconds(DOOR_DELAY_SECONDS);
		paused = false;
	}

	/// GET HIT VARIABLES ///
	private const float GET_HIT_EFFECT_TIME = 1.5f;
	private const float GET_HIT_BLINK_SECONDS = 0.15f;
	private const int GET_HIT_BLINK_NUM = 10;
	private const float GET_HIT_SPEEDUP = 1.5f;

	private bool isEffectedByHit = false;

	IEnumerator Blink() {
		bool spriteEnabled = false;
		for (int i = 0; i < GET_HIT_BLINK_NUM; i++) {
			GetComponent<SpriteRenderer>().enabled = spriteEnabled;
			spriteEnabled = !spriteEnabled;
			yield return new WaitForSeconds(GET_HIT_BLINK_SECONDS);
		}
	}

	IEnumerator GetHitSpeedUp() {
		moveSpeed *= GET_HIT_SPEEDUP;
		yield return new WaitForSeconds(GET_HIT_EFFECT_TIME);
		moveSpeed /= GET_HIT_SPEEDUP;

		isEffectedByHit = false;
	}

	/// END MEMBERS

	protected virtual void Awake() {
		rb2D = GetComponent<Rigidbody2D>();
		rb2D.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
		rb2D.interpolation = RigidbodyInterpolation2D.Extrapolate;
		rb2D.freezeRotation = true;

		animator = GetComponent<Animator> ();
	}

	protected virtual void Start () {
		filename = Application.persistentDataPath + "/" + gameObject.name + "-" + CLASS_NAME + ".dat";
		Load();
	}

	#if UNITY_EDITOR || UNITY_STANDALONE
	protected void OnApplicationQuit() {
		Save();
	}
	#elif UNITY_ANDROID || UNITY_IOS
	protected void OnApplicationPause() {
		Save();
	}
	#endif

	protected virtual void FixedUpdate() {
		Move(new Vector3(0, 0, 0));		
	}

	protected void Move(Vector2 movement) {
		if (!paused && !GameManager.instance.IsPaused()) {
			rb2D.velocity = movement * moveSpeed;
			UpdateAnimator(movement);
		} else {
			rb2D.velocity = new Vector3(0, 0, 0);
		}
	}

	protected void UpdateAnimator(Vector3 movement) {
		if (movement.sqrMagnitude == 0) {
			return;
		}

		int stateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

		if (stateHash != forwardStateHash &&
			stateHash != backStateHash &&
			stateHash != leftStateHash &&
			stateHash != rightStateHash) {
				return;
			}

		if (Mathf.Abs (movement.y) >= Mathf.Abs (movement.x)) {
			if (movement.y <= 0) {
				if (stateHash != forwardStateHash) {
					animator.SetTrigger(forwardHash);
				}
			} else {
				if (stateHash != backStateHash) {
					animator.SetTrigger(backHash);
				}
			}
		} else {
			if (movement.x <= 0) {
				if (stateHash != leftStateHash) {
					animator.SetTrigger(leftHash);
				}
			} else {
				if (stateHash != rightStateHash) {
					animator.SetTrigger(rightHash);
				}
			}
		}
	}

	public virtual void Punch() {
		animator.SetTrigger(punchHash);
	}

	public virtual void GetHitBy(Character other) {
		health -= other.strength;
		if (!isEffectedByHit) {
			StartCoroutine(Blink());
			StartCoroutine(GetHitSpeedUp());
			isEffectedByHit = true;
		}
	}

	protected virtual void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag (Constants.STAIRS_TAG)) {
			if (onStairs == 0) {
				floor = 1 - floor;

				transform.position = new Vector3 (transform.position.x, transform.position.y, 
					0 - (float)floor / 10);

				onStairs += 2;
			}
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag (Constants.STAIRS_TAG) && onStairs > 0) {
			onStairs -= 1;
		}
	}

	public void StartDoorDelay() {
		StartCoroutine (DoorDelay ());
	}

	public int GetFloor() {
		return floor + 1;
	}

	public void Pause() {
		paused = true;
	}

	public void Resume() {
		paused = false;
	}

	public bool IsPaused() {
		return paused;
	}

	public int GetOnStairs() {
		return onStairs;
	}

	public Vector3 GetPosition() {
		return rb2D.transform.position;
	}

	public int GetMoney() { return money; }

	public void SetMoney(int money) { this.money = money; }

	public int GetHealth() { return health; }

	public void SetHealth(int health) { this.health = health; }

	public int GetExperience() { return exp; }

	public void SetExperience(int exp) { this.exp = exp; }

	public int GetStrength() { return strength; }

	public void SetStrength(int strength) { this.strength = strength; }

	public abstract void Save();

	public abstract void Load();

	protected void LoadFromData(CharacterData data) {
		this.onStairs = data.onStairs;
		this.floor = data.floor;
		rb2D.transform.position = data.getPosition();

		this.money = data.money; this.health = data.health; this.exp = data.exp; this.strength = data.strength;
	}
}

[System.Serializable]
public class CharacterData : GameData {
	public int onStairs;
	public int floor;
	public float xPos;
	public float yPos;
	public float zPos;

	public int money;
	public int health;
	public int exp;
	public int strength;

	public CharacterData() {
		
	}

	public CharacterData(Character moveObj) {
		SetPositionalData(moveObj.GetOnStairs(), moveObj.GetFloor() - 1, moveObj.GetPosition());
		SetStats(moveObj.GetMoney(), moveObj.GetHealth(), moveObj.GetExperience(), moveObj.GetStrength());
	}

	protected void SetPositionalData(int onStairs, int floor, Vector3 position) {
		this.onStairs = onStairs;
		this.floor = floor;
		this.xPos = position.x;
		this.yPos = position.y;
		this.zPos = position.z;
	}

	protected void SetStats(int money, int health, int exp, int strength) {
		this.money = money;
		this.health = health;
		this.exp = exp;
		this.strength = strength;
	}

	public Vector3 getPosition() {
		return new Vector3(xPos, yPos, zPos);
	}
}