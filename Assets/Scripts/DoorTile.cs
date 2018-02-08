using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DoorTile : Tile {

	[SerializeField]
	private Sprite[] sprites;

	public override void RefreshTile(Vector3Int position, ITilemap tilemap) {
		
	}

	public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
		
	}

	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Tiles/DoorTile")]
	public static void CreateDoorTile() {
		string path = EditorUtility.SaveFilePanelInProject ("Save Door Tile", "New Door Tile", "Asset", "Save Door Tile", "Assets");
		if (path == "") {
			return;
		}
		AssetDatabase.CreateAsset (ScriptableObject.CreateInstance<DoorTile>(), path);
	}
	#endif
}
