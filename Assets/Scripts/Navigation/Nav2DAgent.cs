using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Navigation/Nav2DAgent")]
public class Nav2DAgent : MonoBehaviour{

	[NonSerialized]
	public float maxSpeed			 = 5; // taken by the gameobject using nav2d agent
	///The object mass
	// public float mass                = 20; for smoother turns, add this back, then in lateupdate, vel += newvel/mass
	///The distance to stop at from the goal
	public float stoppingDistance    = 0.05f;
	///The distance to start slowing down
	public float slowingDistance     = 0f;
	///The rate at which it will slow down
	public float decelerationRate    = 10;
	///Rotate transform as well?
	public bool rotateTransform      = false;
	///Speed to rotate at
	public float rotateSpeed         = 350;
	///The avoidance radius of the agent. 0 for no avoidance	
	public float avoidRadius         = 0.5f;
	///The lookahead distance for Slowing down and agent avoidance. Set to 0 to eliminate the slowdown but the avoidance too, as well as increase performance.
	public float lookAheadDistance    = 1;
	///Should the agent repath? Disable for performance.
	public bool repath 				  = false;
	///Should the agent be forced restricted within valid areas? Disable for performance.
	public bool restrict 			  = false;
	///Will debug the path (gizmos). Disable for performance.
	public bool debugPath            = true;

	///Raised when a new destination is started after path found
	public event Action OnNavigationStarted;

	public event Action<Vector2> OnSetVelocity;
	///Raised when the destination is reached
	public event Action OnDestinationReached;
	///Raised when the destination is or becomes invalid
	public event Action OnDestinationInvalid;
	///Raised when a "corner" point has been reached while traversing the path
	public event Action<Vector2> OnNavigationPointReached;

	private event Action<bool> reachedCallback;

	private Rigidbody2D rb2d;
	
	private int requests              = 0;
	private List<Vector3> _activePath = new List<Vector3>();

	// private Vector2 velocity {
	// 	get {
	// 		return rb2d.velocity;
	// 	}
	// 	set {
	// 		rb2d.velocity = value;
	// 	}
	// }

	private Vector3 velocity = Vector3.zero;

	private bool paused {
		get {
			return gameObject.GetComponent<Character>().IsPaused() || GameManager.instance.IsPaused();
		}
	}
	
	///The current goal of the agent
	private Vector3 primeGoal = Vector3.zero;

	private static List<Nav2DAgent> allAgents = new List<Nav2DAgent>();

	///The position of the agent
	public Vector3 position{
		get {return transform.position;}
		set {transform.position = value;}
	}

	///The current active path of the agent
	public List<Vector3> activePath{
		get { 
			return _activePath; 
		}
		set
		{
			_activePath = value;
			if (_activePath.Count > 0)
				_activePath.RemoveAt(0);			
		}
	}

	///Is a path pending?
	public bool pathPending {
		get	{ return requests > 0;}
	}

	///The PolyNav object
	public Nav2D polyNav;

	///Does the agent has a path?
	public bool hasPath{
		get { return activePath.Count > 0;}
	}

	///The point that the agent is currenty going to. Returns the agent position if no active path
	public Vector3 nextPoint{
		get {
			if (hasPath)
				return activePath[0];
			return position;			
		}
	}

	///The remaining distance of the active path. 0 if none
	public float remainingDistance {
		get {
			if (!hasPath) {
				return 0;
			}
			float dist= Vector2.Distance(position, activePath[0]);
			for (int i= 0; i < activePath.Count; i++) {
				dist += Vector2.Distance(activePath[i], activePath[i == activePath.Count - 1 ? i : i + 1]);
			}
			return dist;
		}
	}

	///The moving direction of the agent
	public Vector2 movingDirection {
		get {
			if (hasPath) {
				return velocity.normalized;
			}
			return Vector2.zero;
		}
	}

	void OnEnable() { allAgents.Add(this); }
	void OnDisable() { allAgents.Remove(this); }
	
	void Awake(){ 
		rb2d = gameObject.GetComponent<Rigidbody2D>();
	}

