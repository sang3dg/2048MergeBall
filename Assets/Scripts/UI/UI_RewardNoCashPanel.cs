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
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.RewardNoCashPanel;
            adButton.onClick.AddListener(OnAdClick);
            nothanksButton.onClick.AddListener(OnNothanksClick);
        }
        private void OnAdClick()
        {
            num *= 2;
            GetReward();
            UIManager.ClosePopPanel(this);
        }
        private void OnNothanksClick()
        {
            GetReward();
            UIManager.ClosePopPanel(this);
        }
        Reward type = Reward.Null;
        int num = 0;
        protected override void OnStartShow()
        {
            type = GameManager.WillReward_NoCashType;
            num = GameManager.WillReward_NoCashNum;
            rewardIcon.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.RewardNoCash, GameManager.WillReward_NoCashType.ToString());
            rewardNum.text = "x" + GameManager.WillReward_NoCashNum;
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
                    GameManager.AddCash(num);
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
