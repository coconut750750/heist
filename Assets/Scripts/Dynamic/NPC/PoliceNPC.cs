using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : NPC {

	public Building patrolBuilding;
	
	enum PatrolType {
		Outer, Inner
	}

	[SerializeField]
	private PatrolType patrolType;

	private Vector3[] patrolPath;
	private int patrolIndex = 0;

	protected override void Start() {
		base.Start();
		
		switch (patrolType) {
			case PatrolType.Inner:
				patrolPath = patrolBuilding.GetInnerPatrol(transform.position);
				break;
			default:
				patrolPath = patrolBuilding.GetOuterPatrol(transform.position);
				break;
		}
	}

	protected override void SetNextDestination() {
		Vector3 randomDest = patrolPath[patrolIndex];
		SetNewDestination(randomDest);
	}

	protected override void NavStarted() {
		base.NavStarted();
	}

	protected override void NavArrived() {
		base.NavArrived();
		patrolIndex = (patrolIndex + 1) % patrolPath.Length;
	}
}
