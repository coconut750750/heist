using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class Nav2DTest {

	[SetUp]
	public void LoadMainScene() {
		SceneManager.LoadScene("MainScene");
	}

	[UnityTest]
	public IEnumerator Nav2DValidPoints() {
		yield return null;

		Nav2D nav2d = GameObject.Find("Navigation").GetComponent<Nav2D>();

		foreach (Vector3 v in nav2d.validPoints) {
			if (v.x <= -2.5f && v.x >= -3.5f)
			Debug.Log(v);
		}
	}

	[TearDown]
	public void UnloadMainScene() {
		GameManager.instance.save = false;
	}
}
