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
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.RewardCashPanel;
            closeButton.onClick.AddListener(OnCloseClick);
            getButton.onClick.AddListener(OnGetClick);
        }

        private void OnCloseClick()
        {
            UIManager.ClosePopPanel(this);
        }
        private void OnGetClick()
        {
            GameManager.AddCash(rewardNum);
            UIManager.ClosePopPanel(this);
        }

        int rewardNum = 0;
        protected override void OnStartShow()
        {
            rewardNum = GameManager.WillReward_CashNum;
            rewardNumText.text = "$" + ToolManager.GetCashShowString(rewardNum);
            remainingTimes.text = "Remaining:" + GameManager.ReduceTodayCanGetCashTime();
        }
    }
}
