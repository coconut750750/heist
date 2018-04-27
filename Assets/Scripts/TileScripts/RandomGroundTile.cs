using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RandomGroundTile : Tile {

	public const int MAX_TILES = 4;
	
	[SerializeField]
	private Sprite[] sprites;

	public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
		tilemap.RefreshTile (position);
	}

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
		if (sprites.Length != 0) {
			tileData.sprite = sprites [Mathf.Abs(position.x + position.y) % sprites.Length];
			tileData.colliderType = Tile.ColliderType.None;
		}
	}

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Tiles/RandomGroundTile")]
	public static void CreateWallTile() {
		string path = EditorUtility.SaveFilePanelInProject ("Save Random Ground Tile", "New Random Ground Tile", "Asset", "Save Random Ground Tile", "Assets");
		if (path == "") {
			return;
		}
		AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<RandomGroundTile>(), path);
	}
	#endif
}
