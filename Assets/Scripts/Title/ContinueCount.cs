using UnityEngine;
using System.Collections;

public class ContinueCount : MonoBehaviour {
	public SpriteRenderer countObj;
	public Sprite countSprites1;
	public Sprite countSprites2;
	public Sprite countSprites3;

	private float timer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (timer != 4.0f) {
			timer += Time.deltaTime;

			if (timer >= 3.0f) {
				GameController.TouchContinueNo ();
				timer = 4.0f;
			} else if (timer >= 2.0f && countObj.sprite == countSprites2) {
				countObj.sprite = countSprites1;
			} else if (timer >= 1.0f && countObj.sprite == countSprites3) {
				countObj.sprite = countSprites2;
			}
		}
	}

	void OnEnable() {
		countObj.sprite = countSprites3;
		timer = 0.0f;
	}
}
