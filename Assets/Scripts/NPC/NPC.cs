using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MovingObject {

	private const string PLAYER_TAG = "Player";

	private Inventory inventory;
	private int money;
	public Vector2 destination;

	private Nav2DAgent agent;

	protected override void Start () {
		base.Start();
		inventory = gameObject.GetComponent<Inventory>();

		agent = gameObject.GetComponent<Nav2DAgent>();
		
		agent.OnDestinationReached += NavArrived;
		agent.OnNavigationStarted += NavStarted;
		agent.maxSpeed = moveSpeed;

		Debug.Log(agent.SetDestination(destination));

		UpdateSortingLayer();
	}

	protected override void FixedUpdate() {
		UpdateAnimator(new Vector3(rb2D.velocity.x, rb2D.velocity.y, 0));
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

	protected void NavStarted() {
		Debug.Log("started");
	}

	protected void NavArrived() {
		Debug.Log("arrived");
	}

    public override void Load()
    {
        
    }

    public override void Save()
    {
        
    }
}
