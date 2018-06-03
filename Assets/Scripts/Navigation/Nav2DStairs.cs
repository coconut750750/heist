using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nav2DStairs : MonoBehaviour {

	public Vector3[] points {
		get {
			List<Vector3> p = new List<Vector3>();

			foreach (Transform transform in GetComponentsInChildren<Transform>()) {
				p.Add(transform.position);
			}
			return p.ToArray();
		}
	}

	public void Awake() {
		
	}
}
