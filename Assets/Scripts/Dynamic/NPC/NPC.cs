using System.Collections;
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
	// trading constants
	public const float BOTTOM_END_TRADING_PERC = 0.85f;
	public const float LOWER_BOUND_TRADING_PERC = 1.15f;
	public const float UPPER_BOUND_TRADING_PERC = 1.30f;
	public const float SELL_PERC = 1.10f;
	public const float BUY_PERC = 0.85f;

	// friendliness constants
	public const int BUY_FRIENDLY_DELTA = 1;
	public const int COMPLETE_QUEST_STAGE_FRIENDLY_DELTA = 5;
	public const int ACCEPT_QUEST_FRIENDLY_DELTA = 3;
	public const int ATTACK_FRIENDLY_DELTA = -35;
	public const int REJECT_QUEST_FRIENDLY_DELTA = -10;

	// fighting constants
	public const float GET_ATTACK_DELAY = 0.2f;
	public const float AFTER_ATTACK_DELAY = 0.5f;

	public bool debugNav = false;
	public bool saveData = true;

	// how long to wait before finding another destination
	public float maxDestinationDelay;

	// closest destination npc will generate
	public int closestDestinationSquared;
	// range where the next destinations will generate
	public int destinationRange;

	protected string npcName = "Billy";

	[HideInInspector]
	public bool hasQuest = false;
	[HideInInspector]
	public bool questActive = false;

	protected int friendliness = 50;
	protected Inventory inventory;
	protected NPCInteractable interactable;

	protected Nav2DAgent agent {
		get {
			return gameObject.GetComponent<Nav2DAgent>();
		}
	}

	protected Vector3 destination;
	protected bool isMoving;
	protected bool canSearchForDest;

	protected bool spawned = false;

	public event Action<NPC> OnKnockout;

	[HideInInspector]
	public bool visibleByCamera = true;
	
	// called when NPC arrives at destination. toggles the canSearchForDest boolean
	IEnumerator ArriveDelay() {
		yield return new WaitForSeconds(UnityEngine.Random.Range(0, maxDestinationDelay));
		canSearchForDest = true;
	}

	// fighting member variables
	protected float squaredVisionDist = 100;
	protected bool fighting = false;
	protected Character opponent = null;

	IEnumerator ChaseAfter() {
		yield return new WaitForSeconds(GET_ATTACK_DELAY);
		fighting = true;
	}

	protected virtual IEnumerator EndFight() {
		// don't let npc search for dest because we want npc to remain still for a bit
		// need to explicitly set to false because it may have turned true
		// (which is does after maxDestinationDelay on arrival)
		canSearchForDest = false;
		fighting = false;
		opponent = null;
		EndRetaliateAnimator();
		interactable.HideFightAlert();
		
		yield return new WaitForSeconds(AFTER_ATTACK_DELAY);
		canSearchForDest = true;
	}

	protected override void Awake() {
		base.Awake();

		inventory = gameObject.GetComponent<Inventory>();
		inventory.SetIndependent(false); // set the inventory as dependent on the NPC

		interactable = gameObject.GetComponent<NPCInteractable>();

		UpdateSortingLayer();
	}

	protected override void Start() {
		base.Start();
		
		// add callback functions for the agent
		agent.OnDestinationReached += NavArrived;
		agent.OnSetVelocity += Move;
		agent.OnNavigationStarted += NavStarted;
		agent.OnDestinationInvalid += DestinationInvalid;

		agent.maxSpeed = moveSpeed;

		SetName(npcName);
	}

	void OnEnable() {
		canSearchForDest = true;
	}

	public void InstantiateBySpawner(Nav2D polyNav, Transform parentTransform) {
		SetAgentNav(polyNav);
		transform.SetParent(parentTransform);

		name = Constants.NPC_NAME + npcName;
		SetSpawned(true);
	}

	public void Spawn() {
		// reset health when respawn
		base.health = 100;
		animator.enabled = true;
		RefreshInventory();
		gameObject.SetActive(true);
		if (hasQuest && !questActive) {
			interactable.ShowQuestIcon();
		}
	}

	public void Recall() {
		interactable.DestroyAllPopUps();
		gameObject.SetActive(false);
	}

	protected override void FixedUpdate() {
		if (fighting) {
			FollowOpponentUpdate();
		} else if (!isMoving && canSearchForDest) {
			SetNextDestination();
		}

		UpdateVisibility();
	}

	protected override void OnTriggerEnter2D(Collider2D other) {
		base.OnTriggerEnter2D (other);
	}

	protected override void OnEnterStairs() {
		UpdateSortingLayer();
	}

	/// INTERACTION ///

	public string Greet() {
		// TODO: change greetings randomly and depending on npc's opinion of player
		return "Hello there!";
	}

	public void ReceiveQuest() {
		interactable.ShowQuestIcon();
		hasQuest = true;
	}

	public Quest GetQuest() {
		return QuestManager.instance.GetCurrentQuest(this);
	}

	public void AcceptedQuestStage() {
		interactable.HideQuestIcon();
		AdjustFriendliness(ACCEPT_QUEST_FRIENDLY_DELTA);
		questActive = true;
	}

	public void CompletedQuestStage() {
		interactable.ShowQuestIcon();
		AdjustFriendliness(COMPLETE_QUEST_STAGE_FRIENDLY_DELTA);
		questActive = false;
	}

	public void CompletedEntireQuest() {
		interactable.HideQuestIcon();
		hasQuest = false;
	}

	public void RejectedQuest() {
		hasQuest = false;
		AdjustFriendliness(REJECT_QUEST_FRIENDLY_DELTA);
		interactable.HideQuestIcon();
	}

	public void BoughtOrTraded() {
		AdjustFriendliness(BUY_FRIENDLY_DELTA);
	}

	public override void GetAttackedBy(Character other) {
		base.GetAttackedBy(other);
		if (health <= 0) {
			return;
		}

		// ensure that npc is moving when it gets attacked
		Resume();
		
		if (!fighting) {
			StartCoroutine(ChaseAfter());
			opponent = other;
		}

		StartRetaliateAnimator();
		interactable.ShowFightAlert(other);
		AdjustFriendliness(ATTACK_FRIENDLY_DELTA);
	}

	// TODO: fix this function
	protected void FollowOpponentUpdate() {
		// the target destination, however, is not exactly the opponent's position
		// there is a slight offset (ATTACK_DISTANCE) so that the player can actually see
		//   the npc attacking
		// so, we calculate the closest point that is ATTACK_DISTANCE away from
		//   the opponent's position (that is perpendicular to the player)
		Vector3 displacement = opponent.transform.position - transform.position;
		displacement.z = 0;

		if (displacement.sqrMagnitude > squaredVisionDist) {
			StartCoroutine(EndFight()); // if opponent too far, set null to terminate all fighting
			return;
		}

		// this means npc far enough to update dest
		if (Mathf.Abs(displacement.x) >= Mathf.Abs(displacement.y)) {
			displacement.y = 0;
		} else {
			displacement.x = 0;
		}
		// mag == x + y since either one is a non-zero and the other is 0
		Vector3 offset = displacement / Mathf.Abs(displacement.x + displacement.y) * ATTACK_DISTANCE;
		SetNewDestination(opponent.transform.position - offset);
	}

	protected virtual void Retaliate() {
		// need to face the correct direction otherwise attack will be missed
		if (visibleByCamera) {
			Vector3 displacement = opponent.transform.position - transform.position;
			if (Mathf.Abs(displacement.x) >= Mathf.Abs(displacement.y)) {
				if (displacement.x > 0) {
					Attack(AnimationDirection.Right, Constants.PLAYER_ONLY_LAYER);
				} else {
					Attack(AnimationDirection.Left, Constants.PLAYER_ONLY_LAYER);
				}
			} else {
				// if at 0, looks nice if npc facing back (attacking into screen)
				if (displacement.y >= 0) {
					Attack(AnimationDirection.Back, Constants.PLAYER_ONLY_LAYER);
				} else {
					Attack(AnimationDirection.Forward, Constants.PLAYER_ONLY_LAYER);
				}
			}
		}

		StartCoroutine(EndFight());
	}

	protected void StartRetaliateAnimator() {
		animator.SetLayerWeight(1, 1);
	}

	protected void EndRetaliateAnimator() {
		animator.SetLayerWeight(1, 0);
	}

	public bool IsFighting() {
		return fighting;
	}

	public override void Knockout() {
		base.Knockout();
		if (OnKnockout != null) {
			OnKnockout(this);
		}
		fighting = false;
		EventManager.instance.OnDefeatNPC(this);
		interactable.OnKnockout();
	}

	/// NAVIGATION ///

	protected virtual void NavStarted() {
		if (debugNav) {
			Debug.Log("nav started");
		}
		isMoving = true;
		canSearchForDest = false;
	}

	protected virtual void NavArrived() {
		if (debugNav) {
			Debug.Log("nav arrived");
		}
		if (fighting) {
			// if fighting and arrived and dest, attack the opponent
			Retaliate();
		} else {
			StartCoroutine(ArriveDelay());
		}
		isMoving = false;
	}

	protected void DestinationInvalid() {
		if (debugNav) {
			Debug.Log("nav invalid");
		}
		isMoving = false;
		canSearchForDest = true;
	}

	protected void SetNewDestination(Vector3 newDest) {
		destination = newDest;
		agent.SetDestination(destination);
		if (debugNav) {
			Debug.Log(gameObject.name + " setting dest: " + destination + " starting at " + transform.position);
		}
	}

	protected virtual void SetNextDestination() {
		Vector3 randomDest = agent.GetRandomValidDestination();

		// TODO: move this check() and this set() into agent
		// this check ensures the npc's travel is significant enough
		if ((randomDest - transform.position).sqrMagnitude >= closestDestinationSquared || 
				randomDest.z != transform.position.z) {
			SetNewDestination(randomDest);
		}
	}

	/// INVENTORY ///

	private void PopulateInventory() {
		int num = UnityEngine.Random.Range(0, inventory.GetCapacity());

		for (int i = 0; i < num; i++) {
			inventory.AddItem(ItemManager.instance.GetRandomItem());
		}
	}

	private void RefreshInventory() {
		inventory.RemoveAll();
		PopulateInventory();
	}

	public void SetAgentNav(Nav2D nav) {
		agent.polyNav = nav;
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
		this.visibleByCamera = GetFloor() == GameManager.instance.GetVisibleFloor();
		GetComponent<NPCInteractable>().SetEnabled(visibleByCamera);
		
		if (GetFloor() > GameManager.instance.GetVisibleFloor()) {
			GetComponent<SpriteRenderer>().enabled = false;
			animator.enabled = false;
		} else {
			GetComponent<SpriteRenderer>().enabled = true;
			animator.enabled = true;
		}
	}

	public Inventory GetInventory() {
		return inventory;
	}

	public Vector3 GetDestination() {
		return destination;
	}

	public string GetName() {
		return npcName;
	}

	public void SetName(string newName) {
		npcName = newName;
		inventory.SetName(newName + "'s Inventory");
	}

	private void AdjustFriendliness(int delta) {
		this.friendliness += delta;
		if (this.friendliness < 0) {
			this.friendliness = 0;
		} else if (this.friendliness > 100) {
			this.friendliness = 100;
		}
	}

	public int GetFriendliness() {
		return friendliness;
	}

	public bool WasSpawned() {
		return spawned;
	}

	public void SetSpawned(bool spawned) {
		this.spawned = spawned;
	}

	public override void Save()
    {
		if (!spawned && saveData) {
			NPCData data = new NPCData(this);
			GameManager.Save(data, base.filename);
		}
    }

   public override void Load()
    {
		base.Load();
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
		if (data == null) {
			return;
		}
		base.LoadFromData(data);
		
		ItemStashData inventoryData = data.inventoryData;
		inventory.LoadFromInventoryData(inventoryData);

		npcName = data.name;
		name = Constants.NPC_NAME + npcName;

		hasQuest = data.hasQuest;
		questActive = data.questActive;
		if (hasQuest && !questActive) {
			interactable.ShowQuestIcon();
		}

		friendliness = data.friendliness;

		destination = new Vector3(data.destX, data.destY, data.destZ);
		isMoving = data.isMoving;
		canSearchForDest = data.canSearchForDest;

		UpdateSortingLayer();

		if (isMoving) {
			SetNewDestination(destination);
		} else if (!canSearchForDest) {
			StartCoroutine(ArriveDelay());
		}
	}

	[System.Serializable]
	public class NPCData : Character.CharacterData {

		public ItemStashData inventoryData;
		public float destX;
		public float destY;
		public float destZ;

		public string name;
		public bool hasQuest;
		public bool questActive;
		public int friendliness;
		public bool isMoving;
		public bool canSearchForDest;
		public bool visible;

		public NPCData(NPC npc) : base(npc) {
			inventoryData = new ItemStashData(npc.GetInventory());
			destX = npc.GetDestination().x;
			destY = npc.GetDestination().y;
			destZ = npc.GetDestination().z;

			this.name = npc.GetName();
			this.hasQuest = npc.hasQuest;
			this.questActive = npc.questActive;
			this.friendliness = npc.GetFriendliness();
			this.isMoving = npc.isMoving;
			this.canSearchForDest = npc.canSearchForDest;
			this.visible = npc.visibleByCamera;
		}
	}
}