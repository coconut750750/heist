using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MovingObject {

	private Inventory inventory;
	private int money;
	public Vector2 destination;

	private Nav2DAgent agent;

	protected override void Start () {
		base.Start();
		inventory = gameObject.GetComponent<Inventory>();

		agent = gameObject.GetComponent<Nav2DAgent>();
		
		agent.OnDestinationReached += Arrived;
		agent.OnNavigationStarted += Started;
		agent.maxSpeed = moveSpeed;

		Debug.Log(agent.SetDestination(destination));
	}

	protected override void FixedUpdate() {
		UpdateAnimator(new Vector3(agent.movingDirection.x, agent.movingDirection.y, 0));
	}

	protected void Started() {
		Debug.Log("started");
	}

	protected void Arrived() {
		Debug.Log("arrived");
	}

    public override void Load()
    {
        
    }

    public override void Save()
    {
        
    }
}
