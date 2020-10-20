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
        private void OnOpenClick()
        {
            GameManager.PlayButtonClickSound();
            if (needAd)
            {
                Debug.Log("看广告开礼盒");
            }
            else
            {
                GameManager.SetHasGetFreeGift();
                Debug.Log("免费开礼盒");
            }
            Reward type = GameManager.RandomGiftReward(out int num);
            GameManager.ShowConfirmRewardPanel(type, num, needAd);
            UIManager.ClosePopPanel(this);
        }
        private void OnCloseClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ClosePopPanel(this);
        }
        bool needAd = false;
        protected override void OnStartShow()
        {
            needAd = GameManager.GetHasGetFreeGift();
            nothanksButton.gameObject.SetActive(needAd);
            adIcon.SetActive(needAd);
            openContentRect.localPosition = needAd ? new Vector3(41.5f, 3.1f, 0) : new Vector3(0, 3.1f, 0);
        }
    }
}
