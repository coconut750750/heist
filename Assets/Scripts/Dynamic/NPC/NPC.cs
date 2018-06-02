﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>  
///		This is the NPC class.
/// 	Each NPC has a Nav2DAgent, which guides it around the map. The destination of the agent is
///  set to a vector2 object. Upon arrival, the NPC delays for a predetermined time span. Then it will
///	 generate another destination.
///  	Each NPC also has an inventory and stats like the player. The player can interact with the
///  inventory.
///
/// 	SAVING AND LOADING:
/// 	Save: INDEPENDENT NPCs save themselves and their item stashes. Their item stashes do not save
///				DEPENDENT NPCs do not save themselves nor their item stashes. Their spawner saves them
///						including their item stashes
/// 	Load: NPC's have a function to load, but will be loaded by their spawner if they were created
///			by one.
/// </summary>  
public class NPC : Character {

	public const float BOTTOM_END_TRADING_PERC = 0.85f;
	public const float LOWER_BOUND_TRADING_PERC = 1.15f;
	public const float UPPER_BOUND_TRADING_PERC = 1.30f;

	public const float SELL_PERC = 1.10f; // sells an item at 110% of the price
	public const float BUY_PERC = 0.85f; // buys an item at 85% of the price

	// how long to wait before finding another destination
	public float maxDestinationDelay;

	// closest destination npc will generate
	public int closestDestinationSquared;
	// range where the next destinations will generate
	public int destinationRange;

	// the floor to start on. used for debugging
	public int startFloor = 1;

	private string npcName = "Billy";
	private Inventory inventory;
	
	private Nav2DAgent agent {
		get {
			return gameObject.GetComponent<Nav2DAgent>();
		}
	}

	private Vector2 destination;
	private bool isMoving; // true if npc is moving
	private bool canSearchForDest; // true if npc may search for another destination

	// independent if npc not spawned
	private bool independent = true;

	// called when npc dies
	public event Action<NPC> OnDeath;

	// true if visible by camera
	public bool visible = true;
	
	// called when NPC arrives at destination. toggles the canSearchForDest boolean
	IEnumerator ArriveDelay() {
		yield return new WaitForSeconds(UnityEngine.Random.Range(0, maxDestinationDelay));
		canSearchForDest = true;
	}

	// fighting member variables
	// how long in seconds until npc starts chasing
	public const float GET_HIT_DELAY = 0.2f;

	// how long to wait after punch to start moving again
	public const float AFTER_PUNCH_DELAY = 0.5f;

	// how far opponent needs to run (squared) before npc gives up retaliating
	public const float SQUARED_STOP_RETALIATE_DIST = 25f;

	// square magnitude distance between npc and player when npc should stop dynamically
	// updating position
	public const float CLOSE_ENOUGH_OPPONENT_DISTANCE = 1;

	private bool fighting = false;
	private Character opponent = null;

	IEnumerator ChaseAfter() {
		yield return new WaitForSeconds(GET_HIT_DELAY);
		fighting = true;
	}

	IEnumerator Retaliate() {
		// need to face the correct direction otherwise punch will be missed
		Vector3 displacement = opponent.transform.position - transform.position;
		Debug.Log(displacement);
		if (Mathf.Abs(displacement.x) >= Mathf.Abs(displacement.y)) {
			if (displacement.x > 0) {
				Punch(AnimationDirection.Right, Constants.PLAYER_ONLY_LAYER);
			} else {
				Punch(AnimationDirection.Left, Constants.PLAYER_ONLY_LAYER);
			}
		} else {
			// if at 0, looks nice if npc facing back (punching into screen)
			if (displacement.y >= 0) {
				Punch(AnimationDirection.Back, Constants.PLAYER_ONLY_LAYER);
			} else {
				Punch(AnimationDirection.Forward, Constants.PLAYER_ONLY_LAYER);
			}
		}

		// don't let npc search for dest because we want npc to remain still for a bit
		// need to explicitly set to false because it may have turned true
		// (which is does after maxDestinationDelay on arrival)
		canSearchForDest = false;
		fighting = false;
		opponent = null;
		
		yield return new WaitForSeconds(AFTER_PUNCH_DELAY);
		EndRetaliateAnimator();
		canSearchForDest = true;
	}

