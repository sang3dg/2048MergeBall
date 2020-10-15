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
            Debug.Log("Sound " + (isOn ? "On" : "Off"));
        }
        private void OnMusicSwitch(bool isOn)
        {
            Debug.Log("Music " + (isOn ? "On" : "Off"));
        }
        private void OnCloseClick()
        {
            UIManager.ClosePopPanel(this);
        }
        private void OnRestartClick()
        {

        }
    }
}
