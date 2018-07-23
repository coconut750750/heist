using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Building : MonoBehaviour {

	[SerializeField]
	private GameObject groundFloor;

	[SerializeField]
	private GameObject secondFloor;

	[SerializeField]
	private Vector3[] innerPatrol;

	public bool drawGroundBounding;

	private Vector3[] _outerBoundingBox = null;
	public Vector3[] outerBoundingBox {
		get {
			if (_outerBoundingBox != null) {
				return _outerBoundingBox;
			}

			float left = int.MaxValue, top = int.MinValue, right = int.MinValue, bot = int.MaxValue;
			foreach (Vector3 point in GetAllPoints()) {
				left = point.x < left ? point.x : left;
				right = point.x > right ? point.x : right;
				bot = point.y < bot ? point.y : bot;
				top = point.y > top ? point.y : top;
			}
			left = Mathf.Floor(left); top = Mathf.Ceil(top); right = Mathf.Ceil(right); bot = Mathf.Floor(bot);
			_outerBoundingBox = new Vector3[4] {new Vector3(left, bot, 0f), new Vector3(left, top, 0f),
											     new Vector3(right, top, 0f), new Vector3(right, bot, 0f)};
			return _outerBoundingBox;
		}
	}

	private IEnumerable<Vector3> GetAllPoints() {
		List<Vector3> allPoints = new List<Vector3>();

		Nav2DObstacle[] obstacles = groundFloor.GetComponentsInChildren<Nav2DObstacle>();
		Nav2DCompObstacle[] compObstacles = groundFloor.GetComponentsInChildren<Nav2DCompObstacle>();

		foreach (Nav2DObstacle obstacle in obstacles) {
			foreach (Vector3 point in obstacle.points) {
				allPoints.Add(point);
			}
		}

		foreach (Nav2DCompObstacle comp in compObstacles) {
			foreach (Vector3[] polygon in comp.polygonPoints) {
				foreach (Vector3 point in polygon) {
					allPoints.Add(point);
				}
			}
		}

		return allPoints;
	}

	public Vector3[] GetOuterPatrol(Vector3 start) {
		return GetPathClosestTo(outerBoundingBox, start);
	}

	public Vector3[] GetInnerPatrol(Vector3 start) {
		return GetPathClosestTo(innerPatrol, start);
	}

	private Vector3[] GetPathClosestTo(Vector3[] path, Vector3 start) {
		int startIndex = 0;

		float shortest = float.MaxValue;
		for (int i = 0; i < path.Length; i++) {
			if ((start - path[i]).magnitude < shortest) {
				startIndex = i;
			}
		}

		int len = path.Length;
		Vector3[] newPath = new Vector3[len];
		for (int i = 0; i < len; i++) {
			newPath[i] = path[(i + startIndex) % len];
		}
		return newPath;
	}

	public void ShowFloor2() {
		SetSecondFloorActive(true);
		SetGroundFloorActive(false);
	}

	public void HideFloor2() {
		SetSecondFloorActive(false);
		SetGroundFloorActive(true);
	}

	public void SetGroundFloorActive (bool active) {
    	foreach (Collider2D c in groundFloor.GetComponentsInChildren<Collider2D>()) {
			if (!c.CompareTag(Constants.STAIRS_TAG)) {
				c.enabled = active;
			}
    	}
	}

	public void SetSecondFloorActive (bool active) {
		foreach (Collider2D c in secondFloor.GetComponentsInChildren<Collider2D>()) {
			if (!c.CompareTag(Constants.STAIRS_TAG)) {
				c.enabled = active;
			}
    	}
		secondFloor.SetActive(active);
	}

#if UNITY_EDITOR

    void OnDrawGizmos () {

		if (!drawGroundBounding) {
			return;
		}

		float nodeSize = 0.15f;

		DebugDrawPath(outerBoundingBox, nodeSize, Color.red);
		DebugDrawPath(innerPatrol, nodeSize, Color.red);
	}

	void DebugDrawPolygon(Vector3[] points, Color color) {
		for (int i = 0; i < points.Length; i++) {
			Debug.DrawLine(points[i], points[(i + 1) % points.Length], color);
		}
	}

	void DebugDrawPath(Vector3[] points, float nodeSize, Color color) {
		for (int i = 0; i < points.Length; i++) {
			Vector3 v = points[i];
			Vector3[] square = new Vector3[4] {v + new Vector3(-nodeSize, 0), v + new Vector3(0, nodeSize),
											   v + new Vector3(nodeSize, 0), v + new Vector3(0, -nodeSize)};

			DebugDrawPolygon(square, Color.red);
			Debug.DrawLine(v, points[(i + 1) % points.Length], Color.red);
		}
	}
#endif
}
