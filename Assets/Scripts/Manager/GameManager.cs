using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public const float ballCircle = 209;
    public const float ballTextHeight = 96;
    public const float ballExplosionTime = 0.2f;
    public const float checkFailDelayTime = 4f;
    public const int canGetCashTimesPerDay = 30;
    public const int startWheelTicket = 10;
    public UIManager UIManager;
    public RectTransform popUIRootRect;
    private PlayerDataManager PlayerDataManager = null;
    public int UpgradeNeedScore = 0;
    private void Awake()
    {
        Instance = this;
        UIManager = gameObject.AddComponent<UIManager>();
        UIManager.Init(popUIRootRect, this);
        PlayerDataManager = new PlayerDataManager();
        UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.LoadingPanel);
        RefreshUpgradeNeedScore();
    }
    public int GetCurrentStageUpgradeNeedScore(int currentStage)
    {
        return ConfigManager.GetStageScoreData(currentStage).upgradeNeedScore;
    }
    public void AddScore(int value)
    {
        bool isBest = PlayerDataManager.SetScore(GetScore() + value);
        if (GetScore() >= UpgradeNeedScore)
        {
            PlayerDataManager.SetScore(0);
            PlayerDataManager.SetStage(GetStage() + 1);
            RefreshUpgradeNeedScore();
        }
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            _MenuPanel.RefreshScoreText();
            if (isBest)
                _MenuPanel.RefreshBestScoreText();
        }
    }
    public int GetScore()
    {
        return PlayerDataManager.GetScore();
    }
    public int GetBestScore()
    {
        return PlayerDataManager.GetBestScore();
    }
    public int GetStage()
    {
        return PlayerDataManager.GetStage();
    }
    private void RefreshUpgradeNeedScore()
    {
        UpgradeNeedScore = ConfigManager.GetStageScoreData(GetStage()).upgradeNeedScore;
    }
    public int GetCoin()
    {
        return PlayerDataManager.GetCoin();
    }
    public int GetCash()
    {
        return PlayerDataManager.GetCash();
    }
    public int GetAmazon()
    {
        return PlayerDataManager.GetAmazon();
    }
    public int GetPop1Num()
    {
        return PlayerDataManager.GetPop1Num();
    }
    public int GetPop2Num()
    {
        return PlayerDataManager.GetPop2Num();
    }
    public int GetTodayCanGetCashTime()
    {
        return PlayerDataManager.GetTodayCanGetCashTime();
    }
    public bool GetTodayHasFreeWheel()
    {
        return PlayerDataManager.GetTodayHasFreeWheel();
    }
    public int GetWheelTicket()
    {
        return PlayerDataManager.GetWheelTicket();
    }
    public void UseFreeWheel()
    {
        PlayerDataManager.UseFreeWheel();
    }
    public int AddCoin(int value)
    {
        int endValue= PlayerDataManager.AddCoin(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            _MenuPanel.RefreshCoinText();
        }
        return endValue;
    }
    public int AddCash(int value)
    {
        int endValue= PlayerDataManager.AddCash(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            _MenuPanel.RefreshCashText();
        }
        return endValue;
    }
    public int AddAmazon(int value)
    {
        int endValue = PlayerDataManager.AddAmazon(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            //_MenuPanel.RefreshCashText();
        }
        return endValue;
    }
    public int AddPop1Num(int value)
    {
        return PlayerDataManager.AddPop1Num(value);
    }
    public int AddPop2Num(int value)
    {
        return PlayerDataManager.AddPop2Num(value);
    }
    public int AddWheelTicket(int value)
    {
        return PlayerDataManager.AddWheelTicket(value);
    }
    public int AddBallFallNum(int value = 1)
    {
        return PlayerDataManager.AddFallBallNum(value);
    }
    public int ReduceTodayCanGetCashTime(int value = -1)
    {
        return PlayerDataManager.ReduceTodayCanGetCashTime(value);
    }
    public bool nextSlotsIsUpgradeSlots = false;
    public int RandomSlotsReward()
    {
        SlotsData slotsData = ConfigManager.GetSlotsData(PlayerDataManager.GetCash());
        int total = slotsData.cashWeight + slotsData.coinWeight;
        int result = Random.Range(0, total);
        if (result < slotsData.cashWeight && GetTodayCanGetCashTime() > 0)
            return -Random.Range(slotsData.cashRange.x, slotsData.cashRange.y);
        else
            return Random.Range(slotsData.coinRange.x, slotsData.coinRange.y);
    }
    public Reward WillReward_NoCashType = Reward.Null;
    public int WillReward_NoCashNum = 0;
    public void ShowConfirmRewardNoCashPanel(Reward type,int num)
    {
        WillReward_NoCashType = type;
        WillReward_NoCashNum = num;
        UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.RewardNoCashPanel);
    }
    public int WillReward_CashNum = 0;
    public void ShowConfirmRewardCashPanel(int num)
    {
        WillReward_CashNum = num;
        UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.RewardCashPanel);
    }
    private struct WheelRandom
    {
        public int index;
        public int maxRange;
    }
    private List<WheelData> allWheelDatas = null;
    private readonly List<WheelRandom> wheelRandomDatas = new List<WheelRandom>();
    public int RandomWheelReward()
    {
        int playTime = PlayerDataManager.GetWheelSpinTimeTotal() + 1;
        PlayerDataManager.AddSpinWheelTimeTotal();
        if(allWheelDatas is null)
            allWheelDatas = ConfigManager.GetWheelDatas();
        wheelRandomDatas.Clear();
        int total = 0;
        int count = allWheelDatas.Count;
        for(int i = 0; i < count; i++)
        {
            List<int> blackBox = allWheelDatas[i].blackbox;
            foreach(int time in blackBox)
            {
                if (time == playTime)
                    return i;
            }
            if (allWheelDatas[i].limitCash < 0 || (GetCash() < allWheelDatas[i].limitCash && GetTodayCanGetCashTime() > 0))
            {
                total += allWheelDatas[i].weight;
                WheelRandom random = new WheelRandom
                {
                    index = i,
                    maxRange = total
                };
                wheelRandomDatas.Add(random);
            }
        }

        int result = Random.Range(0, total);
        int randomCount = wheelRandomDatas.Count;
        for(int i = 0; i < randomCount; i++)
        {
            if (result < wheelRandomDatas[i].maxRange)
                return wheelRandomDatas[i].index;
        }
        Debug.LogError("随机转盘奖励错误");
        return -1;
    }
    public void SaveBallData(List<Vector2> ballPos,List<int> ballNum , int currentBallNum)
    {
        PlayerDataManager.SaveBallData(ballPos, ballNum, currentBallNum);
    }
    public List<Vector2> GetBallData(out List<int> ballNum,out int currentBallNum)
    {
        return PlayerDataManager.GetBallData(out ballNum, out currentBallNum);
    }
}
public enum Reward
{
    Null,
    Prop1,
    Prop2,
    Cash,
    Coin,
    Amazon,
    WheelTicket
}
