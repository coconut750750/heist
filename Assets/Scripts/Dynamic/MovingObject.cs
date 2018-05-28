using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public abstract class MovingObject : MonoBehaviour {

	private const string CLASS_NAME = "movingobj";

	protected Rigidbody2D rb2D;
	private bool paused = false;

	private const float DOOR_DELAY_SECONDS = 0.02f;

	private const string STAIRS_TAG = "Stairs";
	protected int onStairs = 0;

	protected int floor = 0;

	public float moveSpeed;

	private int forwardStateHash = Animator.StringToHash("Base Layer.Forward");
	private int backStateHash = Animator.StringToHash("Base Layer.Back");
	private int leftStateHash = Animator.StringToHash("Base Layer.Left");
	private int rightStateHash = Animator.StringToHash("Base Layer.Right");

	private int forwardHash = Animator.StringToHash("Forward");
	private int backHash = Animator.StringToHash("Back");
	private int leftHash = Animator.StringToHash("Left");
	private int rightHash = Animator.StringToHash("Right");
	private int punchHash = Animator.StringToHash("Punch");

	protected Animator animator;

	protected string filename;

	protected int health = 100;
	protected int money = 100;
	protected int exp = 0;
	protected int strength = 0;

	IEnumerator doorDelay() {
		paused = true;
		yield return new WaitForSeconds(DOOR_DELAY_SECONDS);
		paused = false;
	}

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

		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

		if (Mathf.Abs (movement.y) >= Mathf.Abs (movement.x)) {
			if (movement.y <= 0) {
				if (stateInfo.fullPathHash != forwardStateHash) {
					animator.SetTrigger(forwardHash);
				}
			} else {
				if (stateInfo.fullPathHash != backStateHash) {
					animator.SetTrigger(backHash);
				}
			}
		} else {
			if (movement.x <= 0) {
				if (stateInfo.fullPathHash != leftStateHash) {
					animator.SetTrigger(leftHash);
				}
			} else {
				if (stateInfo.fullPathHash != rightStateHash) {
					animator.SetTrigger(rightHash);
				}
			}
		}
	}

	public void Punch() {
		animator.SetTrigger(punchHash);
	}

	protected virtual void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag (STAIRS_TAG)) {
			if (onStairs == 0) {
				floor = 1 - floor;

				transform.position = new Vector3 (transform.position.x, transform.position.y, 
					0 - (float)floor / 10);
				gameObject.layer = 17 - gameObject.layer;
				onStairs += 2;
			}
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.CompareTag (STAIRS_TAG) && onStairs > 0) {
			onStairs -= 1;
		}
	}

	public void StartDoorDelay() {
		StartCoroutine (doorDelay ());
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

	protected void LoadFromData(MovingObjectData data) {
		this.onStairs = data.onStairs;
		this.floor = data.floor;
		rb2D.transform.position = data.getPosition();

		this.money = data.money; this.health = data.health; this.exp = data.exp; this.strength = data.strength;
	}
}

[System.Serializable]
public class MovingObjectData : GameData {
	public int onStairs;
	public int floor;
	public float xPos;
	public float yPos;
	public float zPos;

	public int money;
	public int health;
	public int exp;
	public int strength;

	public MovingObjectData() {
		
	}

	public MovingObjectData(MovingObject moveObj) {
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