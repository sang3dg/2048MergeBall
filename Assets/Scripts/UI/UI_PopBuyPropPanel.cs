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
            GameManager.PlayButtonClickSound();
            UIManager.ClosePopPanel(this);
        }
        private void OnCoinBuyClick()
        {
            GameManager.PlayButtonClickSound();
            GameManager.AddCoin(-needCoinNum);
            if (isProp1)
            {
                GameManager.AddPop1Num(1);
                GameManager.IncreaseByProp1NeedCoin();
            }
            else
            {
                GameManager.AddPop2Num(1);
                GameManager.IncreaseByProp2NeedCoin();
            }
            UIManager.ClosePopPanel(this);
        }
        private void OnAdBuyClick()
        {
            GameManager.PlayButtonClickSound();
            Debug.Log("看广告得道具");
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
            needCoinNum = isProp1 ? GameManager.GetProp1NeedCoinNum() : GameManager.GetProp2NeedCoinNum();
            icon.sprite = isProp1 ? prop1icon : prop2icon;
            coinBuyButton.gameObject.SetActive(false);
            adBuyButton.gameObject.SetActive(true);
        }
        protected override void OnEndShow()
        {
            StopCoroutine("DelayShowBuyByCoin");
            StartCoroutine("DelayShowBuyByCoin");
        }
        IEnumerator DelayShowBuyByCoin()
        {
            yield return new WaitForSeconds(1);
            coinBuyButton.gameObject.SetActive(true);
            adBuyButton.gameObject.SetActive(false);
        }
    }
}
