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
				tilemap.RefreshTile (nPos);
			}
		}
	}

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
		bool left = isWall (tilemap, new Vector3Int (position.x - 1, position.y, position.z));
		bool up = isWall (tilemap, new Vector3Int (position.x, position.y + 1, position.z));
		bool upleft = isWall (tilemap, new Vector3Int (position.x - 1, position.y + 1, position.z));

		tileData.sprite = GetShadow (new bool[]{ left, up, upleft });
		tileData.colliderType = Tile.ColliderType.None;
	}

	public bool isWall(ITilemap tilemap, Vector3Int pos) {
		TileBase tile = tilemap.GetTile (pos);
		if (tile != null) {
			return tile.GetType () == typeof(WallTile);
		}
		return false;
	}

	private Sprite GetShadow(bool[] adj) {
		bool l = adj [0]; bool u = adj [1]; bool ul = adj [2];
		if (!l && !u && ul) {
			return sprites [0];
		} else if (!l && u && ul) {
			return sprites [1];
		} else if (l && !u && ul) {
			return sprites [2];
		} else if (l && u && ul) {
			return sprites [3];
		} else if (l && !u && !ul) {
			return sprites [4];
		} else if (!l && u && !ul) {
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
