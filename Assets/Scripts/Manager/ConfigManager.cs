using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ConfigManager
{
    static GameConfig gameConfig = null;
    public static BallBaseData GetBallBaseConfig(int num)
    {
        if(gameConfig is null)
        {
            gameConfig = Resources.Load<GameConfig>("GameConfig");
        }
        int count = gameConfig.BallBaseDatas.Count;
        for(int i = 0; i < count; i++)
        {
            if (gameConfig.BallBaseDatas[i].BallNum == num)
                return gameConfig.BallBaseDatas[i];
        }
        return gameConfig.BallBaseDatas[count - 1];
    }
    public static BallSpawnData GetBallSpawnConfig(int maxNum)
    {
        if (gameConfig is null)
        {
            gameConfig = Resources.Load<GameConfig>("GameConfig");
        }
        List<BallSpawnData> ballSpawnDatas = gameConfig.BallSpawnDatas;
        int count = ballSpawnDatas.Count;
        for(int i = 0; i < count; i++)
        {
            BallSpawnData data = ballSpawnDatas[i];
            switch (data.realateRange)
            {
                case BallFallRangeType.More:
                    if (maxNum > data.ballRangeNum)
                        return data;
                    break;
                case BallFallRangeType.Less:
                    if (maxNum < data.ballRangeNum)
                        return data;
                    break;
                case BallFallRangeType.Equal:
                    if (maxNum == data.ballRangeNum)
                        return data;
                    break;
                case BallFallRangeType.MoreEqual:
                    if (maxNum >= data.ballRangeNum)
                        return data;
                    break;
                case BallFallRangeType.LessEqual:
                    if (maxNum <= data.ballRangeNum)
                        return data;
                    break;
            }
        }
        Debug.LogError("Num is not in range." + maxNum);
        return default;
    }
    public static StageScoreData GetStageScoreData(int currentStage)
    {
        if (gameConfig is null)
        {
            gameConfig = Resources.Load<GameConfig>("GameConfig");
        }
        int count = gameConfig.StageScoreDatas.Count;
        for(int i = 0; i < count; i++)
        {
            if (currentStage < gameConfig.StageScoreDatas[i].maxStage)
                return gameConfig.StageScoreDatas[i];
        }
        Debug.LogError("StageScore Data Out of range");
        return gameConfig.StageScoreDatas[count - 1];
    }
    public static SlotsData GetSlotsData(int currentCash)
    {
        if (gameConfig is null)
        {
            gameConfig = Resources.Load<GameConfig>("GameConfig");
        }
        int count = gameConfig.SlotsDatas.Count;
        for(int i = 0; i < count; i++)
        {
            if (currentCash <= gameConfig.SlotsDatas[i].maxCash)
            {
                return gameConfig.SlotsDatas[i];
            }
        }
        Debug.LogError("Slots Data Out of range");
        return gameConfig.SlotsDatas[count - 1];
    }
    public static List<WheelData> GetWheelDatas()
    {
        return gameConfig.WheelDatas;
    }
}
