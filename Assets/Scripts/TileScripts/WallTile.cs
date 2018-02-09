using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WallTile : Tile {

	[SerializeField]
	private Sprite[] sprites;

	public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
		for (int y = -1; y <= 1; y++) {
			for (int x = -1; x <= 1; x++) {
				Vector3Int nPos = new Vector3Int (position.x + x, position.y + y, position.z);

				if (isWall (tilemap, nPos)) {
					tilemap.RefreshTile (nPos);
				}
			}
		}
		
	}

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
		bool left = isWall (tilemap, new Vector3Int (position.x - 1, position.y, position.z));
		bool up = isWall (tilemap, new Vector3Int (position.x, position.y + 1, position.z));
		bool right = isWall (tilemap, new Vector3Int (position.x + 1, position.y, position.z));
		bool down = isWall (tilemap, new Vector3Int (position.x, position.y - 1, position.z));
		tileData.sprite = GetWall (new bool[]{ left, up, right, down });
		tileData.colliderType = Tile.ColliderType.Grid;
	}

	public bool isWall(ITilemap tilemap, Vector3Int pos) {
		return tilemap.GetTile (pos) == this;
	}

	private Sprite GetWall(bool[] adj) {
		bool l = adj [0]; bool u = adj [1]; bool r = adj [2]; bool d = adj [3];
		if (!l && !u && !r && d) {
			return sprites [5];
		} else if (!l && !u && r && !d) {
			return sprites [4];
		} else if (!l && !u && r && d) {
			return sprites [7];
		} else if (!l && u && !r && !d) {
			return sprites [9];
		} else if (!l && u && !r && d) {
			return sprites [1];
		} else if (!l && u && r && !d) {
			return sprites [3];
		} else if (!l && u && r && d) {
			return sprites [11];
		} else if (l && !u && !r && !d) {
			return sprites [8];
		} else if (l && !u && !r && d) {
			return sprites [6];
		} else if (l && !u && r && !d) {
			return sprites [0];
		} else if (l && !u && r && d) {
			return sprites [12];
		} else if (l && u && !r && !d) {
			return sprites [2];
		} else if (l && u && !r && d) {
			return sprites [13];
		} else if (l && u && r && !d) {
			return sprites [10];
		} else if (l && u && r && d) {
			return sprites [14];
		} else {
			return null;
		}
	}


	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Tiles/WallTile")]
	public static void CreateWallTile() {
		string path = EditorUtility.SaveFilePanelInProject ("Save Wall Tile", "New Wall Tile", "Asset", "Save Wall Tile", "Assets");
		if (path == "") {
			return;
		}
		AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<WallTile>(), path);
	}
	#endif
}
