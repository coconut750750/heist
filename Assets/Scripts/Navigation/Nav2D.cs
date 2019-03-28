using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

[AddComponentMenu("Navigation/Nav2D")]
public class Nav2D : MonoBehaviour {

	public bool debug = false;

	[SerializeField]
	private GameObject gameMap;

	public bool drawNodes;
	public bool drawNodesAndEdges;

	///The list of obstacles for the navigation
	private List<Nav2DObstacle> navObstacles = new List<Nav2DObstacle>();
	private List<Nav2DCompObstacle> navCompObstacles = new List<Nav2DCompObstacle>();
	private List<Nav2DStairs> navStairs = new List<Nav2DStairs>();

	///The radius from the edges to offset the agents.
	public float inflateRadius = 0.5f;

	///A Flag to tell PolyNav to regenerate the map
	public bool regenerateFlag = false;

	private PolyMap map;
	private List<PathNode> nodes = new List<PathNode>();
	private List<StairNode> stairNodes = new List<StairNode>();
	
	private List<Vector3> validDestinations = new List<Vector3>();

	private Queue<PathRequest> pathRequests = new Queue<PathRequest>();
	private PathRequest currentRequest;
	private bool isProcessingPath;
	private PathNode startNode;
	private PathNode endNode;

	private Collider2D masterCollider;
	[NonSerialized]
	public Bounds masterBounds;

	//some initializing
	void Awake() {
		navObstacles.AddRange(gameMap.GetComponentsInChildren<Nav2DObstacle>().ToList());
		navCompObstacles.AddRange(gameMap.GetComponentsInChildren<Nav2DCompObstacle>().ToList());
		navStairs.AddRange(gameMap.GetComponentsInChildren<Nav2DStairs>().ToList());

		masterCollider = GetComponent<Collider2D>();
		masterBounds = masterCollider.bounds;
		masterCollider.enabled = false;

		GenerateMap(true);
	}

	void LateUpdate() {
		if (regenerateFlag) {
			regenerateFlag = false;
			GenerateMap(false);
		}
	}

	///Find a path 'from' and 'to', providing a callback for when path is ready containing the path.
	public void FindPath(Vector3 start, Vector3 end, System.Action<Vector3[], bool> callback) {
		if (CheckLineOfSight(start, end)) {
			callback( new Vector3[]{start, end}, true );
			return;
		}

		pathRequests.Enqueue(new PathRequest(start, end, callback));
		TryNextFindPath();
	}

	//Pathfind next request
	void TryNextFindPath() {
		if (isProcessingPath || pathRequests.Count <= 0) {
			return;
		}
		currentRequest = pathRequests.Dequeue();
		isProcessingPath = true;

		if (!PointIsValid(currentRequest.start)) {
			currentRequest.start = GetCloserEdgePoint(currentRequest.start);
		}

		//create start & end as temp nodes
		startNode = new PathNode(currentRequest.start);
		endNode = new PathNode(currentRequest.end);

		List<PathNode> newNodes = new List<PathNode>();
		foreach (PathNode p in nodes) {
			newNodes.Add(p);
		}

		newNodes.Add(startNode);
		LinkStart(startNode, newNodes);

		newNodes.Add(endNode);
		LinkEnd(endNode, newNodes);

		StartCoroutine(AStar.CalculatePath(startNode, endNode, newNodes.ToArray(), stairNodes.ToArray(), RequestDone));
	}

	//Pathfind request finished (path found or not)
	// delete start and end node
	// destroy links between end node and other nodes
	void RequestDone(Vector3[] path, bool success) {
		DeleteExternalLinks();
		isProcessingPath = false;
		currentRequest.callback(path, success);
		TryNextFindPath();
	}

	public void GenerateMap(bool generateMaster) {
		CreatePolyMap(generateMaster);
		CreateAndCleanNodes();

		// since stairs are non-negotiable, create them after cleaning up nodes
		CreateAndLinkStairNodes();
		LinkNodes(nodes);
	}

	Vector3[] TransformPoints ( Vector3[] points, Transform t ) {
		for (int i = 0; i < points.Length; i++) {
			points[i] = t.TransformPoint(points[i]);
		}
		return points;
	}

