﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
public class NPC : MovingObject {

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

	private const string PLAYER_TAG = "Player";

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

	private int prevFloor; // the floor before arriving at destination

	// independent if npc not spawned
	private bool independent = true;

	// called when NPC arrives at destination. toggles the canSearchForDest boolean
	IEnumerator ArriveDelay() {
		canSearchForDest = false;
		yield return new WaitForSeconds(Random.Range(0, maxDestinationDelay));
		canSearchForDest = true;
	}

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

		prevFloor = GetFloor();

		UpdateSortingLayer();

		if (isActiveAndEnabled) {
			// need to populate when first started
			PopulateInventory();
		}
		
	}

	void OnEnable() {
		canSearchForDest = true;

		if (inventory.GetCapacity() > 0) {
			// need to populate when enabled, but not before start()
			PopulateInventory();
		}
	}

	protected override void FixedUpdate() {
		if (!isMoving && canSearchForDest) {
			Bounds nav2DBounds = agent.polyNav.masterBounds;
			destination = GenerateRandomDest(nav2DBounds);
			if ((destination - (Vector2)gameObject.transform.position).sqrMagnitude >= closestDestinationSquared) {
				agent.SetDestination(destination);
			}
		}
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
		UpdateSortingLayer();
	}

	// depending on the floor the NPC is in, its sorting layer will change
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

	protected void NavStarted() {
		isMoving = true;
	}

	// if npc lands on stairs, it will either go up a floor or down a floor
	protected void NavArrived() {
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
		isMoving = false;
	}

	void PopulateInventory() {
		int num = Random.Range(0, inventory.GetCapacity());
		Debug.Log(num);
		Debug.Log(inventory.GetNumItems());

		for (int i = 0; i < num; i++) {
			inventory.AddItem(ItemManager.instance.GetRandomItem());
			Debug.Log("added item for " + gameObject.name);
		}
	}

	public override void Save()
    {
		if (independent) {
			NPCData data = new NPCData(this);
			GameManager.Save(data, base.filename);
		}
    }

	public void SetAgentNav(Nav2D nav) {
		agent.polyNav = nav;
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

	public int GetPrevFloor() {
		return prevFloor;
	}

	public bool IsIndependent() {
		return independent;
	}

	public void SetIndependent(bool independent) {
		this.independent = independent;
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

			prevFloor = data.prevFloor;
			if (isMoving) {
				agent.SetDestination(destination);
			} else if (!canSearchForDest) {
				StartCoroutine(ArriveDelay());
			}
		} else {
			//Destroy(this);
		}
	}
}

[System.Serializable]
public class NPCData : MovingObjectData {

	public ItemStashData inventoryData;
	public float destX;
	public float destY;

	public bool isMoving;
	public bool canSearchForDest;
	public int prevFloor;

	public NPCData(NPC npc) : base(npc) {
		inventoryData = new ItemStashData(npc.GetInventory());
		destX = npc.GetDestination().x;
		destY = npc.GetDestination().y;

		this.isMoving = npc.IsMoving();
		this.canSearchForDest = npc.CanSearchForDest();
		this.prevFloor = npc.GetPrevFloor();
	}
}