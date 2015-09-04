using UnityEngine;
using System.Collections;

/*
 * ゲームで使う数値の設定クラス / Parameter Setting Class in Game
 * 
 * インスペクタから設定し、各コンポーネントから参照して利用する.
 * Set from the inspector, to be used by reference from each component.
 * 
*/

public class GameSettings : MonoBehaviour {
	//ネットワーク用シーン.
	public bool isNetworkScene;

	//最大ステージ数.
	public int maxStageNum;

	//ステージ長さ係数.
	//charaMovePower * stageLongFactor = length of the stage
	public float stageLongFactor;

	public float[] timeLimit;	//seconds
	public float gravity;
	public float gravityFactorWithBalloon;
	public int maxBalloonNum;
	public float charaJumpPower;
	public float charaJumpTime;
	public float charaMovePower;

	public int[] itemNumInStage;

	//アイテム設定 / Item Setting
	public int scoreByCoin;
	public int scoreByHat;
	public float timeByMaracas;
	public int scoreByTequila;
	public int scoreByTacos;
	
	public GameObject[] normalItemPrefab;
	public GameObject[] rareItemPrefab;
	public float rarity;	//レアリティ（百分率）：この確率でレアアイテムの方が出る　例：0.3→レアアイテム出現率30％、ノーマルアイテム出現率70％.


	//残り時間に対するスコア係数.
	//Remaining time * scoreTimeFactor = score for the remaining time
	public float scoreTimeFactor;

	//進んだ距離に対するスコアの基準値 / Reference value of the score for the distance advanced
	public float scoreByMaxStage;

	//風船の色によるボーナススコア加算値.
	public int scoreByBalloonGreen;
	public int scoreByBalloonYellow;

	
	//ネットワーク用.
	//ステージの左端(X)・右端(Y)（ループ用）.
	public Vector2 networkStageEdge;
	//ループ時のカメラのX位置：左端(X)・右端(Y).
	public Vector2 networkCameraPosition;
	//復活後の無敵時間.
	public float networkRecoverArmorModeTime;
	//復活後の点滅速度.
	public float networkRecoverArmorModeBlinkTime;
	//風船が割れた後の無敵時間（単位；秒）.
	public float networkHitCoolTime;
}
