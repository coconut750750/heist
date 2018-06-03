using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nav2DStairs : MonoBehaviour {

	public Vector3[] points {
		get {
			List<Vector3> p = new List<Vector3>();

			foreach (Collider2D collider in GetComponentsInChildren<Collider2D>()) {
				p.Add(collider.transform.position);
			}
			return p.ToArray();
		}
	}

	public void Awake() {
		
	}
}
