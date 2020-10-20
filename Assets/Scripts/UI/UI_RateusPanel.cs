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
            UIManager.ClosePopPanel(this);
        }
    }
}
