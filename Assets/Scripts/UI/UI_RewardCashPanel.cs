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
        public Image cashIcon;
        public GameObject reedemTip;
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
            GameManager.PlayIV("放弃现金奖励");
            UIManager.ClosePopPanel(this);
        }
        int clickAdTime = 0;
        private void OnGetClick()
        {
            GameManager.PlayButtonClickSound();
            if (needAd)
            {
                clickAdTime++;
                GameManager.PlayRV(OnAdGetCallback, clickAdTime, "获得现金", OnCloseClick);
            }
            else
                OnAdGetCallback();
        }
        private void OnAdGetCallback()
        {
            GameManager.AddCash(rewardNum);
            UIManager.FlyReward(Reward.Cash, rewardNum, transform.position);
            UIManager.ClosePopPanel(this);
        }
        int rewardNum = 0;
        bool needAd = true;
        Coroutine closeDelay = null;
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            if (GameManager.ConfirmReward_Type != Reward.Cash)
            {
                Debug.LogError("奖励类型错误");
                return;
            }
            bool isPackB = GameManager.GetIsPackB();
            cashIcon.sprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.RewardCash, "manycash" + (isPackB ? "B" : "A"));
            needAd = GameManager.ConfirmReward_Needad;
            rewardNum = GameManager.ConfirmRewrad_Num;
            rewardNumText.text = (isPackB ? "$" : "") + ToolManager.GetCashShowString(rewardNum);
            reedemTip.SetActive(isPackB);
            remainingTimes.text = "Remaining:" + GameManager.ReduceTodayCanGetCashTime();
            closeButton.gameObject.SetActive(needAd);
            adicon.SetActive(needAd);
            getContentRect.localPosition = needAd ? new Vector3(54.5f, 6, 0) : new Vector3(0, 6, 0);
            if (needAd)
            {
                closeDelay= StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(closeButton.gameObject));
            }
        }
        protected override void OnEndClose()
        {
            if (needAd)
                StopCoroutine(closeDelay);
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