	//takes all colliders points and convert them to usable stuff
	void CreatePolyMap(bool generateMaster) {

		List<Polygon> masterPolys = new List<Polygon>();
		List<Polygon> obstaclePolys = new List<Polygon>();

		//create a polygon object for each obstacle
		for (int i = 0; i < navObstacles.Count; i++) {
			Nav2DObstacle obstacle = navObstacles[i];
			Vector3[] transformedPoints = TransformPoints(obstacle.points, obstacle.transform);
			Vector3[] inflatedPoints = InflatePolygon(transformedPoints, Mathf.Max(0.01f, inflateRadius + obstacle.extraOffset) );
			obstaclePolys.Add(new Polygon(inflatedPoints));
		}

		//create polygon objects for each composite obstacle
		foreach (Nav2DCompObstacle compObstacle in navCompObstacles) {
			foreach (Vector3[] p in compObstacle.polygonPoints) {
				Vector3[] transformedPoints = TransformPoints(p, compObstacle.transform);
				Vector3[] inflatedPoints = InflatePolygon(transformedPoints, Mathf.Max(0.01f, inflateRadius + compObstacle.extraOffset) );
				obstaclePolys.Add(new Polygon(inflatedPoints));
			}
		}		

		// TODO: assign max and min when master collider is polygon collider
		Vector2 max = Vector2.zero;
		Vector2 min = Vector2.zero;
		if (generateMaster) {
			if (masterCollider is PolygonCollider2D) {
				PolygonCollider2D polyCollider = (PolygonCollider2D)masterCollider;
				//invert the main polygon points so that we save checking for inward/outward later (for Inflate)
				List<Vector3> reversed = new List<Vector3>();
				
				for (int i = 0; i < polyCollider.pathCount; ++i) {

					for (int p = 0; p < polyCollider.GetPath(i).Length; ++p)
						reversed.Add( polyCollider.GetPath(i)[p] );
					
					reversed.Reverse();

					Vector3[] transformed = TransformPoints(reversed.ToArray(), polyCollider.transform);
					Vector3[] inflated = InflatePolygon(transformed, Mathf.Max(0.01f, inflateRadius));
				
					masterPolys.Add(new Polygon(inflated));
					reversed.Clear();
				}

			} else if (masterCollider is BoxCollider2D) {
				BoxCollider2D box = (BoxCollider2D)masterCollider;
				Vector2 tl = box.offset + new Vector2(-box.size.x, box.size.y)/2;
				Vector2 tr = box.offset + new Vector2(box.size.x, box.size.y)/2;
				Vector2 br = box.offset + new Vector2(box.size.x, -box.size.y)/2;
				Vector2 bl = box.offset + new Vector2(-box.size.x, -box.size.y)/2;
				Vector3[] transformed = TransformPoints(new Vector3[]{tl, bl, br, tr}, masterCollider.transform);
				Vector3[] inflated = InflatePolygon(transformed, Mathf.Max(0.01f, inflateRadius));
				masterPolys.Add(new Polygon(inflated));

				max = new Vector2(Mathf.Round(tr.x), Mathf.Round(tr.y));
				min = new Vector2(Mathf.Round(bl.x), Mathf.Round(bl.y));
			}
		
		} else {
			masterPolys = map.masterPolygons.ToList();
		}

		//create the main polygon map (based on inverted) also containing the obstacle polygons
		map = new PolyMap(masterPolys.ToArray(), obstaclePolys.ToArray());

		GenerateValidDestinationsInRange(max, min, -0.1f);

		//The colliders are never used again after this point. They are simply a drawing method.
	}

	private void GenerateValidDestinationsInRange(Vector2 max, Vector2 min, float minFloor) {
		for (float x = min.x; x <= max.x; x++) {
			for (float y = min.y; y <= max.y; y++) {
				for (float z = 0; z >= minFloor; z -= 0.1f) {
					Vector3 point = new Vector3(x, y, z);
					if (PointIsValid(point)) {
						validDestinations.Add(point);
					}
				}
			}
		}
	}