	/// END MEMBER VARIABLES

	protected override void Awake() {
		base.Awake();

		inventory = gameObject.GetComponent<Inventory>();
		inventory.SetIndependent(false); // set the inventory as dependent on the NPC
	}

	protected override void Start() {
		base.Start();
		
		// add callback functions for the agent
		agent.OnDestinationReached += NavArrived;
		agent.OnSetVelocity += Move;
		agent.OnNavigationStarted += NavStarted;
		agent.OnDestinationInvalid += DestinationInvalid;

		agent.maxSpeed = moveSpeed;

		// floor = startFloor - 1;
		// OnFloorChanged();
	}

	void OnEnable() {
		canSearchForDest = true;
	}

	public void Spawn() {
		RefreshInventory();
		gameObject.SetActive(true);
	}

	protected override void FixedUpdate() {
		if (fighting) {
			// if fighting, constantly update the destination to the opponent
			//   since player can be moving
			// the set destination, however, is not exactly the opponent's position
			// there is a slight offset (PUNCH_DISTANCE) so that the player can actually see
			//   the npc punching
			// so, we calculate the closest point that is PUNCH_DISTANCE away from
			//   the opponent's position (that is perpendicular to the player)

			Vector3 displacement = opponent.transform.position - transform.position;
			if (displacement.sqrMagnitude > SQUARED_STOP_RETALIATE_DIST) {
				// opponent is too far, so give up fighting
				fighting = false;
				EndRetaliateAnimator();
			}

			if (displacement.sqrMagnitude > CLOSE_ENOUGH_OPPONENT_DISTANCE) {
				// this means npc far enough to update dest
				if (Mathf.Abs(displacement.x) >= Mathf.Abs(displacement.y)) {
					displacement.y = 0;
				} else {
					displacement.x = 0;
				}
				// mag == x + y since either one is a non-zero and the other is 0
				Vector3 offset = displacement / Mathf.Abs(displacement.x + displacement.y) * PUNCH_DISTANCE;

				SetNewDestination(opponent.transform.position - offset);
			}
		}
		else if (!isMoving && canSearchForDest) {
			SetNewRandomDestination();
		}

		UpdateVisibility();
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
		OnFloorChanged();
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

		float x = Mathf.Round(UnityEngine.Random.Range(minX, maxX));
		float y = Mathf.Round(UnityEngine.Random.Range(minY, maxY));

		return new Vector2(x, y);
	}

	/// INTERACTION ///

	public string Greet() {
		// TODO: change greetings randomly and depending on npc's opinion of player
		return "Hello there";
	}

	public override void GetHitBy(Character other) {
		base.GetHitBy(other);

		// if health is 0 or less, die and call OnDeath function if there is one
		// usually, OnDeath is set by NPC spawner that just removes this object from the array
		if (health <= 0) {
			if (OnDeath != null) {
				OnDeath(this);
			}
			Destroy(gameObject);
		}

		// ensure that npc is moving when it gets hit
		Resume();

		// fight back
		// chase after opponent
		StartCoroutine(ChaseAfter());
		opponent = other;

		// adjust animator to be in fighting layer
		StartRetaliateAnimator();

		// hide all pop ups
		GetComponent<NPCInteractable>().HideAllPopUps();
	}

	protected void StartRetaliateAnimator() {
		animator.SetLayerWeight(1, 1);
	}

	protected void EndRetaliateAnimator() {
		animator.SetLayerWeight(1, 0);
	}

	/// NAVIGATION ///

	protected void NavStarted() {
		isMoving = true;
		canSearchForDest = false;
	}

	protected void NavArrived() {
		if (fighting) {
			// if fighting and arrived and dest, hit the opponent
			StartCoroutine(Retaliate());
		} else {
			StartCoroutine(ArriveDelay());
		}

		isMoving = false;

		// if npc lands on stairs, it will enter trigger
	}

	protected void DestinationInvalid() {
		isMoving = false;
	}

	protected void SetNewDestination(Vector2 newDest) {
		agent.SetDestination(newDest);
	}

	protected void SetNewRandomDestination() {
		Bounds nav2DBounds = agent.polyNav.masterBounds;
		destination = GenerateRandomDest(nav2DBounds);
		if ((destination - (Vector2)gameObject.transform.position).sqrMagnitude >= closestDestinationSquared) {
			agent.SetDestination(destination);
		}
	}