	public Vector3 GetRandomValidDestination() {
		return polyNav.GetRandomValidDestination();
	}

	///Set the destination for the agent. As a result the agent starts moving
	public bool SetDestination(Vector3 goal){
		return SetDestination(goal, null);
	}

	///Set the destination for the agent. As a result the agent starts moving. Only the callback from the last SetDestination will be called upon arrival
	public bool SetDestination(Vector3 goal, Action<bool> callback){
		if (polyNav == null){
			Debug.LogError("No Nav2D in scene!");
			return false;
		}

		//goal is almost the same as the last goal. Nothing happens for performace in case it's called frequently
		if ((goal - primeGoal).sqrMagnitude < Mathf.Epsilon && primeGoal.z == goal.z) {
			return true;
		}

		reachedCallback = callback;
		primeGoal = goal;

		//goal is almost the same as agent position. We consider arrived immediately
		//also ensure that goal and position are on same floor
		if ((goal - position).sqrMagnitude < stoppingDistance && goal.z == position.z){
			OnArrived();
			return true;
		}

		//check if goal is valid
		if (!polyNav.PointIsValid(goal)) {
			// Go to closer point if requested destination is invalid
			return SetDestination(polyNav.GetCloserEdgePoint(goal), callback);
		}

		//if a path is pending dont calculate new path
		//the prime goal will be repathed anyway
		if (requests > 0)
			return true;

		//compute path
		requests++;
		polyNav.FindPath(position, goal, SetPath);

		return true;
	}

	///Clears the path and as a result the agent is stop moving
	public void Stop(){
		activePath.Clear();
		velocity = Vector3.zero;
		OnSetVelocity(velocity);

		requests = 0;
		primeGoal = position;
	}

	//the callback from polyNav for when path is ready to use
	void SetPath(Vector3[] path, bool success){
		//in case the agent stoped somehow, but a path was pending
		if (requests == 0) {
			return;
		}

		requests--;

		if (!success || path.Length == 0) {
			OnInvalid();
			return;
		}

		activePath = path.ToList();
		if (OnNavigationStarted != null) {
			OnNavigationStarted();
		}
	}

	//main loop
	void FixedUpdate(){
		if (polyNav == null) {
			return;
		}
		//when there is no path just restrict
		if (!hasPath) {
			Restrict();
			return;
		}

		if (paused) {
			rb2d.velocity = Vector2.zero;
			return;
		}

		//calculate velocities
		if (remainingDistance < slowingDistance) {
			velocity = Arrive(nextPoint);
		} else {
			velocity = Seek(nextPoint);
		}

		OnSetVelocity(movingDirection);

		//slow down if wall ahead
		LookAhead();

		//restrict just after movement
		Restrict();

		//rotate if must
		if (rotateTransform) {
			float rot = -Mathf.Atan2(movingDirection.x, movingDirection.y) * 180 / Mathf.PI;
			float newZ = Mathf.MoveTowardsAngle(transform.localEulerAngles.z, rot, rotateSpeed * Time.deltaTime);
			transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, newZ);
		}

		if (repath) {
			//repath if there is no LOS with the next point
			if (polyNav.CheckLineOfSight(position, nextPoint) == false) {
				Repath();
			}
			//in case just after repath-ing there is no path
			if (!hasPath) {
				OnArrived();
				return;
			}
		}

		//Check and remove if we reached a point. 
		if (((Vector2)position - (Vector2)nextPoint).sqrMagnitude <= stoppingDistance) {
			activePath.RemoveAt(0);

			//if it was last point, means the path is complete and no longer have an active path.
			if (!hasPath) {
				
				OnArrived();
				return;
			} else {
				if (repath){
					//repath after a point is reached
					Repath();
				}

				if (OnNavigationPointReached != null) {
					OnNavigationPointReached(position);
				}
			}
		}

