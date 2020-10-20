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
        private void OnAdClick()
        {
            GameManager.PlayButtonClickSound();
            if (needAd)
                Debug.Log("看广告得双倍");
            else
                Debug.Log("免费得双倍");
            num *= 2;
            GetReward();
            UIManager.ClosePopPanel(this);
        }
        private void OnNothanksClick()
        {
            GameManager.PlayButtonClickSound();
            GetReward();
            UIManager.ClosePopPanel(this);
        }
        Reward type = Reward.Null;
        int num = 0;
        bool needAd = true;
        protected override void OnStartShow()
        {
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
                    GameManager.AddPop1Num(num);
                    break;
                case Reward.Prop2:
                    GameManager.AddPop2Num(num);
                    break;
                case Reward.Cash:
                    Debug.LogError("奖励类型错误，该面板不会奖励现金");
                    break;
                case Reward.Coin:
                    GameManager.AddCoin(num);
                    break;
                case Reward.Amazon:
                    GameManager.AddAmazon(num);
                    break;
            }
        }
    }
}
