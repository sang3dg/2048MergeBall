using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_MenuPanel : UI_PanelBase
    {
        public Button settingButton;
        public Button cashButton;
        public Button coinButton;
        public Button wheelButton;
        public Button propButton1;
        public Button propButton2;
        public Text cashText;
        public Text coinText;
        public Text scoreText;
        public Text bestScoreText;
        public Text currentStageText;
        public Text nextStageText;
        public Text prop1NumText;
        public Text prop2NumText;
        public Text prop1NeedNumText;
        public Text prop2NeedNumText;
        public Image prop1NeedIcon;
        public Image prop2NeedIcon;
        public Image stageProgressFillImage;
        [Space(15)]
        public Button guideMaskButton;
        public CanvasGroup guideCG;
        public Image guideCard;
        public Image guideHandle;
        Sprite adIcon;
        Sprite coinIcon;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.MenuPanel;
            settingButton.onClick.AddListener(OnSettingButtonClick);
            cashButton.onClick.AddListener(OnCashButtonClick);
            coinButton.onClick.AddListener(OnCoinButtonClick);
            wheelButton.onClick.AddListener(OnWheelButtonClick);
            propButton1.onClick.AddListener(OnProp1ButtonClick);
            propButton2.onClick.AddListener(OnProp2ButtonClick);
            guideMaskButton.onClick.AddListener(OnGuideBGClick);
            currentProgress = Mathf.Floor(GameManager.CurrentLevelProgress);
            stageProgressFillImage.fillAmount = FillStart;
            adIcon = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Menu, "prop_ad");
            coinIcon = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Menu, "prop_coin");
        }
        private void OnSettingButtonClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.SettingPanel);
        }
        private void OnCashButtonClick()
        {
            GameManager.PlayButtonClickSound();
            if (isWaitingGuide)
                return;
            if (isGuiding)
                OnGuideBGClick();
        }
        private void OnCoinButtonClick()
        {
            GameManager.PlayButtonClickSound();
        }
        private void OnWheelButtonClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.WheelPanel);
        }
        private void OnProp1ButtonClick()
        {
            GameManager.PlayButtonClickSound();
            if (hasProp1)
            {
                GameManager.AddPop1Num(-1);
                GameManager.UseProp1();
            }
            else
            {
                int needCoin = GameManager.GetProp1NeedCoinNum();
                if (GameManager.GetCoin() >= needCoin)
                {
                    GameManager.WillBuyProp = Reward.Prop1;
                    UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.BuyPropPanel);
                }
                else
                {
                    Debug.Log("观看广告获得道具1");
                    GameManager.AddPop1Num(1);
                }
            }
        }
        private void OnProp2ButtonClick()
        {
            GameManager.PlayButtonClickSound();
            if (hasProp2)
            {
                if(GameManager.UseProp2())
                    GameManager.AddPop2Num(-1);
            }
            else
            {
                int needCoin = GameManager.GetProp2NeedCoinNum();
                if (GameManager.GetCoin() >= needCoin)
                {
                    GameManager.WillBuyProp = Reward.Prop2;
                    UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.BuyPropPanel);
                }
                else
                {
                    Debug.Log("观看广告获得道具2");
                    GameManager.AddPop2Num(1);
                }
            }
        }
        public void RefreshCashText()
        {
            cashText.text = ToolManager.GetCashShowString(GameManager.GetCash());
        }
        public void RefreshCoinText()
        {
            coinText.text = GameManager.GetCoin().ToString();
        }
        public void RefreshScoreText()
        {
            scoreText.text = GameManager.GetScore().ToString();
            RefreshStageProgress();
        }
        public void RefreshBestScoreText()
        {
            bestScoreText.text = GameManager.GetBestScore().ToString();
        }
        public void SetStageInfo()
        {
            currentStageText.text = GameManager.GetLevel().ToString();
            nextStageText.text = (GameManager.GetLevel() + 1).ToString();
        }
        bool hasProp1 = false;
        bool hasProp2 = false;
        public void RefreshProp1()
        {
            int prop1Num = GameManager.GetProp1Num();
            prop1NumText.text = "x" + prop1Num;
            hasProp1 = prop1Num > 0;
            if (prop1Num <= 0)
            {
                int needCoin = GameManager.GetProp1NeedCoinNum();
                if (GameManager.GetCoin() >= needCoin)
                {
                    prop1NeedIcon.sprite = coinIcon;
                    prop1NeedNumText.text = needCoin.ToString();
                }
                else
                {
                    prop1NeedIcon.sprite = adIcon;
                    prop1NeedNumText.text = "FREE";
                }
                prop1NeedIcon.gameObject.SetActive(true);
                prop1NeedNumText.gameObject.SetActive(true);
            }
            else
            {
                prop1NeedIcon.gameObject.SetActive(false);
                prop1NeedNumText.gameObject.SetActive(false);
            }
        }
        public void RefreshProp2()
        {
            int prop2Num = GameManager.GetProp2Num();
            prop2NumText.text = "x" + prop2Num;
            hasProp2 = prop2Num > 0;
            if (prop2Num <= 0)
            {
                int needCoin = GameManager.GetProp2NeedCoinNum();
                if (GameManager.GetCoin() >= needCoin)
                {
                    prop2NeedIcon.sprite = coinIcon;
                    prop2NeedNumText.text = needCoin.ToString();
                }
                else
                {
                    prop2NeedIcon.sprite = adIcon;
                    prop2NeedNumText.text = "FREE";
                }
                prop2NeedIcon.gameObject.SetActive(true);
                prop2NeedNumText.gameObject.SetActive(true);
            }
            else
            {
                prop2NeedIcon.gameObject.SetActive(false);
                prop2NeedNumText.gameObject.SetActive(false);
            }
        }
        const float FillStart = 0.33f;
        const float FillEnd = 0.673f;
        const float FillLength = FillEnd - FillStart;
        public void RefreshStageProgress()
        {
            targetProgress = GameManager.CurrentLevelProgress;
        }
        public void ResetStageProgress()
        {
            StopCoroutine("StageProgressAnimation");
            stageProgressFillImage.fillAmount = FillStart;
            currentProgress = 0;
            targetProgress = 0;
            StartCoroutine("StageProgressAnimation");
        }
        const float ProgressAnimationSpeed = 1f;
        private float targetProgress = 0;
        private float currentProgress = 0;
        private IEnumerator StageProgressAnimation()
        {
            while (true)
            {
                if (currentProgress<targetProgress)
                {
                    float delta = Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f) * ProgressAnimationSpeed;
                    currentProgress += delta;
                    float nextFillAmount = FillStart + FillLength*(currentProgress - Mathf.Floor(currentProgress));
                    if (stageProgressFillImage.fillAmount > nextFillAmount)
                    {
                        GameManager.nextSlotsIsUpgradeSlots = true;
                        UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.SlotsPanel);
                        SetStageInfo();
                        GameManager.Instance.SpawnAGiftBall();
                    }
                    stageProgressFillImage.fillAmount = nextFillAmount;
                }
                yield return null;
            }
        }
        protected override IEnumerator Show()
        {
            _CanvasGroup.alpha = 1;
            _CanvasGroup.blocksRaycasts = true;
            RefreshCashText();
            RefreshCoinText();
            RefreshScoreText();
            RefreshBestScoreText();
            RefreshProp1();
            RefreshProp2();
            SetStageInfo();
            StartCoroutine("StageProgressAnimation");
            yield return null;
        }
        protected override IEnumerator Close()
        {
            _CanvasGroup.alpha = 0;
            _CanvasGroup.blocksRaycasts = false;
            yield return null;
        }
        bool isGuiding = false;
        bool isWaitingGuide = false;
        public void ShowGuideCash()
        {
            StartCoroutine("ShowGuideDelayHide");
        }
        private IEnumerator ShowGuideDelayHide()
        {
            isGuiding = true;
            isWaitingGuide = true;
            guideCG.transform.SetAsLastSibling();
            cashButton.transform.SetAsLastSibling();
            guideHandle.transform.position = new Vector3(cashButton.transform.position.x, guideHandle.transform.position.y, guideHandle.transform.position.z);
            guideCG.alpha = 1;
            guideCG.blocksRaycasts = true;
            guideCard.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Menu, "guide1");
            guideHandle.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Menu, "guideHandle1");
            yield return new WaitForSeconds(1);
            isWaitingGuide = false;
        }
        private void OnGuideBGClick()
        {
            if (isWaitingGuide) return;
            guideCG.alpha = 0;
            guideCG.blocksRaycasts = false;
            isGuiding = false;
        }
    }
}
