using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
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
	public Vector2[] points{
		get {
			if (collider == null) {
				return new Vector2[0];
			}
			if (collider is BoxCollider2D){
				BoxCollider2D box = (BoxCollider2D) collider;
				Vector2 pos = Vector2.zero;//new Vector2(transform.position.x, transform.position.y);
				Vector2 tl = pos + new Vector2(-box.size.x, box.size.y)/2;
				Vector2 tr = pos + new Vector2(box.size.x, box.size.y)/2;
				Vector2 br = pos + new Vector2(box.size.x, -box.size.y)/2;
				Vector2 bl = pos + new Vector2(-box.size.x, -box.size.y)/2;
				return new Vector2[]{tl, tr, br, bl};
			}

			if (collider is PolygonCollider2D){
				Vector2[] tempPoints = (collider as PolygonCollider2D).points;				
				if (invertPolygon)
					System.Array.Reverse(tempPoints);
				return tempPoints;			
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

	void Awake() {
		this.polyNav.AddObstacle(this);
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
