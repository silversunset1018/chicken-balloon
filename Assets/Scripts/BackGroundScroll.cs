using UnityEngine;
using System.Collections;

/*
 * 背景のスクロール / BackGround Scroll
 * 
*/
public class BackGroundScroll : MonoBehaviour {
	
	public float speed = 10;
	public int spriteCount = 2;
	public bool moveInTimer;	//時間経過による移動か / Whether move over time
	private Vector3 initialPosition;

	private Vector2 cameraEnd;
	private float width;

	void Start() {
		initialPosition = transform.localPosition;
		width = GetComponent<SpriteRenderer>().bounds.size.x;
	}
	
	public void Init() {
		transform.localPosition = initialPosition;
	}
	
	void Update() {

		if (moveInTimer && GameController.isCanInput) {
			transform.localPosition += Vector3.left * speed * Time.deltaTime;
		}
		cameraEnd.x = Camera.main.ViewportToWorldPoint (Vector3.zero).x;
		cameraEnd.y = Camera.main.ViewportToWorldPoint (new Vector3 (1, 1, 0)).x;
	}

	//------------------------------
	//左右へ移動する / Move left or right
	//------------------------------
	public void Move (bool isRight, float factor) {
		if (moveInTimer) {
			return;
		}
		if (isRight) {
			transform.localPosition += Vector3.left * speed * factor * Time.deltaTime;
			if(transform.position.x + width / 2.0f < cameraEnd.x) {
				ResetPositionRight();
			}
		} else {
			transform.localPosition -= Vector3.left * speed * factor * Time.deltaTime;
			if(transform.position.x - width / 2.0f > cameraEnd.y) {
				ResetPositionLeft();
			}
		}

	}

	//------------------------------
	//カメラから見えなくなったら端へ移動 / Move to the end and when it is unvisible to the camera
	//------------------------------
	void ResetPositionRight() {
		transform.localPosition += Vector3.right * width * spriteCount;
	}
	void ResetPositionLeft() {
		transform.localPosition -= Vector3.right * width * spriteCount;
	}
}
