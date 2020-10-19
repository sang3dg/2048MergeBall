using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;
using System;

public class ReadExcel : Editor
{
    [MenuItem("ReadConfigExcel/Read")]
    public static void ReadConfig()
    {
        GameConfig _config = Resources.Load<GameConfig>("GameConfig");
        _config.BallBaseDatas.Clear();
        _config.BallSpawnDatas.Clear();
        _config.StageScoreDatas.Clear();
        _config.SlotsDatas.Clear();
        _config.WheelDatas.Clear();
        _config.GiftDataAs.Clear();
        _config.GiftDataBs.Clear();

        string XlsxPath = Application.dataPath + "/2048.xlsx";
        if (!File.Exists(XlsxPath))
        {
            Debug.LogError("Read MG_ConfigXlsx File Error : file is not exist.");
            return;
        }
        FileStream fs = new FileStream(XlsxPath, FileMode.Open, FileAccess.Read);
        IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
        DataSet dataSet = reader.AsDataSet();
        reader.Dispose();
        if (dataSet is null)
        {
            Debug.LogError("Read MG_ConfigXlsx File Error : file is empty.");
            return;
        }

        #region 球的基本数据
        DataTable ballTable = dataSet.Tables[0];
        int rowCount0 = ballTable.Rows.Count;
        for(int i = 1; i < rowCount0; i++)
        {
            var tempRow = ballTable.Rows[i];
            if (string.IsNullOrEmpty(tempRow[0].ToString()))
                continue;
            BallBaseData configData = new BallBaseData()
            {
                BallNum = int.Parse(tempRow[0].ToString()),
                BallSize = float.Parse(tempRow[1].ToString()),
                BallSpriteName = int.Parse(tempRow[2].ToString())
            };
            _config.BallBaseDatas.Add(configData);
        }
        #endregion
        #region 球的生成规则
        DataTable ballFallTable = dataSet.Tables[1];
        int rowCount1 = ballFallTable.Rows.Count;
        for(int rowIndex = 3; rowIndex < rowCount1; rowIndex++)
        {
            var tempRow = ballFallTable.Rows[rowIndex];
            if (string.IsNullOrEmpty(tempRow[0].ToString()))
                continue;
            BallSpawnData ballFallData = new BallSpawnData()
            {
                ballRangeNum = int.Parse(tempRow[1].ToString())
            };
            string str_realate = tempRow[0].ToString();
            switch (str_realate)
            {
                case ">":
                    ballFallData.realateRange = BallFallRangeType.More;
                    break;
                case "<":
                    ballFallData.realateRange = BallFallRangeType.Less;
                    break;
                case "=":
                    ballFallData.realateRange = BallFallRangeType.Equal;
                    break;
                case ">=":
                    ballFallData.realateRange = BallFallRangeType.MoreEqual;
                    break;
                case "<=":
                    ballFallData.realateRange = BallFallRangeType.LessEqual;
                    break;
                default:
                    Debug.LogError("ball fall config error : coloum 0 value is error.");
                    break;
            }
            List<BallNumWeight> ballNumWeights = new List<BallNumWeight>();
            int columnCount = ballFallTable.Columns.Count;
            for(int columnIndex = 2; columnIndex < columnCount; columnIndex++)
            {
                BallNumWeight numWeight = new BallNumWeight()
                {
                    num = int.Parse(ballFallTable.Rows[1][columnIndex].ToString()),
                    weight = int.Parse(tempRow[columnIndex].ToString())
                };
                ballNumWeights.Add(numWeight);
            }
            ballFallData.ballNumWeights = ballNumWeights;
            _config.BallSpawnDatas.Add(ballFallData);
        }
        #endregion
        #region 关卡升级规则
        DataTable stageScoreTable = dataSet.Tables[2];
        int rowCount2 = stageScoreTable.Rows.Count;
        for(int rowIndex = 1; rowIndex < rowCount2; rowIndex++)
        {
            var tempRow = stageScoreTable.Rows[rowIndex];
            if (string.IsNullOrEmpty(tempRow[0].ToString()))
                continue;
            StageScoreData stageScoreData = new StageScoreData()
            {
                maxStage = int.Parse(tempRow[1].ToString()),
                upgradeNeedScore = int.Parse(tempRow[2].ToString())
            };
            _config.StageScoreDatas.Add(stageScoreData);
        }
        #endregion
        #region 老虎机奖励规则
        DataTable slotsTable = dataSet.Tables[3];
        int rowCount3 = slotsTable.Rows.Count;
        for(int rowIndex = 1; rowIndex < rowCount3; rowIndex++)
        {
            var tempRow = slotsTable.Rows[rowIndex];
            if (string.IsNullOrEmpty(tempRow[0].ToString()))
                continue;
            SlotsData slotsData = new SlotsData()
            {
                maxCash = int.Parse(tempRow[1].ToString()),
                cashRange = new Vector2Int(int.Parse(tempRow[2].ToString()), int.Parse(tempRow[3].ToString())),
                cashWeight = int.Parse(tempRow[4].ToString()),
                coinRange = new Vector2Int(int.Parse(tempRow[5].ToString()), int.Parse(tempRow[6].ToString())),
                coinWeight = int.Parse(tempRow[7].ToString())
            };
            _config.SlotsDatas.Add(slotsData);
        }
        #endregion
        #region 转盘奖励规则
        DataTable wheelTable = dataSet.Tables[4];
        int rowCount4 = wheelTable.Rows.Count;
        for(int rowIndex = 1; rowIndex < rowCount4; rowIndex++)
        {
            var tempRow = wheelTable.Rows[rowIndex];
            if (string.IsNullOrEmpty(tempRow[0].ToString()))
                continue;
            WheelData wheelData = new WheelData()
            {
                type = (Reward)Enum.Parse(typeof(Reward), tempRow[0].ToString()),
                num = int.Parse(tempRow[1].ToString()),
                weight = int.Parse(tempRow[2].ToString()),
                limitCash = int.Parse(tempRow[4].ToString())
            };
            wheelData.blackbox = new List<int>();
            string[] blackBoxStr = tempRow[3].ToString().Split(',');
            foreach(string index in blackBoxStr)
            {
                wheelData.blackbox.Add(int.Parse(index));
            }
            _config.WheelDatas.Add(wheelData);
        }
        #endregion
        #region 礼盒A包生成及奖励规则
        DataTable giftTableA = dataSet.Tables[5];
        int rowCount5 = giftTableA.Rows.Count;
        for(int rowIndex = 1; rowIndex < rowCount5; rowIndex++)
        {
            var tempRow = giftTableA.Rows[rowIndex];
            if (string.IsNullOrEmpty(tempRow[0].ToString()))
                continue;
            GiftDataA giftDataA = new GiftDataA()
            {
                maxStage = int.Parse(tempRow[0].ToString()),
                fallBallNumRange = new Vector2Int(int.Parse(tempRow[1].ToString()), int.Parse(tempRow[2].ToString())),
                rewardCoinNumRange = new Vector2Int(int.Parse(tempRow[3].ToString()), int.Parse(tempRow[4].ToString()))
            };
            _config.GiftDataAs.Add(giftDataA);
        }
        #endregion

        DataTable giftTableB = dataSet.Tables[6];
        int rowCount6 = giftTableB.Rows.Count;
        for(int rowIndex = 3; rowIndex < rowCount6; rowIndex++)
        {
            var tempRow = giftTableB.Rows[rowIndex];
            if (string.IsNullOrEmpty(tempRow[0].ToString()))
                continue;
            GiftDataB giftDataB = new GiftDataB()
            {
                maxCash = int.Parse(tempRow[1].ToString()),
                fallBallNumRange = new Vector2Int(int.Parse(tempRow[2].ToString()), int.Parse(tempRow[3].ToString())),
                cashWeight = int.Parse(tempRow[4].ToString()),
                coinWeight = int.Parse(tempRow[5].ToString()),
                rewardCashNumRange = new Vector2Int(int.Parse(tempRow[6].ToString()), int.Parse(tempRow[7].ToString())),
                rewardCoinNumRange = new Vector2Int(int.Parse(tempRow[8].ToString()), int.Parse(tempRow[8].ToString()))
            };
            _config.GiftDataBs.Add(giftDataB);
        }

        EditorUtility.SetDirty(_config);
        AssetDatabase.SaveAssets();
    }
}
