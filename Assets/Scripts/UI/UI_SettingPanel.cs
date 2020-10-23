using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_SettingPanel : UI_PopPanelBase
    {
        public UI_Switch soundSwitch;
        public UI_Switch musicSwitch;
        public Button closeButton;
        public Button restartButton;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.SettingPanel;
            soundSwitch.OnValueChanged.AddListener(OnSoundSwitch);
            musicSwitch.OnValueChanged.AddListener(OnMusicSwitch);
            closeButton.onClick.AddListener(OnCloseClick);
            restartButton.onClick.AddListener(OnRestartClick);
        }
        private void OnSoundSwitch(bool isOn)
        {
            GameManager.PlayButtonClickSound();
            GameManager.SetSaveSoundState(isOn);
        }
        private void OnMusicSwitch(bool isOn)
        {
            GameManager.PlayButtonClickSound();
            GameManager.SetSaveMusicState(isOn);
        }
        private void OnCloseClick()
        {
            GameManager.PlayButtonClickSound();
            UIManager.ClosePopPanel(this);
        }
        private void OnRestartClick()
        {
            GameManager.PlayButtonClickSound();
            if (AnimationAutoEnd.IsAnimation) return;
            GameManager.SendAdjustGameOverEvent(false);
            GameManager.RestartGame();
            UIManager.ClosePopPanel(this);
        }
        Coroutine closeDelay = null;
        protected override void OnStartShow()
        {
            soundSwitch.IsOn = GameManager.GetSoundOn();
            musicSwitch.IsOn = GameManager.GetMusicOn();
            closeDelay= StartCoroutine(ToolManager.DelaySecondShowNothanksOrClose(closeButton.gameObject));
        }
        protected override void OnEndClose()
        {
            StopCoroutine(closeDelay);
        }
    }
}
