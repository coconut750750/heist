//#define DEBUG

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using PathNode = Nav2D.PathNode;
using StairNode = Nav2D.StairNode;

static class AStar {

	// calculates path between start and end assuming they are on same floor
	private static bool CalculateFlatPath(PathNode startNode, PathNode endNode, PathNode[] allNodes) {
		#if DEBUG
		Stopwatch sw = new Stopwatch();
		sw.Start();
		#endif

		var openList = new Heap<PathNode>(allNodes.Length);
		var closedList = new HashSet<PathNode>();
		var success = false;

		openList.Add(startNode);

		while (openList.Count > 0){

			var currentNode = openList.RemoveFirst();
			closedList.Add(currentNode);

			if (currentNode == endNode){
				#if DEBUG
				sw.Stop();
				UnityEngine.Debug.Log("Path Found: " + sw.ElapsedMilliseconds + " ms.");
				#endif

				success = true;
				break;
			}

			foreach (PathNode neighbour in currentNode.links){

				if (closedList.Contains(neighbour))
					continue;

				var costToNeighbour = currentNode.gCost + GetDistance( currentNode, neighbour );
				if (costToNeighbour < neighbour.gCost || !openList.Contains(neighbour) ){
					neighbour.gCost = costToNeighbour;
					neighbour.hCost = GetDistance(neighbour, endNode);
					neighbour.parent = currentNode;

					if (!openList.Contains(neighbour)){
						openList.Add(neighbour);
						openList.UpdateItem(neighbour);
					}
				}
			}
		}

		return success;
	}

	public static IEnumerator CalculatePath(PathNode startNode, PathNode endNode, PathNode[] allNodes, StairNode[] stairNodes, Action<Vector3[], bool> callback){
		bool success = false;
		bool reverse = true;
		// if start and end are on different floors, find nearest stair to the node on top floor
		if (startNode.pos.z != endNode.pos.z) {
			// floors are going down in z, so its inverted
			bool topToDown = startNode.pos.z < endNode.pos.z;
			if (!topToDown) {
				reverse = false;
			}
			PathNode topNode = topToDown ? startNode : endNode;
			PathNode botNode = !topToDown ? startNode : endNode;

			foreach (StairNode stairNode in stairNodes) {
				// for each stair node, find if there is a path between start/end node (which ever is on top floor)
				// if stair Node not on same floor as topNode, continue
				if (stairNode.pos.z != topNode.pos.z) {
					continue;
				} else {
					success = CalculateFlatPath(topNode, stairNode, allNodes);
				}
				if (success) {
					stairNode.neighborStair.parent = stairNode;
					success = CalculateFlatPath(stairNode.neighborStair, botNode, allNodes);
				}
				if (success) {
					break;
				}
			}
		} else {
			success = CalculateFlatPath(startNode, endNode, allNodes);
		}

		yield return null;
		if (success) {
			callback( RetracePath(startNode, endNode, reverse), true );
		} else {
			callback( new Vector3[0], false );
		}
	}

	private static Vector3[] RetracePath(PathNode startNode, PathNode endNode, bool reverse){
		List<Vector3> path = new List<Vector3>();
		PathNode currentNode = reverse ? endNode : startNode;
		PathNode retraceEndNode = reverse ? startNode : endNode;
		
		while(currentNode != retraceEndNode){
			//UnityEngine.Debug.Log("path node: " +currentNode.pos);
			path.Add(currentNode.pos);
			
			currentNode = currentNode.parent;
		}

		path.Add(retraceEndNode.pos);
		if (reverse) {
			path.Reverse();
		}

		return path.ToArray();
	}

	private static float GetDistance(PathNode a, PathNode b){
		return (a.pos - b.pos).magnitude;
	}
}