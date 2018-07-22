using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public abstract class Character : MonoBehaviour {

	private const string CLASS_NAME = "movingobj";

	public const float DOOR_DELAY_SECONDS = 0.05f;
	public const float ATTACK_DISTANCE = 0.5f;

	protected Rigidbody2D rb2D;
	private bool paused = false;

	public float moveSpeed;

	// keep tack of previous directions to get rid of random outliers
	// only used in Move() but updated in Face()
	private AnimationDirection prevDir = AnimationDirection.Forward;

	protected enum AnimationDirection {
		Forward, Right, Back, Left, None
	}

	// TODO: extract animation state controller into object
	protected int forwardStateHash = Animator.StringToHash("Base Layer.Forward");
	protected int backStateHash = Animator.StringToHash("Base Layer.Back");
	protected int leftStateHash = Animator.StringToHash("Base Layer.Left");
	protected int rightStateHash = Animator.StringToHash("Base Layer.Right");

	protected int forwardHash = Animator.StringToHash("Forward");
	protected int backHash = Animator.StringToHash("Back");
	protected int leftHash = Animator.StringToHash("Left");
	protected int rightHash = Animator.StringToHash("Right");
	protected int attackHash = Animator.StringToHash("Attack");
	protected int knockedoutHash = Animator.StringToHash("KnockedOut");

	protected Animator animator;

	protected string filename;

	protected int health = 100;
	protected int money = 100;
	protected int exp = 0;
	protected int strength = 10;

	IEnumerator DoorDelay() {
		paused = true;
		yield return new WaitForSeconds(DOOR_DELAY_SECONDS);
		paused = false;
	}

	/// GET ATTACK VARIABLES ///
	private const float GET_ATTACK_EFFECT_TIME = 1.5f;
	private const float GET_ATTACK_BLINK_SECONDS = 0.075f;
	private const int GET_ATTACK_BLINK_NUM = 20;
	private const float GET_ATTACK_SPEEDUP = 1.25f;

	private bool isEffectedByAttack = false;

	IEnumerator Blink() {
		bool spriteEnabled = false;
		for (int i = 0; i < GET_ATTACK_BLINK_NUM; i++) {
			GetComponent<SpriteRenderer>().enabled = spriteEnabled;
			spriteEnabled = !spriteEnabled;
			yield return new WaitForSeconds(GET_ATTACK_BLINK_SECONDS);
		}
	}

	IEnumerator GetAttackSpeedUp() {
		moveSpeed *= GET_ATTACK_SPEEDUP;
		yield return new WaitForSeconds(GET_ATTACK_EFFECT_TIME);
		moveSpeed /= GET_ATTACK_SPEEDUP;

		isEffectedByAttack = false;
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
	}

	protected virtual void FixedUpdate() {
	}

	protected void Move(Vector2 movement) {
		if (CanMove()) {
			rb2D.velocity = movement * moveSpeed;
			UpdateAnimator(movement);
		} else {
			rb2D.velocity = new Vector3(0, 0, 0);
		}
	}

	private bool CanMove() {
		return !paused && !GameManager.instance.IsPaused() && health > 0;
	}

	private void Face(AnimationDirection direction, int currentAnimStateHash) {
		switch (direction) {
			case AnimationDirection.Forward:
				if (currentAnimStateHash != forwardStateHash) {
					animator.SetTrigger(forwardHash);
					prevDir = AnimationDirection.Forward;
				}
				return;
			case AnimationDirection.Back:
				if (currentAnimStateHash != backStateHash) {
					animator.SetTrigger(backHash);
					prevDir = AnimationDirection.Back;
				}
				return;
			case AnimationDirection.Left:
				if (currentAnimStateHash != leftStateHash) {
					animator.SetTrigger(leftHash);
					prevDir = AnimationDirection.Left;
				}
				return;
			case AnimationDirection.Right:
				if (currentAnimStateHash != rightStateHash) {
					animator.SetTrigger(rightHash);
					prevDir = AnimationDirection.Right;
				}
				return;
		}
	}

	protected void Face(AnimationDirection direction) {
		int stateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
		Face(direction, stateHash);
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

		AnimationDirection dirToFace = AnimationDirection.None;
		if (Mathf.Abs (movement.y) >= Mathf.Abs (movement.x)) {
			if (movement.y <= 0) {
				dirToFace = AnimationDirection.Forward;				
			} else {
				dirToFace = AnimationDirection.Back;		
			}
		} else {
			if (movement.x <= 0) {
				dirToFace = AnimationDirection.Left;		
			} else {
				dirToFace = AnimationDirection.Right;		
			}
		}
		
		if (prevDir == dirToFace) {
			Face(dirToFace);
		} else {
			prevDir = dirToFace;
		}
	}

	protected virtual void Attack(AnimationDirection animDirection, int layer) {
		Vector3 direction = Vector3.zero;
		switch (animDirection) {
			case AnimationDirection.Forward:
				direction = Vector3.down;
				break;
			case AnimationDirection.Back:
				direction = Vector3.up;
				break;
			case AnimationDirection.Left:
				direction = Vector3.left;
				break;
			case AnimationDirection.Right:
				direction = Vector3.right;
				break;
		}

		if (direction.sqrMagnitude != 0) {
			float z = transform.position.z;
			RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, ATTACK_DISTANCE, 
												 layer, z, z);
			if (hit.collider != null) {
				hit.collider.gameObject.GetComponent<Character>().GetAttackedBy(this);
			}
			
			Face(animDirection);
			animator.SetTrigger(attackHash);
		}
	}

	protected virtual void Attack(int layer) {
		int stateHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
		if (stateHash == forwardStateHash) {
			Attack(AnimationDirection.Forward, layer);
		} else if (stateHash == backStateHash) {
			Attack(AnimationDirection.Back, layer);
		} else if (stateHash == leftStateHash) {
			Attack(AnimationDirection.Left, layer);
		} else if (stateHash == rightStateHash) {
			Attack(AnimationDirection.Right, layer);
		}
	}

	public virtual void GetAttackedBy(Character other) {		
		health -= other.strength;
		
		if (health <= 0) {
			Knockout();
			return;
		}

		if (!isEffectedByAttack) {
			StartCoroutine(Blink());
			StartCoroutine(GetAttackSpeedUp());
			isEffectedByAttack = true;
		}
	}

	public virtual void Knockout() {
		animator.SetTrigger(knockedoutHash);
	}

	protected virtual void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.CompareTag (Constants.STAIRS_TAG)) {
			float currentZ = transform.position.z;
			float nextZ = -0.1f - currentZ;
			transform.position = new Vector3 (transform.position.x, transform.position.y, nextZ);
			OnEnterStairs();
		}
	}

	protected virtual void OnTriggerExit2D(Collider2D other) {
	}

	protected virtual void OnEnterStairs() {

	}

	public void StartDoorDelay() {
		StartCoroutine (DoorDelay ());
	}

	public int GetFloor() {
		int floor = (int)(transform.position.z * -10);
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

	public int GetMoney() { return money; }

	public void SetMoney(int money) { this.money = money; }

	public int GetHealth() { return health; }

	public bool IsKnockedOut() { return health <= 0; }

	public void SetHealth(int health) { this.health = health; }

	public int GetExperience() { return exp; }

	public void SetExperience(int exp) { this.exp = exp; }

	public int GetStrength() { return strength; }

	public void SetStrength(int strength) { this.strength = strength; }

	public abstract void Save();

	public virtual void Load() {
		filename = Application.persistentDataPath + "/" + gameObject.name + "-" + CLASS_NAME + ".dat";
	}

	protected void LoadFromData(CharacterData data) {
		rb2D.transform.position = data.getPosition();

		this.money = data.money; this.health = data.health; this.exp = data.exp; this.strength = data.strength;
	}

	[System.Serializable]
	public class CharacterData : GameData {
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
			SetPositionalData(moveObj.rb2D.transform.position);
			SetStats(moveObj.GetMoney(), moveObj.GetHealth(), moveObj.GetExperience(), moveObj.GetStrength());
		}

		protected void SetPositionalData(Vector3 position) {
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
}