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

}
