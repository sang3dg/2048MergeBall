using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_PopBuyPropPanel : UI_PopPanelBase
    {
        public Button closeButton;
        public Button coinBuyButton;
        public Button adBuyButton;
        public Image icon;
        Sprite prop1icon;
        Sprite prop2icon;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.BuyPropPanel;
            closeButton.onClick.AddListener(OnCloseClick);
            coinBuyButton.onClick.AddListener(OnCoinBuyClick);
            adBuyButton.onClick.AddListener(OnAdBuyClick);
            prop1icon = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.BuyProp, "prop1");
            prop2icon = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.BuyProp, "prop2");
        }
        private void OnCloseClick()
        {
            UIManager.ClosePopPanel(this);
        }
        private void OnCoinBuyClick()
        {
            GameManager.AddCoin(-needCoinNum);
            if (isProp1)
                GameManager.AddPop1Num(1);
            else
                GameManager.AddPop2Num(1);
            UIManager.ClosePopPanel(this);
        }
        private void OnAdBuyClick()
        {
            if (isProp1)
                GameManager.AddPop1Num(1);
            else
                GameManager.AddPop2Num(1);
            UIManager.ClosePopPanel(this);
        }
        bool isProp1 = false;
        int needCoinNum = 0;
        protected override void OnStartShow()
        {
            isProp1 = GameManager.WillBuyProp == Reward.Prop1;
            int coinNum = GameManager.GetCoin();
            needCoinNum = isProp1 ? GameManager.GetProp1NeedCoinNum() : GameManager.GetProp2NeedCoinNum();
            bool coinIsEnough = coinNum >= needCoinNum;
            icon.sprite = isProp1 ? prop1icon : prop2icon;
            coinBuyButton.gameObject.SetActive(coinIsEnough);
            adBuyButton.gameObject.SetActive(!coinIsEnough);
        }
    }
}
