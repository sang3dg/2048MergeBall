using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager
{
    static PlayerData playerData = null;
    public PlayerDataManager()
    {
        if (playerData is null)
        {
            string dataStr = PlayerPrefs.GetString("playerData", "");
            if (string.IsNullOrEmpty(dataStr))
            {
                playerData = new PlayerData()
                {
                    isPackB = false,
                    cash = 0,
                    coin = 0,
                    amazon = 0,
                    prop1Num = 0,
                    prop2Num = 0,
                    prop1NeedCoinNum = GameManager.originPropNeedCoinNum,
                    prop2NeedCoinNum = GameManager.originPropNeedCoinNum,
                    wheelTicket = GameManager.startWheelTicket,
                    todayCanGetCashTime = GameManager.canGetCashTimesPerDay,
                    lastGetCashTime = DateTime.Now.ToString(),
                    lastUseFreeWheelTime = DateTime.Now.AddDays(-1).ToString(),
                    spinWheelTimeTotal = 0,
                    fallBallNum = 0,
                    score = 0,
                    bestScore = 0,
                    stage = 0,
                    currentBallNum = 0,
                    ballPos = new List<Vector2>(),
                    ballNum = new List<int>()
                };
                Save();
            }
            else
                playerData = JsonUtility.FromJson<PlayerData>(dataStr);
        }
    }
    public bool GetIsPackB()
    {
        return playerData.isPackB;
    }
    public void SetIsPackB()
    {
        if (playerData.isPackB) return;
        playerData.isPackB = true;
        Save();
    }
    public bool SetScore(int value)
    {
        bool isBest = false;
        playerData.score = value;
        if (value > playerData.bestScore)
        {
            isBest = true;
            playerData.bestScore = value;
        }
        Save();
        return isBest;
    }
    public void SetStage(int value)
    {
        playerData.stage = value;
        Save();
    }
    private void Save()
    {
        PlayerPrefs.SetString("playerData", JsonUtility.ToJson(playerData));
        PlayerPrefs.Save();
    }
    public int GetScore()
    {
        return playerData.score;
    }
    public int GetBestScore()
    {
        return playerData.bestScore;
    }
    public int GetStage()
    {
        return playerData.stage;
    }
    public int CurrentBallNum()
    {
        return playerData.currentBallNum;
    }
    public int GetCoin()
    {
        return playerData.coin;
    }
    public int GetCash()
    {
        return playerData.cash;
    }
    public int GetAmazon()
    {
        return playerData.amazon;
    }
    public int GetPop1Num()
    {
        return playerData.prop1Num;
    }
    public int GetPop2Num()
    {
        return playerData.prop2Num;
    }
    public int GetProp1NeedCoinNum()
    {
        return playerData.prop1NeedCoinNum;
    }
    public int GetProp2NeedCoinNum()
    {
        return playerData.prop2NeedCoinNum;
    }
    public bool GetTodayHasFreeWheel()
    {
        DateTime now = DateTime.Now;
        DateTime last = DateTime.Parse(playerData.lastUseFreeWheelTime);
        bool isTomorrow = false;
        if (now.Year == last.Year)
        {
            if (now.Month == last.Month)
            {
                if (now.Day > last.Day)
                    isTomorrow = true;
            }
            else if (now.Month > last.Month)
                isTomorrow = true;
        }
        else if (now.Year > last.Year)
            isTomorrow = true;
        return isTomorrow;
    }
    public int GetWheelTicket()
    {
        return playerData.wheelTicket;
    }
    public int GetWheelSpinTimeTotal()
    {
        return playerData.spinWheelTimeTotal;
    }
    public void UseFreeWheel()
    {
        playerData.lastUseFreeWheelTime = DateTime.Now.ToString();
        Save();
    }
    public int GetTodayCanGetCashTime()
    {
        DateTime now = DateTime.Now;
        DateTime last = DateTime.Parse(playerData.lastGetCashTime);
        bool isTomorrow = false;
        if (now.Year == last.Year)
        {
            if (now.Month == last.Month)
            {
                if (now.Day > last.Day)
                    isTomorrow = true;
            }
            else if (now.Month > last.Month)
                isTomorrow = true;
        }
        else if (now.Year > last.Year)
            isTomorrow = true;
        if (isTomorrow)
        {
            playerData.todayCanGetCashTime = GameManager.canGetCashTimesPerDay;
            playerData.lastGetCashTime = now.ToString();
            Save();
        }
        return playerData.todayCanGetCashTime;
    }
    public int GetFallBallNum()
    {
        return playerData.fallBallNum;
    }
    public int AddCoin(int value)
    {
        playerData.coin += value;
        playerData.coin = Mathf.Clamp(playerData.coin, 0, int.MaxValue);
        Save();
        return playerData.coin;
    }
    public int AddCash(int value)
    {
        playerData.cash += value;
        playerData.cash = Mathf.Clamp(playerData.cash, 0, int.MaxValue);
        Save();
        return playerData.cash;
    }
    public int AddAmazon(int value)
    {
        playerData.amazon += value;
        playerData.amazon = Mathf.Clamp(playerData.amazon, 0, int.MaxValue);
        Save();
        return playerData.amazon;
    }
    public int AddPop1Num(int value)
    {
        playerData.prop1Num += value;
        playerData.prop1Num = Mathf.Clamp(playerData.prop1Num, 0, int.MaxValue);
        Save();
        return playerData.prop1Num;
    }
    public int AddPop2Num(int value)
    {
        playerData.prop2Num += value;
        playerData.prop2Num = Mathf.Clamp(playerData.prop2Num, 0, int.MaxValue);
        Save();
        return playerData.prop2Num;
    }
    public void SetProp1NeedCoinNum(int value)
    {
        playerData.prop1NeedCoinNum = value;
        Save();
    }
    public void SetProp2NeedCoinNum(int value)
    {
        playerData.prop2NeedCoinNum = value;
        Save();
    }
    public int ReduceTodayCanGetCashTime(int value = -1)
    {
        playerData.todayCanGetCashTime += value;
        if (playerData.todayCanGetCashTime < 0)
            playerData.todayCanGetCashTime = 0;
        playerData.lastGetCashTime = DateTime.Now.ToString();
        Save();
        return playerData.todayCanGetCashTime;
    }
    public int AddSpinWheelTimeTotal(int value = 1)
    {
        playerData.spinWheelTimeTotal += value;
        Save();
        return playerData.spinWheelTimeTotal;
    }
    public int AddWheelTicket(int value)
    {
        playerData.wheelTicket += value;
        playerData.wheelTicket = Mathf.Clamp(playerData.wheelTicket, 0, int.MaxValue);
        Save();
        return playerData.wheelTicket;
    }
    public int AddFallBallNum(int value = 1)
    {
        playerData.fallBallNum += value;
        Save();
        return playerData.fallBallNum;
    }
    public void ClearFallBallNum()
    {
        playerData.fallBallNum = 0;
        Save();
    }
    public void SaveBallData(List<Vector2> ballPos,List<int> ballNum,int currentBallNum)
    {
        playerData.ballPos = ballPos;
        playerData.ballNum = ballNum;
        playerData.currentBallNum = currentBallNum;
        Save();
    }
    public List<Vector2> GetBallData(out List<int> ballNum,out int currentBallNum)
    {
        ballNum = playerData.ballNum;
        currentBallNum = playerData.currentBallNum;
        return playerData.ballPos;
    }
}
[System.Serializable]
public class PlayerData
{
    public bool isPackB;
    public int cash;
    public int coin;
    public int amazon;
    public int prop1Num;
    public int prop2Num;
    public int prop1NeedCoinNum;
    public int prop2NeedCoinNum;
    public int wheelTicket;
    public int todayCanGetCashTime;
    public string lastGetCashTime;
    public string lastUseFreeWheelTime;
    public int spinWheelTimeTotal;
    public int fallBallNum;
    public int score;
    public int bestScore;
    public int stage;
    public int currentBallNum;
    public List<Vector2> ballPos;
    public List<int> ballNum;
}
