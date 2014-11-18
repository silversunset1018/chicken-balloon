using UnityEngine;
using System.Collections;
using Soomla;
using Soomla.Example;
using Soomla.Store;

public class ShopInitialize : MonoBehaviour {
	//課金用.
	private static bool isIAPInit;
	private static ExampleEventHandler handler;

	void Awake() {
		string beforeName = gameObject.name;
		gameObject.name = gameObject.name +"_";
		GameObject obj = GameObject.Find(beforeName);
		if (obj != null) {
			Destroy(obj);
		}
		gameObject.name = beforeName;
	}

	
	// Use this for initialization
	void Start () {
		if (!isIAPInit) {
			handler = new ExampleEventHandler();
			SoomlaStore.Initialize(new IAPAssets());
			SoomlaUtils.LogDebug("", "SoomlaStore.Initialize");
			isIAPInit = true;
		}

		DontDestroyOnLoad(gameObject);
	}
}
