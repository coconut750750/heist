using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nav2DStairs : MonoBehaviour {

	private List<Collider2D> stairColliders;

	private List<Vector3> points;

	public void Start() {
		stairColliders = new List<Collider2D>();
		points = new List<Vector3>();

		foreach (Collider2D collider in GetComponentsInChildren<Collider2D>()) {
			stairColliders.Add(collider);
			points.Add(new Vector3());
		}

		

	}
}
