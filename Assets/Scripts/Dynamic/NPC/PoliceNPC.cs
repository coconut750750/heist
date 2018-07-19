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

	// TODO: make this variable based on attack speed
	private const float ATTACK_DELAY_SECONDS = 0.5f;
	private bool canAttack = true;

	protected override IEnumerator EndFight() {
		if (opponent == null || opponent.GetHealth() <= 0) {
			yield return base.EndFight();
		}
		canAttack = false;
		yield return new WaitForSeconds(AFTER_ATTACK_DELAY);
		canAttack = true;
	}

	protected override void Start() {
		base.Start();

		squaredStopRetaliateDist = 1000;
		
		switch (patrolType) {
			case PatrolType.Inner:
				patrolPath = patrolBuilding.GetInnerPatrol(transform.position);
				break;
			default:
				patrolPath = patrolBuilding.GetOuterPatrol(transform.position);
				break;
		}
	}

	protected override void Retaliate() {
		if (canAttack) {
			base.Retaliate();
		}
	}

	protected override void Knockout() {
		base.Knockout();
	}

	protected override void NavStarted() {
		base.NavStarted();
	}

	protected override void NavArrived() {
		base.NavArrived();
		patrolIndex = (patrolIndex + 1) % patrolPath.Length;
	}

	protected override void SetNextDestination() {
		Vector3 randomDest = patrolPath[patrolIndex];
		SetNewDestination(randomDest);
	}
}