		//little trick. Check the next waypoint ahead of the current for LOS and if true consider the current reached.
		//helps for tight corners and when agent has big innertia
		// if (activePath.Count > 1 && polyNav.CheckLOS(position, activePath[1])){
		// 	activePath.RemoveAt(0);
		// 	if (OnNavigationPointReached != null)
		// 		OnNavigationPointReached(position);
		// }
	}


	//recalculate path to prime goal if there is no pending requests
	void Repath() {
		if (requests > 0) {
			return;
		}

		requests++;
		polyNav.FindPath(position, primeGoal, SetPath);
	}

	//stop the agent and callback + message
	void OnArrived() {
		Stop();

		if (reachedCallback != null) {
			reachedCallback(true);
		}
		if (OnDestinationReached != null) {
			OnDestinationReached();
		}
	}

	//stop the agent and callback + message
	void OnInvalid(){
		Stop();

		if (reachedCallback != null) {
			reachedCallback(false);
		}
		if (OnDestinationInvalid != null) {
			OnDestinationInvalid();
		}
	}

	
	//seeking a target
	Vector2 Seek(Vector3 pos){
		Vector2 desiredVelocity = (pos - position).normalized * maxSpeed;
		// Vector2 steer = desiredVelocity - velocity;
		// steer = Truncate(steer, maxSpeed);
		return desiredVelocity;
	}

	//slowing at target's arrival
	Vector2 Arrive(Vector3 pos){

		Vector3 desiredVelocity = (pos - position);
		float dist = desiredVelocity.magnitude;

		if (dist > 0) {
			float reqSpeed = dist / (decelerationRate);
			reqSpeed = Mathf.Min(reqSpeed, maxSpeed);
			desiredVelocity *= reqSpeed / dist;
		}

		Vector2 steer = desiredVelocity - velocity;

		return steer;
	}

	//slowing when there is an obstacle ahead.
	void LookAhead(){

		if (lookAheadDistance <= 0)
			return;

		float currentLookAheadDistance = Mathf.Lerp(0, lookAheadDistance, velocity.magnitude/maxSpeed);
		Vector3 lookAheadPos = position + velocity.normalized * currentLookAheadDistance;

		Debug.DrawLine(position, lookAheadPos, Color.blue);

		if (!polyNav.PointIsValid(lookAheadPos))
			velocity -= (lookAheadPos - position);

		if (avoidRadius > 0){
			
			for (int i = 0; i < allAgents.Count; i++){
				Nav2DAgent otherAgent = allAgents[i];
				if (otherAgent == this || otherAgent.avoidRadius <= 0)
					continue;

				float mlt = otherAgent.avoidRadius + this.avoidRadius;
				float dist = (lookAheadPos - otherAgent.position).magnitude;
				Vector3 str = (lookAheadPos - otherAgent.position).normalized * mlt;
				Vector3 steer = Vector3.Lerp( (Vector3)str, Vector3.zero, dist/mlt);
				velocity += new Vector3(steer.x, steer.y, 0);

				Debug.DrawLine(otherAgent.position, otherAgent.position + str, new Color(1,0,0,0.1f));
			}
		}
	}

	//keep agent within valid area
	void Restrict(){
		if (!restrict) {
			return;
		}

		if (!polyNav.PointIsValid(position)) {
			position = polyNav.GetCloserEdgePoint(position);
		}
	}
    
    //limit the magnitude of a vector
    Vector2 Truncate(Vector2 vec, float max){
        if (vec.magnitude > max) {
            vec.Normalize();
            vec *= max;
        }
        return vec;
    }


    ////////////////////////////////////////
    ///////////GUI AND EDITOR STUFF/////////
    ////////////////////////////////////////
    
    void OnDrawGizmos(){

    	Gizmos.color = new Color(1,1,1,0.1f);
    	Gizmos.DrawWireSphere(transform.position, avoidRadius);

    	if (!hasPath)
    		return;

		Gizmos.color = new Color(0.2f, 1f, 0.2f, 1f);
		Gizmos.DrawLine(position, activePath[0]);
		for (int i= 0; i < activePath.Count; i++) {
			Gizmos.DrawLine(activePath[i], activePath[(i == activePath.Count - 1)? i : i + 1]);
		}
    }
}
