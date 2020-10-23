using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_GameOverPanel : UI_PopPanelBase
    {
        public CanvasGroup continueAll;
        public Image timeDown;
        public Text time;
        public Button continueButton;
        public Button nothanksButton;
        [Space(15)]
        public CanvasGroup gameoverAll;
        public Text scoreText;
        public Text bestText;
        public Button restartButton;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.GameOverPanel;
            continueButton.onClick.AddListener(OnContinueClick);
            nothanksButton.onClick.AddListener(OnNothanksClick);
            restartButton.onClick.AddListener(OnRestartButtonClick);
        }
        int clickAdTime = 0;
        private void OnContinueClick()
        {
            GameManager.PlayButtonClickSound();
            StopCoroutine("AutoTimeDown");
            clickAdTime++;
            GameManager.PlayRV(OnContinueAdCallback, clickAdTime, "死亡复活", OnNothanksClick);
        }
        private void OnContinueAdCallback()
        {
            UIManager.ClosePopPanel(this);
            GameManager.ContinueGame();
        }
        private void OnNothanksClick()
        {
            GameManager.PlayButtonClickSound();
            StopCoroutine("AutoTimeDown");
            continueAll.alpha = 0;
            continueAll.blocksRaycasts = false;
            gameoverAll.alpha = 1;
            gameoverAll.blocksRaycasts = true;
            scoreText.text = GameManager.GetScore().ToString();
            bestText.text = GameManager.GetBestScore().ToString();
        }
        private void OnRestartButtonClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ClosePopPanel(this);
            GameManager.SendAdjustGameOverEvent(true);
            GameManager.RestartGame();
        }
        Coroutine nothanksDelay = null;
        protected override void OnStartShow()
        {
            continueAll.alpha = 1;
            continueAll.blocksRaycasts = true;
            gameoverAll.alpha = 0;
            gameoverAll.blocksRaycasts = false;
            clickAdTime = 0;
            nothanksDelay = StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(nothanksButton.gameObject));
        }
        protected override void OnEndClose()
        {
            StopCoroutine(nothanksDelay);
        }
        protected override void OnEndShow()
        {
            StartCoroutine("AutoTimeDown");
        }
        IEnumerator AutoTimeDown()
        {
            timeDown.fillAmount = 1;
            time.text = "5";
            float timer = 5;
            while (timer >= 0)
            {
                timer -= Time.deltaTime;
                timeDown.fillAmount = timer / 5;
                time.text = Mathf.CeilToInt(timer).ToString();
                yield return null;
            }
            continueAll.alpha = 0;
            continueAll.blocksRaycasts = false;
            gameoverAll.alpha = 1;
            gameoverAll.blocksRaycasts = true;
            scoreText.text = GameManager.GetScore().ToString();
            bestText.text = GameManager.GetBestScore().ToString();
        }
    }
}
