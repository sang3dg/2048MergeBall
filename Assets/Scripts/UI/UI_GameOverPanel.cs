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
        private void OnContinueClick()
        {
            StopCoroutine("AutoTimeDown");
            UIManager.ClosePopPanel(this);
            GameManager.ContinueGame();
        }
        private void OnNothanksClick()
        {
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
            UIManager.ClosePopPanel(this);
            GameManager.RestartGame();
        }
        protected override void OnStartShow()
        {
            continueAll.alpha = 1;
            continueAll.blocksRaycasts = true;
            gameoverAll.alpha = 0;
            gameoverAll.blocksRaycasts = false;
            nothanksButton.gameObject.SetActive(false);
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
            float nothanksTime = 4;
            while (timer >= 0)
            {
                timer -= Time.deltaTime;
                if (timer <= nothanksTime)
                    if (!nothanksButton.gameObject.activeSelf)
                        nothanksButton.gameObject.SetActive(true);
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
