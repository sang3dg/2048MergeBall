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
            openButton.onClick.AddListener(OnOpenClick);
            closeButton.onClick.AddListener(OnCloseClick);
            nothanksButton.onClick.AddListener(OnCloseClick);
        }
        int clickAdTime = 0;
        private void OnOpenClick()
        {
            GameManager.PlayButtonClickSound();
            if (needAd)
            {
                clickAdTime++;
                GameManager.PlayRV(OnOpenAdCallback, clickAdTime, "打开礼盒", OnCloseClick);
            }
            else
            {
                GameManager.SetHasGetFreeGift();
                OnOpenAdCallback();
            }
        }
        private void OnOpenAdCallback()
        {
            if (GameManager.isPropGift)
                GameManager.isPropGift = false;
            else
                GameManager.AddOpenGiftBallNum();
            Reward type = GameManager.RandomGiftReward(out int num);
            GameManager.ShowConfirmRewardPanel(type, num, needAd);
            UIManager.ClosePopPanel(this);
        }
        private void OnCloseClick()
        {
            GameManager.PlayButtonClickSound();
            GameManager.PlayIV("放弃礼盒", OnCloseIVCallback);
        }
        private void OnCloseIVCallback()
        {
            UIManager.ClosePopPanel(this);
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
        bool needAd = false;
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            needAd = GameManager.GetHasGetFreeGift();
            nothanksButton.gameObject.SetActive(needAd);
            adIcon.SetActive(needAd);
            closeButton.gameObject.SetActive(needAd);
            openContentRect.localPosition = needAd ? new Vector3(41.5f, 3.1f, 0) : new Vector3(0, 3.1f, 0);
        }
    }
}
