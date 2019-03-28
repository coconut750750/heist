using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using PathNode = Nav2D.PathNode;
using StairNode = Nav2D.StairNode;

public static class AStar {
	// calculates path between start and end assuming they are on same floor
	private static bool CalculateFlatPath(PathNode startNode, PathNode endNode, PathNode[] allNodes) {
		Heap<PathNode> openList = new Heap<PathNode>(allNodes.Length);
		HashSet<PathNode> closedList = new HashSet<PathNode>();

		openList.Add(startNode);

		while (openList.Count > 0){
			PathNode currentNode = openList.RemoveFirst();
			closedList.Add(currentNode);

			if (currentNode == endNode){
				return true;
			}

			foreach (PathNode neighbour in currentNode.links){

				if (closedList.Contains(neighbour)) {
					continue;
				}

				float costToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
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

		return false;
	}

	public static IEnumerator CalculatePath(PathNode startNode, PathNode endNode, PathNode[] allNodes, StairNode[] stairNodes,
											Action<Vector3[], bool> callback){
		bool success = false;
		bool reverse = true;

		// if start and end are on different floors, find nearest stair to the node on top floor
		if (startNode.pos.z != endNode.pos.z) {
			// floors are going down in z, so its inverted
			bool topToDown = startNode.pos.z < endNode.pos.z;
			PathNode topNode = topToDown ? startNode : endNode;
			PathNode botNode = !topToDown ? startNode : endNode;
			reverse = topToDown;

			foreach (StairNode stairNode in stairNodes) {
				// for each stair node, find if there is a path between start/end node (which ever is on top floor)
				// if stair Node not on same floor as topNode, continue
				if (stairNode.pos.z != topNode.pos.z) {
					continue;
				}
				
				success = CalculateFlatPath(topNode, stairNode, allNodes);
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

		Vector3[] path = success ? RetracePath(startNode, endNode, reverse) : new Vector3[0];
		callback(path, success);
	}

	private static Vector3[] RetracePath(PathNode startNode, PathNode endNode, bool reverse){
		List<Vector3> path = new List<Vector3>();
		PathNode currentNode = reverse ? endNode : startNode;
		PathNode retraceEndNode = reverse ? startNode : endNode;
		
		while (currentNode != retraceEndNode){
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