using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Navigation/Nav2DObstacle")]
///Place on a game object to act as an obstacle
public class Nav2DObstacle : MonoBehaviour {

	///Inverts the polygon (done automatically if collider already exists due to a sprite)
	public bool invertPolygon = false;
	public float extraOffset;

	private new Collider2D collider {
		get {
			return gameObject.GetComponent<Collider2D>();
		}
	}

	private Vector3 lastPos;
	private Quaternion lastRot;
	private Vector3 lastScale;
	private Transform _transform;

	///The polygon points of the obstacle
	private Vector3[] _points = null;
	public Vector3[] points{
		get {
			if (_points != null) {
				return _points;
			}

			if (collider == null) {
				_points = new Vector3[0];
				return _points;
			}

			if (collider is BoxCollider2D){
				BoxCollider2D box = (BoxCollider2D) collider;
				Vector3 tl = new Vector3(-box.size.x / 2, box.size.y / 2);
				Vector3 tr = new Vector3(box.size.x  / 2, box.size.y / 2);
				Vector3 br = new Vector3(box.size.x  / 2, -box.size.y / 2);
				Vector3 bl = new Vector3(-box.size.x  / 2, -box.size.y / 2);

				_points = new Vector3[] {tl, tr, br, bl};
				return _points;
			}

			if (collider is PolygonCollider2D){
				Vector2[] tempPoints = (collider as PolygonCollider2D).points;				
				if (invertPolygon)
					System.Array.Reverse(tempPoints);
				_points = new Vector3[tempPoints.Length];
				for (int i = 0; i < tempPoints.Length; i++) {
					_points[i] = new Vector3(tempPoints[i].x, tempPoints[i].y, 0);
				}
				return _points;			
			}

			return null;
		}
	}

	[SerializeField]
	private Nav2D polyNav;

	void Reset() {
		if (GetComponent<SpriteRenderer>() != null) {
			invertPolygon = true;
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
		return collider;
	}
}
