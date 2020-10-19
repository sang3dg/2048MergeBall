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
    public const int originPropNeedCoinNum = 300;
    public const int propNeedCoinNumIncreaseStep = 50;
    public const int propNeedMaxCoinNum = 1000;
    public static bool isLoadingEnd = false;
    public AnimationCurve PopPanelScaleAnimation;
    public AnimationCurve PopPanelAlphaAnimation;
    public UIManager UIManager;
    public RectTransform popUIRootRect;
    public RectTransform menuRootRect;
    private PlayerDataManager PlayerDataManager = null;
    private ConfigManager ConfigManager = null;
    [System.NonSerialized]
    public int UpgradeNeedScore = 0;
    [System.NonSerialized]
    public Reward WillBuyProp = Reward.Null;
    private void Awake()
    {
        Instance = this;
        UIManager = gameObject.AddComponent<UIManager>();
        UIManager.Init(popUIRootRect, menuRootRect, this);
        PlayerDataManager = new PlayerDataManager();
        ConfigManager = new ConfigManager();
        UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.LoadingPanel);
        RefreshUpgradeNeedScore();
    }
    public bool GetIsPackB()
    {
        return PlayerDataManager.GetIsPackB();
    }
    public void SetIsPackB()
    {
        PlayerDataManager.SetIsPackB();
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
    public int GetProp1Num()
    {
        return PlayerDataManager.GetPop1Num();
    }
    public int GetProp2Num()
    {
        return PlayerDataManager.GetPop2Num();
    }
    public int GetProp1NeedCoinNum()
    {
        return PlayerDataManager.GetProp1NeedCoinNum();
    }
    public int GetProp2NeedCoinNum()
    {
        return PlayerDataManager.GetProp2NeedCoinNum();
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
            _MenuPanel.RefreshProp1();
            _MenuPanel.RefreshProp2();
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
        int currentPropNum = PlayerDataManager.AddPop1Num(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            _MenuPanel.RefreshProp1();
        }
        return currentPropNum;
    }
    public int AddPop2Num(int value)
    {
        int currentPropNum = PlayerDataManager.AddPop2Num(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            _MenuPanel.RefreshProp2();
        }
        return currentPropNum;
    }
    public int AddWheelTicket(int value)
    {
        return PlayerDataManager.AddWheelTicket(value);
    }
    public int AddBallFallNum(int value = 1)
    {
        return PlayerDataManager.AddFallBallNum(value);
    }
    public int UseProp1ByCoin()
    {
        int nextNeedCoin = GetProp1NeedCoinNum() + propNeedCoinNumIncreaseStep;
        nextNeedCoin = Mathf.Clamp(nextNeedCoin, 0, propNeedMaxCoinNum);
        PlayerDataManager.SetProp1NeedCoinNum(nextNeedCoin);
        return nextNeedCoin;
    }
    public int UseProp2ByCoin()
    {
        int nextNeedCoin = GetProp2NeedCoinNum() + propNeedCoinNumIncreaseStep;
        nextNeedCoin = Mathf.Clamp(nextNeedCoin, 0, propNeedMaxCoinNum);
        PlayerDataManager.SetProp2NeedCoinNum(nextNeedCoin);
        return nextNeedCoin;
    }
    public void ResetPropNeedCoinNum()
    {
        PlayerDataManager.SetProp1NeedCoinNum(originPropNeedCoinNum);
        PlayerDataManager.SetProp2NeedCoinNum(originPropNeedCoinNum);
    }
    public void ClearBallFallNum()
    {
        PlayerDataManager.ClearFallBallNum();
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
    public Reward ConfirmReward_Type = Reward.Null;
    public int ConfirmRewrad_Num = 0;
    public void ShowConfirmRewardPanel(Reward type, int num)
    {
        ConfirmReward_Type = type;
        ConfirmRewrad_Num = num;
        UIManager.ShowPopPanelByType(type == Reward.Cash ? UI_Panel.UI_PopPanel.RewardCashPanel : UI_Panel.UI_PopPanel.RewardNoCashPanel);
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
    public int RandomGiftNeedFallBall()
    {
        ClearBallFallNum();
        bool isPackB = GetIsPackB();
        if (isPackB)
        {
            GiftDataB giftDataB = ConfigManager.GetGiftBData(GetCash());
            return Random.Range(giftDataB.fallBallNumRange.x, giftDataB.fallBallNumRange.y);
        }
        else
        {
            GiftDataA giftDataA = ConfigManager.GetGiftAData(GetStage());
            return Random.Range(giftDataA.fallBallNumRange.x, giftDataA.fallBallNumRange.y);
        }
    }
    public Reward RandomGiftReward(out int rewardNum)
    {
        bool isPackB = GetIsPackB();
        if (isPackB)
        {
            GiftDataB giftDataB = ConfigManager.GetGiftBData(GetCash());
            if (GetTodayCanGetCashTime() <= 0)
            {
                rewardNum = Random.Range(giftDataB.rewardCoinNumRange.x, giftDataB.rewardCoinNumRange.y);
                return Reward.Coin;
            }
            int total = giftDataB.cashWeight + giftDataB.coinWeight;
            int result = Random.Range(0, total);
            if (result < giftDataB.cashWeight)
            {
                rewardNum = Random.Range(giftDataB.rewardCashNumRange.x, giftDataB.rewardCashNumRange.y);
                return Reward.Cash;
            }
            else
            {
                rewardNum = Random.Range(giftDataB.rewardCoinNumRange.x, giftDataB.rewardCoinNumRange.y);
                return Reward.Coin;
            }
        }
        else
        {
            GiftDataA giftDataA = ConfigManager.GetGiftAData(GetStage());
            rewardNum = Random.Range(giftDataA.rewardCoinNumRange.x, giftDataA.rewardCoinNumRange.y);
            return Reward.Coin;
        }
    }
    public void ContinueGame()
    {
        MainController.Instance.OnContinueGame();
    }
    public void RestartGame()
    {
        MainController.Instance.OnRestartGame();
        PlayerDataManager.SetScore(0);
        PlayerDataManager.SetStage(0);
        RefreshUpgradeNeedScore();
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            _MenuPanel.RefreshScoreText();
            _MenuPanel.ResetStageProgress();
            _MenuPanel.SetStageInfo();
        }
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
