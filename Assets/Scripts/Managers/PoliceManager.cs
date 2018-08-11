using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceManager : MonoBehaviour {

	public int numSuspicious = 0;

	private PoliceNPC[] police;

	void Awake () {
		police = GetComponentsInChildren<PoliceNPC>();
	}

	
	
}
