using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Navigation/Nav2DCompObstacle")]
///Place on a game object to act as an obstacle
public class Nav2DCompObstacle : MonoBehaviour {

	public float extraOffset;

	private CompositeCollider2D compCollider {
		get {
			return gameObject.GetComponent<CompositeCollider2D>();
		}
	}

	[SerializeField]
	private Nav2D polyNav;

	private Vector3 lastPos;
	private Quaternion lastRot;
	private Vector3 lastScale;
	private Transform _transform;

	///The polygon points of the obstacle
	public Vector3[][] polygonPoints {
		get {
			var obstaclePolys = new List<Vector3[]>();

			var points = new List<Vector3>();
			
			for (int i = 0; i < compCollider.pathCount; ++i){
				Vector2[] pathPoints = new Vector2[compCollider.GetPathPointCount(i)];
				compCollider.GetPath(i, pathPoints);
				foreach (Vector2 v in pathPoints) {
					points.Add(v);
				}
				
				points.Reverse();
				
				obstaclePolys.Add(points.ToArray());
				points.Clear();
			}

			return obstaclePolys.ToArray();
		}
	}

	void OnEnable(){
		lastPos = transform.position;
		lastRot = transform.rotation;
		lastScale = transform.localScale;
		_transform = transform;
	}

	void Update(){
		if (_transform.position != lastPos || _transform.rotation != lastRot || _transform.localScale != lastScale) {
			polyNav.regenerateFlag = true;
		}

		lastPos = _transform.position;
		lastRot = _transform.rotation;
		lastScale = _transform.localScale;
	}

	public Collider2D GetCollider() {
		return compCollider;
	}
}
