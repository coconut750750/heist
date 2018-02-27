using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.IO;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour {

	public int mapNumber;

	private Transform boardHolder;

//	#if UNITY_ANDROID
//	IEnumerator DownloadFiles (string itemsUrl) {
//		WWW itemsWWW = new WWW(itemsUrl);
//		yield return itemsWWW;
//
//		String[] itemsText = itemsWWW.text.Split(new char[]{ '\n' });
//
//		BoardSetup();
//	}
//	#endif

//	void MapSetup() {
//		//char[] delimiterLines = { '\n' };
//
//		#if UNITY_STANDALONE || UNITY_WEBPLAYER
//
//		//itemFile = Application.dataPath + "/StreamingAssets/Maps/items" + mapNumber;
//
//		#elif UNITY_IOS || UNITY_IPHONE
//
//		//itemFile = Application.dataPath + "/Raw/Maps/items" + mapNumber
//
//		#elif UNITY_ANDROID
//
//		//itemFile = "jar:file://" + Application.dataPath + "!/assets/Maps/items" + mapNumber;
//		//StartCoroutine(DownloadFiles (itemFile));
//
//		#endif
//
//		#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_IOS || UNITY_IPHONE
//
//		BoardSetup();
//
//		#endif
//	} 

	void InstantiateGameObject(GameObject obj, Vector3 pos) {
		GameObject instance = Instantiate (obj, pos, Quaternion.identity);
		instance.transform.SetParent (boardHolder);
	}

	void BoardSetup() {		
		// load from previous save state
	}

	public void SetupScene() {
		boardHolder = new GameObject ("Board").transform;
		//MapSetup ();
	}
}