	/// INVENTORY ///

	void PopulateInventory() {
		int num = UnityEngine.Random.Range(0, inventory.GetCapacity());

		for (int i = 0; i < num; i++) {
			inventory.AddItem(ItemManager.instance.GetRandomItem());
		}
	}

	void RefreshInventory() {
		inventory.RemoveAll();
		PopulateInventory();
	}

	// called when npc enters trigger (could be stairs)
	// called when first loads
	private void OnFloorChanged() {
		UpdateAgentNav();
		UpdateSortingLayer();
	}

	public void SetAgentNav(Nav2D nav) {
		agent.polyNav = nav;
	}

	// called when floor changes
	// changes agent nav to correct floor
	private void UpdateAgentNav() {
		if (GetFloor() == 1) {
			SetAgentNav(GameManager.instance.groundNav);
		} else if (GetFloor() == 2) {
			SetAgentNav(GameManager.instance.floor2Nav);
		}
	}

	// called only when floor changes
	// depending on the floor the NPC is in, its sorting layer will change
	protected void UpdateSortingLayer() {
		if (GetFloor () == 1) {
			gameObject.GetComponent<SpriteRenderer>().sortingLayerName = Constants.ELEVATED1;
		} else if (GetFloor () == 2) {
			gameObject.GetComponent<SpriteRenderer>().sortingLayerName = Constants.ELEVATED2;
		}
	}

	// called every frame
	// if npc and player not on same floor, hide npc
	private void UpdateVisibility() {
		if (GetFloor() != GameManager.instance.GetVisibleFloor() && visible) {
			SetVisibility(false);
		} else if (GetFloor() == GameManager.instance.GetVisibleFloor() && !visible) {
			SetVisibility(true);
		}
	}

	private void SetVisibility(bool visible) {
		this.visible = visible;
		GetComponent<SpriteRenderer>().enabled = visible;
		GetComponent<Animator>().enabled = visible;
		GetComponent<NPCInteractable>().SetEnabled(visible);
	}

	public Inventory GetInventory() {
		return inventory;
	}

	public Vector2 GetDestination() {
		return destination;
	}

	public string GetName() {
		return npcName;
	}

	public bool IsMoving() {
		return isMoving;
	}

	public bool CanSearchForDest() {
		return canSearchForDest;
	}

	public bool IsIndependent() {
		return independent;
	}

	public void SetIndependent(bool independent) {
		this.independent = independent;
	}

	public override void Save()
    {
		if (independent) {
			NPCData data = new NPCData(this);
			GameManager.Save(data, base.filename);
		}
    }

   public override void Load()
    {
		// dont need to check for independence because if npc is not independent,
		// the game object won't exist when game starts
        LoadFromFile(base.filename);
    }

	public void LoadFromFile(string filename)
	{
		NPCData data = GameManager.Load<NPCData>(filename);
        LoadFromData(data);
	}

	public void LoadFromData(NPCData data)
	{
		if (data != null) {
			base.LoadFromData(data);
			
			ItemStashData inventoryData = data.inventoryData;
			inventory.LoadFromInventoryData(inventoryData);

			destination = new Vector2(data.destX, data.destY);
			isMoving = data.isMoving;
			canSearchForDest = data.canSearchForDest;

			SetVisibility(data.visible);
			OnFloorChanged();

			if (isMoving) {
				SetNewDestination(new Vector2(-1, 11)); //-1, 11
			} else if (!canSearchForDest) {
				StartCoroutine(ArriveDelay());
			}
		} else {
			//Destroy(this);
		}
	}
}

[System.Serializable]
public class NPCData : CharacterData {

	public ItemStashData inventoryData;
	public float destX;
	public float destY;

	public bool isMoving;
	public bool canSearchForDest;
	public bool visible;

	public NPCData(NPC npc) : base(npc) {
		inventoryData = new ItemStashData(npc.GetInventory());
		destX = npc.GetDestination().x;
		destY = npc.GetDestination().y;

		this.isMoving = npc.IsMoving();
		this.canSearchForDest = npc.CanSearchForDest();
		this.visible = npc.visible;
	}
}