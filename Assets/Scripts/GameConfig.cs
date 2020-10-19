using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameConfig", menuName = "Create GameConfig")]
public class GameConfig : ScriptableObject
{
    public List<BallBaseData> BallBaseDatas = new List<BallBaseData>();
    public List<BallSpawnData> BallSpawnDatas = new List<BallSpawnData>();
    public List<StageScoreData> StageScoreDatas = new List<StageScoreData>();
    public List<SlotsData> SlotsDatas = new List<SlotsData>();
    public List<WheelData> WheelDatas = new List<WheelData>();
    public List<GiftDataA> GiftDataAs = new List<GiftDataA>();
    public List<GiftDataB> GiftDataBs = new List<GiftDataB>();
}
[System.Serializable]
public struct BallBaseData
{
    public int BallNum;
    public float BallSize;
    public int BallSpriteName;
}
[System.Serializable]
public struct BallSpawnData
{
    public BallFallRangeType realateRange;
    public int ballRangeNum;
    public List<BallNumWeight> ballNumWeights;
}
[System.Serializable]
public struct BallNumWeight
{
    public int num;
    public int weight;
}
public enum BallFallRangeType
{
    More,
    Less,
    Equal,
    MoreEqual,
    LessEqual,
}
[System.Serializable]
public struct StageScoreData
{
    public int maxStage;
    public int upgradeNeedScore;
}
[System.Serializable]
public struct SlotsData
{
    public int maxCash;
    public Vector2Int cashRange;
    public int cashWeight;
    public Vector2Int coinRange;
    public int coinWeight;
}
[System.Serializable]
public struct WheelData
{
    public Reward type;
    public int num;
    public int weight;
    public List<int> blackbox;
    public int limitCash;
}
[System.Serializable]
public struct GiftDataA
{
    public int maxStage;
    public Vector2Int fallBallNumRange;
    public Vector2Int rewardCoinNumRange;
}
[System.Serializable]
public struct GiftDataB
{
    public int maxCash;
    public Vector2Int fallBallNumRange;
    public int cashWeight;
    public int coinWeight;
    public Vector2Int rewardCashNumRange;
    public Vector2Int rewardCoinNumRange;
}
