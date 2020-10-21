using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace UI
{
    public class UI_RewardNoCashPanel : UI_PopPanelBase
    {
        public Image rewardIcon;
        public Text rewardNum;
        public Button adButton;
        public Button nothanksButton;
        public GameObject adicon;
        public RectTransform adContentRect;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.RewardNoCashPanel;
            adButton.onClick.AddListener(OnAdClick);
            nothanksButton.onClick.AddListener(OnNothanksClick);
        }
        int clickAdTime = 0;
        private void OnAdClick()
        {
            GameManager.PlayButtonClickSound();
            if (needAd)
            {
                clickAdTime++;
                GameManager.PlayRV(OnAdGetDoubleCallback, clickAdTime, "获得双倍" + type, OnNothanksClick);
            }
            else
                OnAdGetDoubleCallback();
        }
        private void OnAdGetDoubleCallback()
        {
            num *= 2;
            GetReward();
            UIManager.ClosePopPanel(this);
        }
        private void OnNothanksClick()
        {
            GameManager.PlayButtonClickSound();
            GameManager.PlayIV("放弃双倍奖励" + type, OnNothanksIVCallback);
        }
        private void OnNothanksIVCallback()
        {
            GetReward();
            UIManager.ClosePopPanel(this);
        }
        Reward type = Reward.Null;
        int num = 0;
        bool needAd = true;
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            type = GameManager.ConfirmReward_Type;
            num = GameManager.ConfirmRewrad_Num;
            needAd = GameManager.ConfirmReward_Needad;
            rewardIcon.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.RewardNoCash, type.ToString());
            rewardNum.text = "x" + num;
            adicon.SetActive(needAd);
            nothanksButton.gameObject.SetActive(needAd);
            adContentRect.localPosition = needAd ? new Vector3(49.3f, 5.7f, 0) : new Vector3(0, 5.7f, 0);
        }
        private void GetReward()
        {
            switch (type)
            {
                case Reward.Prop1:
                    Debug.LogError("奖励类型错误，该面板不会奖励道具1");
                    break;
                case Reward.Prop2:
                    Debug.LogError("奖励类型错误，该面板不会奖励道具2");
                    break;
                case Reward.Cash:
                    Debug.LogError("奖励类型错误，该面板不会奖励现金");
                    break;
                case Reward.Coin:
                    GameManager.AddCoin(num);
                    UIManager.FlyReward(Reward.Coin, num, transform.position);
                    break;
                case Reward.Amazon:
                    GameManager.AddAmazon(num);
                    UIManager.FlyReward(Reward.Amazon, num, transform.position);
                    break;
                case Reward.WheelTicket:
                    GameManager.AddWheelTicket(num);
                    UIManager.FlyReward(Reward.WheelTicket, num, transform.position);
                    break;
            }
        }
        protected override void OnEndClose()
        {
            if (GameManager.WillShowGift > 0)
            {
                GameManager.WillShowGift--;
                UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.GiftPanel);
            }
            else if (GameManager.WillShowSlots > 0)
            {
                GameManager.WillShowSlots--;
                UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.SlotsPanel);
            }
        }
    }
}
