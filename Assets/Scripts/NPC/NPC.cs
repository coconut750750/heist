﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MovingObject {

	public float newDestinationDelay;

	private const string PLAYER_TAG = "Player";

	private Inventory inventory;
	private int money;
	
	private Nav2DAgent agent {
		get {
			return gameObject.GetComponent<Nav2DAgent>();
		}
	}
	private Vector2 destination;
	private bool isMoving;

	private bool searchForDest;

	IEnumerator ArriveDelay() {
		searchForDest = false;
		yield return new WaitForSeconds(newDestinationDelay);
		searchForDest = true;
	}

	protected override void Start () {
		base.Start();
		//inventory = gameObject.GetComponent<Inventory>();
		
		agent.OnDestinationReached += NavArrived;
		agent.OnNavigationStarted += NavStarted;
		agent.OnDestinationInvalid += DestinationInvalid;
		agent.maxSpeed = moveSpeed;

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
				agent.SetDestination(newDest);
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

	protected Vector2 GenerateRandomDest(Bounds bound) {
		float minX = bound.min.x;
		float minY = bound.min.y;
		float maxX = bound.max.x;
		float maxY = bound.max.y;

		float x = Random.Range(minX, maxX);
		float y = Random.Range(minY, maxY);

		return new Vector2((int)x, (int)y);
	}

	public void SetAgentNav(Nav2D nav) {
		agent.polyNav = nav;
	}

	protected void NavStarted() {
		Debug.Log("started");
		isMoving = true;
	}

	protected void NavArrived() {
		Debug.Log("arrived");
		isMoving = false;
		StartCoroutine(ArriveDelay());
	}

	protected void DestinationInvalid() {
		Debug.Log("invalid dest");
		isMoving = false;
	}

    public override void Load()
    {
        
    }

    public override void Save()
    {
        
    }
}
