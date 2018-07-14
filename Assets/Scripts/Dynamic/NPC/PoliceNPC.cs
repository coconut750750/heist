using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : NPC {

	private List<Vector3> patrolPath;
	private int patrolIndex = 0;

	protected override void Start() {
		base.Start();
		
		patrolPath = new List<Vector3>();
		patrolPath.Add(new Vector3(-15, -3, 0));
		patrolPath.Add(new Vector3(-15, 17, 0));
		patrolPath.Add(new Vector3(16, 17, 0));
		patrolPath.Add(new Vector3(16, -3, 0));
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
