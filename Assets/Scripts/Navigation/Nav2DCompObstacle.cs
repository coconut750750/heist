using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
[AddComponentMenu("Navigation/Nav2DCompObstacle")]
///Place on a game object to act as an obstacle
public class Nav2DCompObstacle : MonoBehaviour {

	///Inverts the polygon (done automatically if collider already exists due to a sprite)
	public bool invertPolygon = false;
	public float extraOffset;

	public CompositeCollider2D compCollider;

	private Vector3 lastPos;
	private Quaternion lastRot;
	private Vector3 lastScale;
	private Transform _transform;

	///The polygon points of the obstacle
	public Vector2[] points{
		get
		{
			var compCollider = (CompositeCollider2D)(this.compCollider);
			//invert the main polygon points so that we save checking for inward/outward later (for Inflate)
			var points = new List<Vector2>();
			for (int i = 0; i < compCollider.pathCount; ++i){
				Vector2[] pathPoints = new Vector2[compCollider.GetPathPointCount(i)];
				compCollider.GetPath(i, pathPoints);
				for (int p = 0; p < pathPoints.Length; ++p) {
					points.Add( pathPoints[p] );
				}
			}

			Vector2[] pointArr = points.ToArray();

			if (invertPolygon)
				System.Array.Reverse(pointArr);
			return pointArr;	
		}
	}

	public Vector2[][] polygonPoints {
		get {
			var obstaclePolys = new List<Vector2[]>();

			var points = new List<Vector2>();
			
			for (int i = 0; i < compCollider.pathCount; ++i){
				Vector2[] pathPoints = new Vector2[compCollider.GetPathPointCount(i)];
				compCollider.GetPath(i, pathPoints);
				foreach (Vector2 v in pathPoints) {
					points.Add(v);
				}
				
				if (invertPolygon)
					points.Reverse();
				
				obstaclePolys.Add(points.ToArray());
				points.Clear();
			}

			return obstaclePolys.ToArray();
		}
	}

	[SerializeField]
	private Nav2D polyNav;

	void Reset(){
		compCollider.isTrigger = true;
		if (GetComponent<SpriteRenderer>() != null)
			invertPolygon = true;
	}

	void OnEnable(){

		if (polyNav) {}
			//polyNav.AddObstacle(this);
		
		lastPos = transform.position;
		lastRot = transform.rotation;
		lastScale = transform.localScale;
		_transform = transform;
	}

	void OnDisable(){

		if (polyNav) {}
			//polyNav.RemoveObstacle(this);
	}

	void Update(){
		if (_transform.position != lastPos || _transform.rotation != lastRot || _transform.localScale != lastScale)
			polyNav.regenerateFlag = true;

		lastPos = _transform.position;
		lastRot = _transform.rotation;
		lastScale = _transform.localScale;
	}

	public void SetNav2D(Nav2D polyNav) {
		this.polyNav = polyNav;
	}
}
