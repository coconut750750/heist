using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceNPC : NPC {

	public Building patrolBuilding;
	
	public enum PatrolType {
		Outer, Inner
	}

	[SerializeField]
	private PatrolType patrolType;

	private Vector3[] patrolPath;
	private int patrolIndex = 0;

	protected bool isSuspicious = false;

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
		// TODO: remove
		base.SetStrength(1);

		squaredVisionDist = 400;

		SetPatrol(patrolBuilding, patrolType);
	}

	protected override void FixedUpdate() {
		base.FixedUpdate();
		UpdateSuspicion();
	}

	protected override void Retaliate() {
		if (canAttack) {
			base.Retaliate();
		}
	}

	// if police is suspicious then loses sight of player, still suspicious
	protected void UpdateSuspicion() {
		if (GameManager.instance.mainPlayer.suspicion == 0) {
			isSuspicious = false;
		} else if (GameManager.instance.CanSeePlayer(transform.position, squaredVisionDist)) {
			isSuspicious = true;
		}
	}

	public override void Knockout() {
		base.Knockout();
	}

	protected override void NavStarted() {
		base.NavStarted();
	}

	protected override void NavArrived() {
		base.NavArrived();
		if (!fighting) {
			patrolIndex = (patrolIndex + 1) % patrolPath.Length;
		}
	}

	protected void SetPatrol(Building building, PatrolType patrolType) {
		this.patrolBuilding = building;
		switch (patrolType) {
			case PatrolType.Inner:
				patrolPath = building.GetInnerPatrol(transform.position);
				return;
			default:
				patrolPath = building.GetOuterPatrol(transform.position);
				return;
		}
	}

	protected override void SetNextDestination() {
		Vector3 patrolDest = patrolPath[patrolIndex];
		SetNewDestination(patrolDest);
	}
}
