using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_BuyEnergyPanel : UI_PopPanelBase
    {
        public Button closeButton;
        public Button adbuyButton;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.BuyEnergyPanel;
            closeButton.onClick.AddListener(OnCloseClick);
            adbuyButton.onClick.AddListener(OnAdbuyClick);
        }
        private void OnCloseClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ClosePopPanel(this);
        }
        int clickAdTime = 0;
        private void OnAdbuyClick()
        {
            GameManager.PlayButtonClickSound();
            if (!GameManager.CheckHasBuyEnergyTime())
            {
                TipsManager.Instance.ShowTips("Not enough times to buy energy.");
                return;
            }
            clickAdTime++;
            GameManager.PlayRV(OnAdbuyCallback, clickAdTime, "购买体力");
        }
        private void OnAdbuyCallback()
        {
            GameManager.AddEnergy(GameManager.addEnergyPerAd);
            GameManager.AddBuyEnergyTime();
            UIManager.FlyReward(Reward.Energy, GameManager.addEnergyPerAd, transform.position);
            UIManager.ClosePopPanel(this);
        }
        Coroutine closeDelay = null;
        protected override void OnStartShow()
        {
            closeDelay = StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(closeButton.gameObject));
        }
        protected override void OnEndClose()
        {
            StopCoroutine(closeDelay);
            MainController.Instance.hasShowBuyEnergyPanel = false;
        }
    }
}
