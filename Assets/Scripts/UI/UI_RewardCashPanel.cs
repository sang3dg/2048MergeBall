using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_RewardCashPanel : UI_PopPanelBase
    {
        public Button closeButton;
        public Button getButton;
        public Text rewardNumText;
        public Text remainingTimes;
        public GameObject adicon;
        public RectTransform getContentRect;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.RewardCashPanel;
            closeButton.onClick.AddListener(OnCloseClick);
            getButton.onClick.AddListener(OnGetClick);
        }

        private void OnCloseClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ClosePopPanel(this);
        }
        private void OnGetClick()
        {
            GameManager.PlayButtonClickSound();
            if (needAd)
                Debug.Log("看广告得现金");
            else
                Debug.Log("免费得现金");
            GameManager.AddCash(rewardNum);
            UIManager.ClosePopPanel(this);
        }

        int rewardNum = 0;
        bool needAd = true;
        protected override void OnStartShow()
        {
            if (GameManager.ConfirmReward_Type != Reward.Cash)
            {
                Debug.LogError("奖励类型错误");
                return;
            }
            needAd = GameManager.ConfirmReward_Needad;
            rewardNum = GameManager.ConfirmRewrad_Num;
            rewardNumText.text = "$" + ToolManager.GetCashShowString(rewardNum);
            remainingTimes.text = "Remaining:" + GameManager.ReduceTodayCanGetCashTime();
            closeButton.gameObject.SetActive(needAd);
            adicon.SetActive(needAd);
            getContentRect.localPosition = needAd ? new Vector3(54.5f, 6, 0) : new Vector3(0, 6, 0);
        }
    }
}
