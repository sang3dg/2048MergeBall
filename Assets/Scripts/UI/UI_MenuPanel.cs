using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_MenuPanel : UI_PanelBase
    {
        public Button settingButton;
        public Button cashButton;
        public Button coinButton;
        public Button wheelButton;
        public Button propButton1;
        public Button propButton2;
        public Text cashText;
        public Text coinText;
        public Text scoreText;
        public Text bestScoreText;
        public Text currentStageText;
        public Text nextStageText;
        public Image stageProgressFillImage;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.MenuPanel;
            settingButton.onClick.AddListener(OnSettingButtonClick);
            cashButton.onClick.AddListener(OnCashButtonClick);
            coinButton.onClick.AddListener(OnCoinButtonClick);
            wheelButton.onClick.AddListener(OnWheelButtonClick);
            propButton1.onClick.AddListener(OnProp1ButtonClick);
            propButton2.onClick.AddListener(OnProp2ButtonClick);
        }
        private void OnSettingButtonClick()
        {
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.SettingPanel);
        }
        private void OnCashButtonClick()
        {

        }
        private void OnCoinButtonClick()
        {

        }
        private void OnWheelButtonClick()
        {
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.WheelPanel);
        }
        private void OnProp1ButtonClick()
        {

        }
        private void OnProp2ButtonClick()
        {

        }
        public void RefreshCashText()
        {
            cashText.text = ToolManager.GetCashShowString(GameManager.GetCash());
        }
        public void RefreshCoinText()
        {
            coinText.text = GameManager.GetCoin().ToString();
        }
        public void RefreshScoreText()
        {
            scoreText.text = GameManager.GetScore().ToString();
            RefreshStageProgress();
        }
        public void RefreshBestScoreText()
        {
            bestScoreText.text = GameManager.GetBestScore().ToString();
        }
        private void SetStageInfo()
        {
            currentStageText.text = GameManager.GetStage().ToString();
            nextStageText.text = (GameManager.GetStage() + 1).ToString();
        }
        const float FillStart = 0.326f;
        const float FillEnd = 0.677f;
        const float FillLength = FillEnd - FillStart;
        public void RefreshStageProgress()
        {
            targetProgress = FillStart + FillLength * (1f * GameManager.GetScore() / GameManager.UpgradeNeedScore);
        }
        const float ProgressAnimationSpeed = 0.1f;
        private float targetProgress = 0;
        private float currentProgress = 0;
        private IEnumerator StageProgressAnimation()
        {
            while (true)
            {
                if (Mathf.Abs(targetProgress - currentProgress) > 0.04f)
                {
                    float delta = Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f) * ProgressAnimationSpeed;
                    currentProgress += delta;
                    if (currentProgress >= FillEnd)
                    {
                        currentProgress = FillStart;
                        GameManager.nextSlotsIsUpgradeSlots = true;
                        UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.SlotsPanel);
                        SetStageInfo();
                    }
                    stageProgressFillImage.fillAmount = currentProgress;
                }
                yield return null;
            }
        }
        protected override IEnumerator Show()
        {
            _CanvasGroup.alpha = 1;
            _CanvasGroup.blocksRaycasts = true;
            RefreshCashText();
            RefreshCoinText();
            RefreshScoreText();
            RefreshBestScoreText();
            SetStageInfo();
            StartCoroutine(StageProgressAnimation());
            yield return null;
        }
        protected override IEnumerator Close()
        {
            _CanvasGroup.alpha = 0;
            _CanvasGroup.blocksRaycasts = false;
            yield return null;
        }
    }
}
