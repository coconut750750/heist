using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : NPC {

	public Building patrolBuilding;

	private List<Vector3> patrolPath;
	private int patrolIndex = 0;

	protected override void Start() {
		base.Start();

		Vector3[] groundBounding = patrolBuilding.groundBoundingBox;
		patrolPath = new List<Vector3>();
		patrolPath.AddRange(groundBounding);

		float shortest = float.MaxValue;
		for (int i = 0; i < patrolPath.Count; i++) {
			if ((transform.position - patrolPath[i]).magnitude < shortest) {
				patrolIndex = i;
			}
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
		patrolIndex = (patrolIndex + 1) % patrolPath.Count;
	}
}
