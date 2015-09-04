using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkDataHolder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
	}


}
