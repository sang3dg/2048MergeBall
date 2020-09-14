using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BallConfig", menuName = "Create BallConfig")]
public class BallConfig : ScriptableObject
{
    public List<BallBaseData> Num_Radio_SpriteIndex = new List<BallBaseData>();
    public List<BallSpawnData> Range_Num_Weight = new List<BallSpawnData>();
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
