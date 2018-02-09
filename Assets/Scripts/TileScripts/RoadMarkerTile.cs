using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoadMarkerTile : Tile {

	[SerializeField]
	private Sprite[] sprites;

	public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
		for (int y = -1; y <= 1; y++) {
			for (int x = -1; x <= 1; x++) {
				Vector3Int nPos = new Vector3Int (position.x + x, position.y + y, position.z);

				if (isRoadMarker (tilemap, nPos)) {
					tilemap.RefreshTile (nPos);
				}
			}
		}
	}

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
		bool left = isRoadMarker (tilemap, new Vector3Int (position.x - 1, position.y, position.z));
		bool up = isRoadMarker (tilemap, new Vector3Int (position.x, position.y + 1, position.z));
		bool right = isRoadMarker (tilemap, new Vector3Int (position.x + 1, position.y, position.z));
		bool down = isRoadMarker (tilemap, new Vector3Int (position.x, position.y - 1, position.z));
		tileData.sprite = GetRoadMarker (new bool[]{ left, up, right, down });
		tileData.colliderType = Tile.ColliderType.None;
	}

	public bool isRoadMarker(ITilemap tilemap, Vector3Int pos) {
		return tilemap.GetTile (pos) == this;
	}

	private Sprite GetRoadMarker(bool[] adj) {
		bool l = adj [0]; bool u = adj [1]; bool r = adj [2]; bool d = adj [3];
		if (!l && !u && !r && !d) {
			return null;
		} else if ((u || d) && !l && !r) {
			return sprites [0];
		} else if ((l || r) && !u && !d) {
			return sprites [1];
		} 

		return null;
	}


	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Tiles/RoadMarkerTile")]
	public static void CreateWallTile() {
		string path = EditorUtility.SaveFilePanelInProject ("Save Road Marker Tile", "New Road Marker Tile", "Asset", "Save Road Marker Tile", "Assets");
		if (path == "") {
			return;
		}
		AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<RoadMarkerTile>(), path);
	}
	#endif
}
