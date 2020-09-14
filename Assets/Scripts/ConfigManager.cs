using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ConfigManager
{
    static BallConfig ballConfig = null;
    public static BallBaseData GetBallBaseConfig(int num)
    {
        if(ballConfig is null)
        {
            ballConfig = Resources.Load<BallConfig>("BallConfig");
        }
        int count = ballConfig.Num_Radio_SpriteIndex.Count;
        for(int i = 0; i < count; i++)
        {
            if (ballConfig.Num_Radio_SpriteIndex[i].BallNum == num)
                return ballConfig.Num_Radio_SpriteIndex[i];
        }
        return ballConfig.Num_Radio_SpriteIndex[count - 1];
    }
    public static BallSpawnData GetBallSpawnConfig(int maxNum)
    {
        if (ballConfig is null)
        {
            ballConfig = Resources.Load<BallConfig>("BallConfig");
        }
        List<BallSpawnData> ballSpawnDatas = ballConfig.Range_Num_Weight;
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
}
