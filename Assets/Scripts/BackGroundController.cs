using UnityEngine;
using System.Collections;

/*
 * 背景スクロール管理クラス / BackGround Scroll Control Class
 *  
 * （ついでにカメラに追随する（水平方向だけ） / follow the camera in horizontal
*/
public class BackGroundController : MonoBehaviour {

	private BackGroundScroll[] childrenComp;

	public Sprite[] skySprite;
	public Sprite[] farSprite;
	private Vector3 initialPosition;

	// Use this for initialization
	void Start () {
		initialPosition = transform.position;
		Init();
	}

	//------------------------------
	//初期化 / Initialization
	//------------------------------
	public void Init() {
		int stage = GameController.nowStage % 3 - 1;
		if (stage < 0) {
			stage = 2;
		}
		
		childrenComp = new BackGroundScroll[transform.childCount];
		int i = 0;
		foreach (Transform child in transform) {
			childrenComp[i] = child.GetComponent<BackGroundScroll>();
			childrenComp[i].Init();
			if (child.name == "bg_sky") {
				child.GetComponent<SpriteRenderer>().sprite = skySprite[stage];
			} else if (child.name == "bg_far") {
				child.GetComponent<SpriteRenderer>().sprite = farSprite[stage];
			}
			i++;
		}

		transform.position = initialPosition;
	}
	
	//------------------------------
	//左右移動を呼び出す / Call Moving
	//------------------------------
	public void Move (bool isRight) {
		foreach (BackGroundScroll item in childrenComp) {
			item.Move(isRight, 1.0f);	
		}
	}
	public void Move (bool isRight, float factor) {
		foreach (BackGroundScroll item in childrenComp) {
			item.Move(isRight, factor);	
		}
	}

	//------------------------------
	//カメラに追随する（水平方向だけ） / follow the camera in horizontal
	//------------------------------
	void Update() {
		Vector3 pos = transform.position;
		pos.x = Camera.main.transform.position.x;
		transform.position = pos;
	}
}
