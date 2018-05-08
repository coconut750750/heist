using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MovingObject {

	public const float BOTTOM_END_TRADING_PERC = 0.85f;
	public const float LOWER_BOUND_TRADING_PERC = 1.15f;
	public const float UPPER_BOUND_TRADING_PERC = 1.30f;

	public const float BUY_PERC = 0.85f;

	public float newDestinationDelay;

	public int closestDestinationSquared;
	public int destinationRange;

	private const string PLAYER_TAG = "Player";

	private string npcName;
	private Inventory inventory;
	private int money = 100;
	
	private Nav2DAgent agent {
		get {
			return gameObject.GetComponent<Nav2DAgent>();
		}
	}
	private Vector2 destination;
	private bool isMoving;

	private int prevFloor;

	private bool searchForDest;

	IEnumerator ArriveDelay() {
		searchForDest = false;
		yield return new WaitForSeconds(newDestinationDelay);
		searchForDest = true;
	}

	protected override void Start () {
		base.Start();
		inventory = gameObject.GetComponent<Inventory>();

		agent.OnDestinationReached += NavArrived;
		agent.OnNavigationStarted += NavStarted;
		agent.OnDestinationInvalid += DestinationInvalid;
		agent.maxSpeed = moveSpeed;

		prevFloor = GetFloor();
		searchForDest = true;

		UpdateSortingLayer();
	}

	protected override void FixedUpdate() {
		if (isMoving) {
			UpdateAnimator(new Vector3(rb2D.velocity.x, rb2D.velocity.y, 0));
		} else {
			if (searchForDest) {
				Bounds nav2DBounds = agent.polyNav.masterBounds;
				Vector2 newDest = GenerateRandomDest(nav2DBounds);
				if ((newDest - (Vector2)gameObject.transform.position).sqrMagnitude >= closestDestinationSquared) {
					agent.SetDestination(newDest);
				}
			}
		}
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
		UpdateSortingLayer();
	}

	protected void UpdateSortingLayer() {
		if (GetFloor () == 1) {
			gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Elevated1";
		} else if (GetFloor () == 2) {
			gameObject.GetComponent<SpriteRenderer>().sortingLayerName = "Elevated2";
		}
	}

	// Generates random destination within bound and within the destination range
	protected Vector2 GenerateRandomDest(Bounds bound) {
		Vector2 currPos = transform.position;

		float posMinX = currPos.x - destinationRange;
		float posMaxX = currPos.x + destinationRange;
		float posMinY = currPos.y - destinationRange;
		float posMaxY = currPos.y + destinationRange;

		float minX = Mathf.Max(bound.min.x, posMinX);
		float maxX = Mathf.Min(bound.max.x, posMaxX);
		float minY = Mathf.Max(bound.min.y, posMinY);
		float maxY = Mathf.Min(bound.max.y, posMaxY);

		float x = Mathf.Round(Random.Range(minX, maxX));
		float y = Mathf.Round(Random.Range(minY, maxY));

		return new Vector2(x, y);
	}

	public void SetAgentNav(Nav2D nav) {
		agent.polyNav = nav;
	}

	public Inventory GetInventory() {
		return inventory;
	}

	protected void NavStarted() {
		Debug.Log("started");
		isMoving = true;
	}

	protected void NavArrived() {
		Debug.Log("arrived");
		isMoving = false;
		StartCoroutine(ArriveDelay());

		if (GetFloor() != prevFloor) {
			prevFloor = GetFloor();
			if (prevFloor == 1) {
				SetAgentNav(GameManager.instance.groundNav);
			} else if (prevFloor == 2) {
				SetAgentNav(GameManager.instance.floor2Nav);
			}
		}
	}

	protected void DestinationInvalid() {
		Debug.Log("invalid dest");
		isMoving = false;
	}

	public int GetMoney() {
		return money;
	}

	public void SetMoney(int money) {
		this.money = money;
	}

    public override void Load()
    {
        
    }

    public override void Save()
    {
        
    }
}
