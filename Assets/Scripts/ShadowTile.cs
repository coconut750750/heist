using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ShadowTile : Tile {

	[SerializeField]
	private Sprite[] sprites;

	public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
		for (int y = -1; y <= 1; y++) {
			for (int x = -1; x <= 1; x++) {
				Vector3Int nPos = new Vector3Int (position.x + x, position.y + y, position.z);

				if (isShadow (tilemap, nPos)) {
					tilemap.RefreshTile (nPos);
				}
			}
		}
	}

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
		bool left = isShadow (tilemap, new Vector3Int (position.x - 1, position.y, position.z));
		bool up = isShadow (tilemap, new Vector3Int (position.x, position.y + 1, position.z));
		bool right = isShadow (tilemap, new Vector3Int (position.x + 1, position.y, position.z));
		bool down = isShadow (tilemap, new Vector3Int (position.x, position.y - 1, position.z));

		tileData.sprite = GetShadow (new bool[]{ left, up, right, down });
		tileData.colliderType = Tile.ColliderType.None;
	}

	public bool isShadow(ITilemap tilemap, Vector3Int pos) {
		return tilemap.GetTile (pos) == this;
	}

	private Sprite GetShadow(bool[] adj) {
		bool l = adj [0]; bool u = adj [1]; bool r = adj [2]; bool d = adj [3];
		if (l && u && !r && !d) {
			return sprites [0];
		} else if (l && !u && r && !d) {
			return sprites [1];
		} else if (l && !u && !r && !d) {
			return sprites [1];
		} else if (!l && u && !r && d) {
			return sprites [2];
		} else if (!l && u && !r && !d) {
			return sprites [2];
		} else if (!l && !u && r && d) {
			return sprites [3];
		} else if (!l && !u && !r && d) {
			return sprites [4];
		} else if (!l && !u && r && !d) {
			return sprites [5];
		} 
		return null;
	}

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Tiles/ShadowTile")]
	public static void CreateWallTile() {
		string path = EditorUtility.SaveFilePanelInProject ("Save Shadow Tile", "New Shadow Tile", "Asset", "Save Shadow Tile", "Assets");
		if (path == "") {
			return;
		}
		AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<ShadowTile>(), path);
	}
	#endif
}
