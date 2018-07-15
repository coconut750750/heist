using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

	[SerializeField]
	private GameObject groundFloor;

	[SerializeField]
	private GameObject secondFloor;

	public bool drawGroundBounding;

	private Vector3[] _groundBoundingBox = null;
	public Vector3[] groundBoundingBox {
		get {
			if (_groundBoundingBox != null) {
				return _groundBoundingBox;
			}

			float left = int.MaxValue, top = int.MinValue, right = int.MinValue, bot = int.MaxValue;
			foreach (Vector3 point in GetAllPoints()) {
				left = point.x < left ? point.x : left;
				right = point.x > right ? point.x : right;
				bot = point.y < bot ? point.y : bot;
				top = point.y > top ? point.y : top;
			}
			left = Mathf.Floor(left); top = Mathf.Ceil(top); right = Mathf.Ceil(right); bot = Mathf.Floor(bot);
			_groundBoundingBox = new Vector3[4] {new Vector3(left, bot, 0f), new Vector3(left, top, 0f),
											     new Vector3(right, top, 0f), new Vector3(right, bot, 0f)};
			return _groundBoundingBox;
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
		Color white = new Color(1, 1f, 1f, 1f);

		foreach (Vector3 v in groundBoundingBox) {
			Vector3[] square = new Vector3[4];
			square[0] = v + new Vector3(-nodeSize, 0);
			square[1] = v + new Vector3(0, nodeSize);
			square[2] = v + new Vector3(nodeSize, 0);
			square[3] = v + new Vector3(0, -nodeSize);

			DebugDrawPolygon(square, white);
		}
	}

	//helper debug function
	void DebugDrawPolygon(Vector3[] points, Color color) {
		for (int i = 0; i < points.Length; i++) {
			Debug.DrawLine(points[i], points[(i + 1) % points.Length], color);
		}
	}
#endif
}
