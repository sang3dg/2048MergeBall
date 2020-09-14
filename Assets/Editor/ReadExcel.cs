using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Excel;
using System.Data;

public class ReadExcel : Editor
{
    [MenuItem("ReadConfigExcel/Read")]
    public static void ReadConfig()
    {
        BallConfig _config = Resources.Load<BallConfig>("BallConfig");
        _config.Num_Radio_SpriteIndex.Clear();
        _config.Range_Num_Weight.Clear();

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
            _config.Num_Radio_SpriteIndex.Add(configData);
        }
        #endregion

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
            _config.Range_Num_Weight.Add(ballFallData);
        }

        EditorUtility.SetDirty(_config);
        AssetDatabase.SaveAssets();
    }
}
