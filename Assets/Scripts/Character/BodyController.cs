using UnityEngine;
using System.Collections;

public class BodyController : MonoBehaviour {
	private PlayerController playerCont;
	
	// Use this for initialization
	void Start () {
		playerCont = gameObject.GetComponentInParent<PlayerController>();
	}
	
	void OnTriggerEnter(Collider collider) {
		playerCont.TriggerEnter(collider, gameObject.tag, collider);
	}
	
	void OnTriggerExit(Collider collider) {
		playerCont.TriggerExit(collider, gameObject.tag);
	}

	void OnTriggerStay(Collider collider) {
		playerCont.TriggerStay(collider, gameObject.tag);
	}
}