	//Link stairs together
	private void CreateAndLinkStairNodes() {
		// adding stair nodes after deleting surrounded
		foreach (Nav2DStairs stairs in navStairs) {
			foreach (Vector3 point in stairs.points) {
				Vector3 rounded = new Vector3(Mathf.Round(point.x), Mathf.Round(point.y), point.z);
				StairNode node = new StairNode(rounded);
				nodes.Add(node);
				stairNodes.Add(node);
			}
		}

		for (int a = 0; a < stairNodes.Count; a++) {
			for (int b = a + 1; b < stairNodes.Count; b++) {
				StairNode nodeA = stairNodes[a];
				StairNode nodeB = stairNodes[b];
				
				if (nodeA.pos.x == nodeB.pos.x && nodeA.pos.y == nodeB.pos.y) {
					stairNodes[a].neighborStair = nodeB;
					stairNodes[b].neighborStair = nodeA;
				}
			}
		}
	}

	//Create Nodes at convex points (since master poly is inverted, it will be concave for it) if they are valid
	void CreateAndCleanNodes() {
		nodes.Clear();

		List<Vector3> alreadyAdded = new List<Vector3>();

		for (int p = 0; p < map.allPolygons.Length; p++) {
			Polygon poly = map.allPolygons[p];
			//Inflate even more for nodes, by a marginal value to allow CheckLineOfSight between them

			Vector3[] inflatedPoints = InflatePolygon(poly.points, inflateRadius);
			for (int i = 0; i < inflatedPoints.Length; i++) {
				//if point is concave dont create a node
				if (PointIsConcave(inflatedPoints, i)) {
					continue;
				}
				
				//round the position vector to ensure objects fit through doors
				//also need to round to allow CheckLineOfSight() between them
				Vector3 rounded = new Vector3(Mathf.Round(inflatedPoints[i].x), Mathf.Round(inflatedPoints[i].y), inflatedPoints[i].z);				
				
				//if point is not in valid area dont create a node
				if (!PointIsValid(rounded)) {
					continue;
				}				

				// if point is already added, do not create a node
				if (!alreadyAdded.Contains(rounded)) {
					alreadyAdded.Add(rounded);
					nodes.Add(new PathNode(rounded));
				}		
			}
		}

		DeleteSurrounded(alreadyAdded);
	}

	//link the nodes provided
	void LinkNodes(List<PathNode> nodeList) {
		for (int i = 0; i < nodeList.Count; i++) {
			nodeList[i].links.Clear();
		}

		for (int a = 0; a < nodeList.Count; a++) {
			for (int b = a + 1; b < nodeList.Count; b++) {
				PathNode nodeA = nodeList[a];
				PathNode nodeB = nodeList[b];
				
				if (CheckLineOfSight(nodeA.pos, nodeB.pos)) {
					nodeList[a].links.Add(nodeB);
					nodeList[b].links.Add(nodeA);
				}
			}
		}

		DeleteLoneNodes();
	}	
	
	//Link the start node in
	void LinkStart(PathNode start, List<PathNode> toNodes) {
		for (int i = 0; i < toNodes.Count; i++) {
			if (CheckLineOfSight(start.pos, toNodes[i].pos)) {
				start.links.Add(toNodes[i]);
				toNodes[i].links.Add(start);
			}			
		}
	}

	//Link the end node in
	void LinkEnd(PathNode end, List<PathNode> toNodes) {
		for (int i = 0; i < toNodes.Count; i++) {
			if (CheckLineOfSight(end.pos, toNodes[i].pos)) {
				toNodes[i].links.Add(end);
				end.links.Add(toNodes[i]);
			}			
		}
	}

	//delete links to nodes that were removed
	void DeleteExternalLinks() {
		for (int i = 0; i < nodes.Count; i++) {
			List<PathNode> freshLinks = new List<PathNode>(nodes[i].links);
			foreach (PathNode link in nodes[i].links) {
				if (!nodes.Contains(link)) {
					freshLinks.Remove(link);
				}
			}
			nodes[i].links = freshLinks;
		}
	}

