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

	public new Collider2D collider;

	private Vector3 lastPos;
	private Quaternion lastRot;
	private Vector3 lastScale;
	private Transform _transform;

	///The polygon points of the obstacle
	public Vector2[] points{
		get {
			if (collider is BoxCollider2D){
				var box = (BoxCollider2D)collider;
				Vector2 pos = new Vector2(collider.transform.position.x, collider.transform.position.y);
				pos -= new Vector2(0.5f, 0.5f);
				var tl = pos + new Vector2(-box.size.x, box.size.y)/2;
				var tr = pos + new Vector2(box.size.x, box.size.y)/2;
				var br = pos + new Vector2(box.size.x, -box.size.y)/2;
				var bl = pos + new Vector2(-box.size.x, -box.size.y)/2;
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

	void Reset(){
		collider.isTrigger = true;
		if (GetComponent<SpriteRenderer>() != null)
			invertPolygon = true;
	}

	void OnEnable(){
		lastPos = transform.position;
		lastRot = transform.rotation;
		lastScale = transform.localScale;
		_transform = transform;
	}

	void OnDisable(){

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
