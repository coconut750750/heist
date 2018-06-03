//#define DEBUG

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using PathNode = Nav2D.PathNode;

static class AStar {

	public static IEnumerator CalculatePath(PathNode startNode, PathNode endNode, PathNode[] allNodes, Action<Vector3[], bool> callback){

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

		yield return null;
		if (success){
			callback( RetracePath(startNode, endNode), true );
		} else {
			callback( new Vector3[0], false );
		}
	}

	private static Vector3[] RetracePath(PathNode startNode, PathNode endNode){
		var path = new List<Vector3>();
		var currentNode = endNode;

		while(currentNode != startNode){
			path.Add(currentNode.pos);
			//UnityEngine.Debug.Log(currentNode.pos);
			currentNode = currentNode.parent;
		}

		path.Add(startNode.pos);
		path.Reverse();

		return path.ToArray() ;
	}

	private static float GetDistance(PathNode a, PathNode b){
		return (a.pos - b.pos).magnitude;
	}
}