	// delete nodes that are completely surrounded by other nodes and obstacles
	// run after creating nodes
	void DeleteSurrounded(List<Vector3> addedNodeCoords) {
		List<PathNode> cleanedNodes = new List<PathNode>();

		for (int n = 0; n < nodes.Count; n++) {
			Vector3 left = nodes[n].pos + new Vector3(-1, 0);
			Vector3 top = nodes[n].pos + new Vector3(0, 1);
			Vector3 right = nodes[n].pos + new Vector3(1, 0);
			Vector3 down = nodes[n].pos + new Vector3(0, -1);

			bool leftBlocked = addedNodeCoords.Contains(left) || !PointIsValid(left);
			bool topBlocked = addedNodeCoords.Contains(top) || !PointIsValid(top);
			bool rightBlocked = addedNodeCoords.Contains(right) || !PointIsValid(right);
			bool downBlocked = addedNodeCoords.Contains(down) || !PointIsValid(down);

			if (!leftBlocked || !topBlocked || !rightBlocked || !downBlocked) {
				cleanedNodes.Add(nodes[n]);
			}
		}

		nodes = cleanedNodes;
	}

	// deletes nodes with no links
	// run after linking nodes
	void DeleteLoneNodes() {
		List<PathNode> nonLoneNodes = new List<PathNode>();
		foreach (PathNode p in nodes) {
			if (p.links.Count > 0) {
				nonLoneNodes.Add(p);
			}
		}
		nodes = nonLoneNodes; 
	}

	public Vector3 GetRandomValidDestination() {
		return validDestinations[UnityEngine.Random.Range(0, validDestinations.Count - 1)];
	}

	///Determine if 2 points see each other.
	public bool CheckLineOfSight (Vector3 posA, Vector3 posB) {
		if (posA.z != posB.z) {
			return false;
		}

		if ((posA - posB).sqrMagnitude < Mathf.Epsilon) {
			return true;
		}
			
		Polygon poly = null;
		for (int i = 0; i < map.obstaclePolygons.Length; i++) {
			poly = map.obstaclePolygons[i];
			// if polygon on different floor, ignore it
			if (poly.points[0].z != posA.z) {
				continue;
			}
			for (int j = 0; j < poly.points.Length; j++) {
				if (SegmentsCross(posA, posB, poly.points[j], poly.points[(j + 1) % poly.points.Length])) {
					return false;
				}
			}
		}
		return true;
	}

	///determine if a point is within a valid (walkable) area.
	public bool PointIsValid (Vector3 point) {
		// check if point in nav 2d boundaries
		for (int i = 0; i < map.masterPolygons.Length; i++) {
			if (!PointInsidePolygon(map.masterPolygons[i].points, point)) {
				return false;
			}
		}
		
		// check if point not in any obstacles
		int inside = 0;
		for (int i = 0; i < map.obstaclePolygons.Length; i++) {
			if (map.obstaclePolygons[i].points[0].z != point.z) {
				continue;
			}
			if (PointInsidePolygon(map.obstaclePolygons[i].points, point)) {
				inside++;
			}
		}

		// if z == 0, on ground floor so anything with even amount of polygons is good
		if (point.z == 0) {
			return (inside & 1) == 0;
		} else {
			return (inside & 1) == 0 && inside != 0;
		}
	}

	///Kind of scales a polygon based on it's vertices average normal.
	public static Vector3[] InflatePolygon(Vector3[] points, float dist) {

		Vector3[] inflatedPoints = new Vector3[points.Length];

		for (int i = 0; i < points.Length; i++) {
			
			Vector2 ab = (points[(i + 1) % points.Length] - points[i]).normalized;
			Vector2 ac = (points[i == 0? points.Length - 1 : i - 1] - points[i]).normalized;
			Vector2 mid = (ab + ac).normalized;
			
			mid *= (!PointIsConcave(points, i)? -dist : dist);
			inflatedPoints[i] = (Vector3)((Vector2)points[i] + mid) + new Vector3(0, 0, points[i].z);
		}

		return inflatedPoints;
	}

	///Check if or not a point is concave to the polygon points provided
	public static bool PointIsConcave(Vector3[] points, int point) {

		Vector2 current = points[point];
		Vector2 next = points[(point + 1) % points.Length];
		Vector2 previous =  points[point == 0? points.Length - 1 : point - 1];

		Vector2 left = new Vector2(current.x - previous.x, current.y - previous.y);
		Vector2 right = new Vector2(next.x - current.x, next.y - current.y);

		float cross = (left.x * right.y) - (left.y * right.x);

		return cross > 0;
	}

