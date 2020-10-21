using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.Events;

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
    public const int levelStartTargetBallNum = 128;
    public static bool isLoadingEnd = false;
    public AnimationCurve PopPanelScaleAnimation;
    public AnimationCurve PopPanelAlphaAnimation;
    public RectTransform popUIRootRect;
    public RectTransform menuRootRect;
    public GameObject audioRoot;

    [System.NonSerialized]
    public UIManager UIManager;
    private PlayerDataManager PlayerDataManager = null;
    private ConfigManager ConfigManager = null;
    private LevelManager LevelManager = null;
    private AudioManager AudioManager = null;
    [System.NonSerialized]
    public int UpgradeNeedScore = 0;
    [System.NonSerialized]
    public float CurrentLevelProgress = 0;
    [System.NonSerialized]
    public Reward WillBuyProp = Reward.Null;
    [System.NonSerialized]
    public int WillShowSlots = 0;
    [System.NonSerialized]
    public int WillShowGift = 0;
    private void Awake()
    {
        Instance = this;
        isLoadingEnd = false;
        UIManager = gameObject.AddComponent<UIManager>();
        LevelManager = gameObject.GetComponent<LevelManager>();
        UIManager.Init(popUIRootRect, menuRootRect, this);

        PlayerDataManager = new PlayerDataManager();
        ConfigManager = new ConfigManager();
        AudioManager = new AudioManager(audioRoot);

        UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.LoadingPanel);
        RefreshUpgradeNeedScore();
        CurrentLevelProgress = GetLevel() + GetScore() / (UpgradeNeedScore * 1f);
        LevelManager.SetTargetBallNum(PlayerDataManager.GetLevelTargetBallNum());
    }
    public bool GetIsPackB()
    {
#if UNITY_EDITOR
        return true;
#endif
        return PlayerDataManager.GetIsPackB();
    }
    public void SetIsPackB()
    {
        if (!GetIsPackB())
        {
            PlayerDataManager.SetIsPackB();
            SendAdjustChangePackBEvent();
        }
    }
    public int GetCurrentStageUpgradeNeedScore(int currentStage)
    {
        return ConfigManager.GetStageScoreData(currentStage).upgradeNeedScore;
    }
    public void AddScore(int value)
    {
        bool isBest = PlayerDataManager.SetScore(GetScore() + value);
        SetCurrentLevelScore(GetCurrentLevelScore() + value);
        if (!GetWhetherRateus() && GetScore() >= 1000)
        {
            SetHasRateus();
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.RateusPanel);
        }
        if (GetCurrentLevelScore() >= UpgradeNeedScore)
        {
            SetCurrentLevelScore(0);
            PlayerDataManager.SetLevel(GetLevel() + 1);
            RefreshUpgradeNeedScore();
        }
        CurrentLevelProgress = GetLevel() + GetCurrentLevelScore() / (UpgradeNeedScore * 1f);
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
    public int GetCurrentLevelScore()
    {
        return PlayerDataManager.playerData.currentLevelScore;
    }
    public void SetCurrentLevelScore(int value)
    {
        PlayerDataManager.playerData.currentLevelScore = value;
        PlayerDataManager.Save();
    }
    public int GetBestScore()
    {
        return PlayerDataManager.GetBestScore();
    }
    public int GetLevel()
    {
        return PlayerDataManager.GetLevel();
    }
    private void RefreshUpgradeNeedScore()
    {
        UpgradeNeedScore = ConfigManager.GetStageScoreData(GetLevel()).upgradeNeedScore;
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
    public int GetTargetLevelBallNum()
    {
        return PlayerDataManager.GetLevelTargetBallNum();
    }
    public void LevelUp()
    {
        LevelManager.WhenLevelUp();
        PlayerDataManager.AddLevelTargetBallNum();
    }
    public void RefreshTargetBallNum()
    {
        LevelManager.SetTargetBallNum(PlayerDataManager.GetLevelTargetBallNum());
    }
    public int AddCoin(int value)
    {
        int endValue= PlayerDataManager.AddCoin(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            if (value < 0)
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
            if (value < 0)
                _MenuPanel.RefreshCashText();
            if (!PlayerDataManager.GetWhetherGuideCash())
            {
                PlayerDataManager.SetHasGuideCash();
                UI_PanelBase wheelPanel = UIManager.GetUIPanel(UI_Panel.UI_PopPanel.WheelPanel);
                if (wheelPanel != null)
                {
                    UIManager.ClosePopPanel(wheelPanel);
                }
                _MenuPanel.ShowGuideCash();
            }
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
    public int AddProp1Num(int value)
    {
        int currentPropNum = PlayerDataManager.AddPop1Num(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            if (value < 0)
                _MenuPanel.RefreshProp1();
        }
        return currentPropNum;
    }
    public int AddProp2Num(int value)
    {
        int currentPropNum = PlayerDataManager.AddPop2Num(value);
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            if (value < 0)
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
        PlayerDataManager.playerData.logPerTenBall++;
        if (PlayerDataManager.playerData.logPerTenBall > 0 && PlayerDataManager.playerData.logPerTenBall % 10 == 0)
            SendAdjustPerTenBallEvent();

        int operationNum= PlayerDataManager.AddFallBallNum(value);
        int targetNum = RandomGiftNeedFallBall();
        if (operationNum >= targetNum)
        {
            ClearBallFallNum();
            SpawnAGiftBall();
        }
        return operationNum;
    }
    [System.NonSerialized]
    public bool isPropGift = false;
    public void UseProp1()
    {
        StartCoroutine(DelayShowUsePropGiftPanel());
        SendAdjustPropChangeEvent(1, 0);
        MainController.Instance.UseProp1();
    }
    public bool UseProp2()
    {
        StartCoroutine(DelayShowUsePropGiftPanel());
        SendAdjustPropChangeEvent(2, 0);
        return MainController.Instance.UseProp2();
    }
    private IEnumerator DelayShowUsePropGiftPanel()
    {
        yield return new WaitForSeconds(0.5f);
        isPropGift = true;
        if (!UIManager.PanelWhetherShowAnyone() && WillShowGift <= 0)
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.GiftPanel);
        else
            WillShowGift++;
    }
    public int IncreaseByProp1NeedCoin()
    {
        int nextNeedCoin = GetProp1NeedCoinNum() + propNeedCoinNumIncreaseStep;
        nextNeedCoin = Mathf.Clamp(nextNeedCoin, 0, propNeedMaxCoinNum);
        PlayerDataManager.SetProp1NeedCoinNum(nextNeedCoin);
        return nextNeedCoin;
    }
    public int IncreaseByProp2NeedCoin()
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
    public bool ConfirmReward_Needad = true;
    public void ShowConfirmRewardPanel(Reward type, int num, bool needAd = true)
    {
        ConfirmReward_Type = type;
        ConfirmRewrad_Num = num;
        ConfirmReward_Needad = needAd;
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
        bool isPackB = GetIsPackB();
        if (isPackB)
        {
            GiftDataB giftDataB = ConfigManager.GetGiftBData(GetCash());
            return Random.Range(giftDataB.fallBallNumRange.x, giftDataB.fallBallNumRange.y);
        }
        else
        {
            GiftDataA giftDataA = ConfigManager.GetGiftAData(GetLevel());
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
            GiftDataA giftDataA = ConfigManager.GetGiftAData(GetLevel());
            rewardNum = Random.Range(giftDataA.rewardCoinNumRange.x, giftDataA.rewardCoinNumRange.y);
            return Reward.Coin;
        }
    }
    public void ContinueGame()
    {
        MainController.Instance.OnContinueGame();
        SendAdjustGameOverEvent(true);
    }
    public void RestartGame()
    {
        PlayerDataManager.playerData.logRestartTime++;
        MainController.Instance.OnRestartGame();
        PlayerDataManager.SetScore(0);
        PlayerDataManager.SetLevel(0);
        RefreshUpgradeNeedScore();
        PlayerDataManager.SetProp1NeedCoinNum(originPropNeedCoinNum);
        PlayerDataManager.SetProp2NeedCoinNum(originPropNeedCoinNum);
        PlayerDataManager.ReSetLevelTargetBallNum();
        UI_MenuPanel _MenuPanel = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
        if (_MenuPanel != null)
        {
            _MenuPanel.RefreshScoreText();
            _MenuPanel.ResetStageProgress();
            _MenuPanel.SetStageInfo();
            _MenuPanel.RefreshProp1();
            _MenuPanel.RefreshProp2();
        }
        LevelManager.SetTargetBallNum(PlayerDataManager.GetLevelTargetBallNum());
    }
    public void WhenLevelUpAnimationEnd()
    {

    }
    public void WhenLoadingGameEnd()
    {
        isLoadingEnd = true;
        if (GetWhetherFirstPlay())
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.GiftPanel);
        MainController.Instance.LoadSaveData();
        stopGuideGame = false;
        CheckGuideGameAndShow();
    }
    public void WhenGetGfitBall()
    {
        if (!UIManager.PanelWhetherShowAnyone() && WillShowGift <= 0)
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.GiftPanel);
        else
            WillShowGift++;
    }
    public void SpawnAGiftBall()
    {
        MainController.Instance.SpawnNewGiftBall();
    }
    public bool GetWhetherFirstPlay()
    {
        return PlayerDataManager.GetWhetherFirstPlay();
    }
    public void SetFirstPlayFalse()
    {
        PlayerDataManager.SetFirstPlayFalse();
    }
    public bool GetWhetherRateus()
    {
        return PlayerDataManager.GetWhetherRateus();
    }
    public void SetHasRateus()
    {
        PlayerDataManager.SetHasRateus();
    }
    public bool GetHasGetFreeGift()
    {
        return PlayerDataManager.playerData.hasGetFreeGift;
    }
    public void SetHasGetFreeGift()
    {
        PlayerDataManager.playerData.hasGetFreeGift = true;
        PlayerDataManager.Save();
    }
    public void AddGiftBallAppearTime(int num = 1)
    {
        PlayerDataManager.playerData.logGiftBallAppearTime += num;
        PlayerDataManager.Save();
    }
    public void AddOpenGiftBallNum(int num = 1)
    {
        PlayerDataManager.playerData.logOpenGiftBallTime += num;
        PlayerDataManager.Save();
    }
    public void AddSpinWheelTime(int num = 1)
    {
        PlayerDataManager.playerData.logSpinWheelTime += num;
        PlayerDataManager.Save();
    }
    public void AddSpinSlotsTime(int num = 1)
    {
        PlayerDataManager.playerData.logSpinSlotsTime += num;
        PlayerDataManager.Save();
    }

    public RectTransform hand;
    private bool stopGuideGame = false;
    private void CheckGuideGameAndShow()
    {
        if (PlayerDataManager.playerData.hasGuideGame) return;
        StartCoroutine("ShakeGuidegameHand");
    }
    public void StopGuideGame()
    {
        stopGuideGame = true;
        PlayerDataManager.playerData.hasGuideGame = true;
        PlayerDataManager.Save();
    }
    IEnumerator ShakeGuidegameHand()
    {
        hand.gameObject.SetActive(true);
        bool isRight = false;
        float leftX = -144;
        float rightX = 144;
        float y = hand.localPosition.y;
        hand.localPosition = new Vector3(leftX, y, 0);
        while (!stopGuideGame)
        {
            yield return null;
            float offset = Time.deltaTime*20;
            hand.Translate(new Vector3(isRight ? -offset : offset, 0, 0));
            if (hand.localPosition.x >= rightX)
                isRight = true;
            else if (hand.localPosition.x <= leftX)
                isRight = false;
        }
        hand.gameObject.SetActive(false);
    }


    public bool GetMusicOn()
    {
        return PlayerDataManager.playerData.musicOn;
    }
    public void SetSaveMusicState(bool isOn)
    {
        PlayerDataManager.playerData.musicOn = isOn;
        PlayerDataManager.Save();
        AudioManager.SetMusicState(isOn);
    }
    public bool GetSoundOn()
    {
        return PlayerDataManager.playerData.soundOn;
    }
    public void SetSaveSoundState(bool isOn)
    {
        PlayerDataManager.playerData.soundOn = isOn;
        PlayerDataManager.Save();
        AudioManager.SetSoundState(isOn);
    }
    public void PlayButtonClickSound()
    {
        AudioManager.PlayOneShot(AudioPlayArea.Button);
    }
    public AudioSource PlaySpinSound()
    {
        return AudioManager.PlayLoop(AudioPlayArea.Spin);
    }
    public void PlayMergeBallCombeSound(int combe)
    {
        switch (combe)
        {
            case 1:
                AudioManager.PlayOneShot(AudioPlayArea.Combo1);
                break;
            case 2:
                AudioManager.PlayOneShot(AudioPlayArea.Combo2);
                break;
            case 3:
                AudioManager.PlayOneShot(AudioPlayArea.Combo3);
                break;
            default:
                AudioManager.PlayOneShot(AudioPlayArea.Combo3);
                break;
        }
    }
    public void PlayFlyOverSound()
    {
        AudioManager.PlayOneShot(AudioPlayArea.FlyOver);
    }
    public void PlayRV(System.Action callback, int clickTime, string des, System.Action failCallback = null)
    {
        Ads._instance.ShowRewardVideo(callback, clickTime, des,failCallback);
    }
    public void PlayIV(string des,System.Action callback = null)
    {
        Ads._instance.ShowInterstialAd(callback, des);
    }

    public void SendAdjustGameStartEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_open,
            ("install_version", "1")
            );
    }
    public void SendAdjustPlayAdEvent(bool hasAd, bool isRewardAd, string adByWay)
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(hasAd ? AdjustEventLogger.TOKEN_ad : AdjustEventLogger.TOKEN_noads,
            //广告位置
            ("id", adByWay),
            //广告类型，0插屏1奖励视频
            ("type", isRewardAd ? "1" : "0"),
            //累计美元
            ("other_int1", GetCash().ToString()),
            //当前金币
            ("other_int2", GetCoin().ToString())
            );
    }
    public void SendAdjustPerTenBallEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_stage_end,
            ("id", (PlayerDataManager.playerData.logPerTenBall / 10).ToString()),
            ("reason", MainController.Instance.BallMaxNum.ToString()),
            //累计美元
            ("other_int1", GetCash().ToString()),
            //当前金币
            ("other_int2", GetCoin().ToString())
            );
    }
    public void SendAdjustGameOverEvent(bool passive)
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_stage_over,
            ("id", PlayerDataManager.playerData.logRestartTime.ToString()),
            ("next_stage_id", PlayerDataManager.GetLevel().ToString()),
            ("result", passive ? "0" : "1"),
            ("reason", PlayerDataManager.GetLevelTargetBallNum().ToString()),
            //累计美元
            ("other_int1", GetCash().ToString()),
            //当前金币
            ("other_int2", GetCoin().ToString())
            );
    }
    public void SendAdjustPropChangeEvent(int propID, int propChangeType)
    {
#if UNITY_EDITOR
        return;
#endif
        string value;
        if (propChangeType == 0)
            value = "-1";
        else
            value = "+1";
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_item_change,
            ("id", propID.ToString()),
            ("type", propChangeType.ToString()),
            ("stage_id", PlayerDataManager.GetLevelTargetBallNum().ToString()),
            ("value", value),
            //累计美元
            ("other_int1", GetCash().ToString()),
            //当前金币
            ("other_int2", GetCoin().ToString())
            );
    }
    public void SendFBAttributeEvent(string uri)
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_deeplink,
            ("link", uri),
            ("order_id", uri),
            //累计美元
            ("other_int1", GetCash().ToString()),
            //当前金币
            ("other_int2", GetCoin().ToString())
            );
    }
    public void SendAdjustChangePackBEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_packb,
            //累计美元
            ("other_int1", GetCash().ToString()),
            //当前金币
            ("other_int2", GetCoin().ToString())
            );
    }
    public void SendAdjustSpawnGiftballEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_box,
            ("id", PlayerDataManager.playerData.logGiftBallAppearTime.ToString()),
            ("time", PlayerDataManager.playerData.logOpenGiftBallTime.ToString()),
            //累计美元
            ("other_int1", GetCash().ToString()),
            //当前金币
            ("other_int2", GetCoin().ToString())
            );
    }
    public void SendAdjustSpinWheelEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_wheel,
            ("id", PlayerDataManager.playerData.logSpinWheelTime.ToString()),
            //累计美元
            ("other_int1", GetCash().ToString()),
            //当前金币
            ("other_int2", GetCoin().ToString())
            );
    }
    public void SendAdjustSpinSlotsEvent()
    {
#if UNITY_EDITOR
        return;
#endif
        AdjustEventLogger.Instance.AdjustEvent(AdjustEventLogger.TOKEN_slots,
            ("id", PlayerDataManager.playerData.logSpinSlotsTime.ToString()),
            //累计美元
            ("other_int1", GetCash().ToString()),
            //当前金币
            ("other_int2", GetCoin().ToString())
            );
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
