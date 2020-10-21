using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_RateusPanel : UI_PopPanelBase
    {
        public Button noButton;
        public Button yesButton;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.RateusPanel;
            noButton.onClick.AddListener(OnNoClick);
            yesButton.onClick.AddListener(OnYesClick);
        }
        private void OnNoClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ClosePopPanel(this);
        }
        private void OnYesClick()
        {
            GameManager.PlayButtonClickSound();
#if UNITY_ANDROID
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.MergeBall.LuckyGame.HugePrizes.Rewards");
#elif UNITY_IOS
        var url = string.Format(
           "itms-apps://itunes.apple.com/cn/app/id{0}?mt=8&action=write-review",
           "");
        Application.OpenURL(url);
#endif
            UIManager.ClosePopPanel(this);
        }
    }
}