	///Check intersection of two segments, each defined by two vectors.
	public static bool SegmentsCross (Vector2 a, Vector2 b, Vector2 c, Vector2 d) {
		float denominator = ((b.x - a.x) * (d.y - c.y)) - ((b.y - a.y) * (d.x - c.x));

		if (denominator == 0) {
			return false;
		}

	    float numerator1 = ((a.y - c.y) * (d.x - c.x)) - ((a.x - c.x) * (d.y - c.y));
	    float numerator2 = ((a.y - c.y) * (b.x - a.x)) - ((a.x - c.x) * (b.y - a.y));

	    if (numerator1 == 0 || numerator2 == 0) {
	    	return false;
		}

	    float r = numerator1 / denominator;
	    float s = numerator2 / denominator;

	    return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
	}

	///Is a point inside a polygon?
	public static bool PointInsidePolygon(Vector3[] polyPoints, Vector3 point) {

		float xMin = 0;
		for (int i = 0; i < polyPoints.Length; i++) {
			xMin = Mathf.Min(xMin, polyPoints[i].x);
		}

		Vector2 origin = new Vector2(xMin - 1f, point.y);
		int intersections = 0;

		for (int i = 0; i < polyPoints.Length; i++) {

			Vector2 pA = polyPoints[i];
			Vector2 pB = polyPoints[(i + 1) % polyPoints.Length];

			if (SegmentsCross(origin, point, pA, pB))
				intersections ++;
		}

		return (intersections & 1) == 1;
	}

	///Finds the closer edge point to the navigation valid area
	public Vector3 GetCloserEdgePoint (Vector3 point) {

		List<Vector3> possiblePoints = new List<Vector3>();
		Vector3 closerVertex = Vector3.zero;
		float closerVertexDist = Mathf.Infinity;

		Polygon poly = null;
		Vector3[] inflatedPoints = null;
		for (int p = 0; p < map.allPolygons.Length; p++) {

			poly = map.allPolygons[p];
			inflatedPoints = InflatePolygon(poly.points, 0.01f);

			for (int i = 0; i < inflatedPoints.Length; i++) {

				Vector3 a = inflatedPoints[i];
				Vector3 b = inflatedPoints[(i + 1) % inflatedPoints.Length];

				Vector3 originalA = poly.points[i];
				Vector3 originalB = poly.points[(i + 1) % poly.points.Length];
				
				Vector3 proj = Vector3.Project( (point - a), (b - a) ) + a;

				if (SegmentsCross(point, proj, originalA, originalB) && PointIsValid(proj))
					possiblePoints.Add(proj);

				float dist = (point - inflatedPoints[i]).sqrMagnitude;
				if ( dist < closerVertexDist && PointIsValid(inflatedPoints[i])) {
					closerVertexDist = dist;
					closerVertex = inflatedPoints[i];
				}
			}
		}

		possiblePoints.Add(closerVertex);

		float closerDist = Mathf.Infinity;
		int index = 0;
		for (int i = 0; i < possiblePoints.Count; i++) {
			float dist = (point - possiblePoints[i]).sqrMagnitude;
			if (dist < closerDist) {
				closerDist = dist;
				index = i;
			}
		}
		Vector3 closest = possiblePoints[index];
		closest.z = point.z;
		return closest;
	}

	//defines a polygon
	public class Polygon {
		public Vector3[] points;
		public Polygon(Vector3[] points) {
			this.points = points;
		}
	}

	//defines the main navigation polygon and its sub obstacle polygons
	class PolyMap {

		public Polygon[] masterPolygons;
		public Polygon[] obstaclePolygons;
		public Polygon[] allPolygons;

		public PolyMap(Polygon[] masterPolys, Polygon[] obstaclePolys) {
			masterPolygons = masterPolys;
			obstaclePolygons = obstaclePolys;
			List<Polygon> temp = new List<Polygon>();
			temp.AddRange(masterPolys);
			temp.AddRange(obstaclePolys);
			allPolygons = temp.ToArray();
		}
	}

	struct PathRequest {
		public Vector3 start;
		public Vector3 end;
		public Action<Vector3[], bool> callback;

