using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_GfitPanel : UI_PopPanelBase
    {
        public Button openButton;
        public Button nothanksButton;
        public Button closeButton;
        public GameObject adIcon;
        public RectTransform openContentRect;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.GiftPanel;
        }
        private void OnOpenClick()
        {
            Reward type = GameManager.RandomGiftReward(out int num);
            GameManager.ShowConfirmRewardPanel(type, num);
        }
    }
}
