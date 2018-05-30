using System.Collections;
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

	// true if visible by camera
	public bool visible = true;
	
	// called when NPC arrives at destination. toggles the canSearchForDest boolean
	IEnumerator ArriveDelay() {
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
	}

	void OnEnable() {
		canSearchForDest = true;
	}

	public void Spawn() {
		RefreshInventory();
		gameObject.SetActive(true);
	}

	protected override void FixedUpdate() {
		if (!isMoving && canSearchForDest) {
			SetNewDestination();
		}

		UpdateVisibility();
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
		UpdateVisibility();
		UpdateSortingLayer();
	}

	// depending on the floor the NPC is in, its sorting layer will change
	protected void UpdateSortingLayer() {
		if (GetFloor () == 1) {
			gameObject.GetComponent<SpriteRenderer>().sortingLayerName = Constants.ELEVATED1;
		} else if (GetFloor () == 2) {
			gameObject.GetComponent<SpriteRenderer>().sortingLayerName = Constants.ELEVATED2;
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

	/// INTERACTION ///

	public string Greet() {
		// TODO: change greetings randomly and depending on npc's opinion of player
		return "Hello there";
	}

	public override void GetHitBy(Character other) {
		base.GetHitBy(other);

		// ensure that npc is moving when it gets hit
		SetNewDestination();
		Resume();
	}

	/// NAVIGATION ///

	protected void NavStarted() {
		isMoving = true;
		canSearchForDest = false;
	}

	// if npc lands on stairs, it will either go up a floor or down a floor
	protected void NavArrived() {
		isMoving = false;
		StartCoroutine(ArriveDelay());

		if (GetFloor() != prevFloor) {
			prevFloor = GetFloor();
			UpdateAgentNav();
		}
	}

	protected void DestinationInvalid() {
		Debug.Log("invalid");
		isMoving = false;
	}

	protected void SetNewDestination() {
		Bounds nav2DBounds = agent.polyNav.masterBounds;
		destination = GenerateRandomDest(nav2DBounds);
		if ((destination - (Vector2)gameObject.transform.position).sqrMagnitude >= closestDestinationSquared) {
			agent.SetDestination(destination);
		}
	}

	/// INVENTORY ///

	void PopulateInventory() {
		int num = Random.Range(0, inventory.GetCapacity());

		for (int i = 0; i < num; i++) {
			inventory.AddItem(ItemManager.instance.GetRandomItem());
		}
	}

	void RefreshInventory() {
		inventory.RemoveAll();
		PopulateInventory();
	}

	public void SetAgentNav(Nav2D nav) {
		agent.polyNav = nav;
	}

	private void UpdateAgentNav() {
		if (GetFloor() == 1) {
			SetAgentNav(GameManager.instance.groundNav);
		} else if (GetFloor() == 2) {
			SetAgentNav(GameManager.instance.floor2Nav);
		}
	}

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

	public int GetPrevFloor() {
		return prevFloor;
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

			prevFloor = data.prevFloor;
			visible = data.visible;
			SetVisibility(visible);

			UpdateAgentNav();

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
public class NPCData : CharacterData {

	public ItemStashData inventoryData;
	public float destX;
	public float destY;

	public bool isMoving;
	public bool canSearchForDest;
	public int prevFloor;
	public bool visible;

	public NPCData(NPC npc) : base(npc) {
		inventoryData = new ItemStashData(npc.GetInventory());
		destX = npc.GetDestination().x;
		destY = npc.GetDestination().y;

		this.isMoving = npc.IsMoving();
		this.canSearchForDest = npc.CanSearchForDest();
		this.prevFloor = npc.GetPrevFloor();
		this.visible = npc.visible;
	}
}