		public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback) {
			this.start = start;
			this.end = end;
			this.callback = callback;
		}
	}

	//defines a default node for A*
	public class PathNode : IHeapItem<PathNode> {
		public Vector3 pos;
		public List<PathNode> links = new List<PathNode>();
		public float gCost = 1;
		public float hCost;
		public PathNode parent = null;
		private int _heapIndex;

		public PathNode (Vector3 pos) {
			this.pos = pos;	
		}

		public float fCost{
			get { return gCost + hCost; }
		}

		public int heapIndex{
			get {return _heapIndex;}
			set {_heapIndex = value;}
		}

		public int CompareTo ( PathNode other ) {
			int compare = fCost.CompareTo(other.fCost);
			if (compare == 0)
				compare = hCost.CompareTo(other.hCost);
			return -compare;
		}
	}

    public class StairNode : PathNode
    {
		public StairNode neighborStair;
        public StairNode(Vector3 pos) : base(pos) {
        }
    }

#if UNITY_EDITOR

    void OnDrawGizmos () {

		if (!drawNodes && !drawNodesAndEdges) {
			return;
		}

		if (!Application.isPlaying) {
			CreatePolyMap(true);
			CreateAndCleanNodes();
			LinkNodes(nodes);
		}

		//the original drawn polygons
		if (masterCollider is PolygonCollider2D) {
			// PolygonCollider2D polyCollider = masterCollider as PolygonCollider2D;
			// for (int i = 0; i < polyCollider.pathCount; ++i ) {
	        //     for (int p = 0; p < polyCollider.GetPath(i).Length; ++p )
	        //         DebugDrawPolygon(TransformPoints( (Vector3[])(polyCollider.GetPath(i)), polyCollider.transform ), Color.green );
	        // }
        
        } else if (masterCollider is BoxCollider2D) {
        	BoxCollider2D box = masterCollider as BoxCollider2D;
			Vector2 tl = box.offset + new Vector2(-box.size.x, box.size.y)/2;
			Vector2 tr = box.offset + new Vector2(box.size.x, box.size.y)/2;
			Vector2 br = box.offset + new Vector2(box.size.x, -box.size.y)/2;
			Vector2 bl = box.offset + new Vector2(-box.size.x, -box.size.y)/2;
        	DebugDrawPolygon(TransformPoints(new Vector3[]{tl, tr, br, bl}, masterCollider.transform), Color.green);
        }

		float nodeSize = 0.15f;
		Color white = new Color(1, 1f, 1f, 1f);

		// foreach (Vector3 v in validDestinations) {
		// 	Vector3[] square = new Vector3[4];
		// 	square[0] = v + new Vector3(-nodeSize, 0);
		// 	square[1] = v + new Vector3(0, nodeSize);
		// 	square[2] = v + new Vector3(nodeSize, 0);
		// 	square[3] = v + new Vector3(0, -nodeSize);

		// 	DebugDrawPolygon(square, white);
		// }

		foreach (PathNode p in nodes) {
			Vector3[] square = new Vector3[4];
			square[0] = p.pos + new Vector3(-nodeSize, 0);
			square[1] = p.pos + new Vector3(0, nodeSize);
			square[2] = p.pos + new Vector3(nodeSize, 0);
			square[3] = p.pos + new Vector3(0, -nodeSize);

			if (drawNodesAndEdges) {
				foreach (PathNode neighbor in p.links) {
					Debug.DrawLine(p.pos, neighbor.pos, white);
				}
			}

			DebugDrawPolygon(square, white);
		}

		//the inflated used polygons
        foreach (Polygon pathPoly in map.masterPolygons) {
        	DebugDrawPolygon(pathPoly.points, new Color(1f, 1f, 1f, 1f));
		}
		foreach(Polygon poly in map.obstaclePolygons) {
			DebugDrawPolygon(poly.points, new Color(1, 0.7f, 0.7f, 1f));
		}
	}

	void DebugDrawPolygon(Vector3[] points, Color color) {
		for (int i = 0; i < points.Length; i++) {
			Debug.DrawLine(points[i], points[(i + 1) % points.Length], color);
		}
	}

	[UnityEditor.MenuItem("GameObject/Create Other/Nav2D")]
	public static void CreatePolyNav2D() {
		if (FindObjectOfType(typeof(Nav2D)) == null) {
			Nav2D newNav = new GameObject("Nav2D").AddComponent<Nav2D>();
			UnityEditor.Selection.activeObject = newNav;
		}
	}

#endif
}