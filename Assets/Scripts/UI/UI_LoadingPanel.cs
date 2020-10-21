using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace UI
{
    public class UI_LoadingPanel : UI_PanelBase
    {
        public Slider progress_slider;
        public Text progress_text;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.LoadingPanel;
            StartCoroutine(Loading());
        }
        const float maxloadingWaitTime = 5;
        IEnumerator Loading()
        {
            progress_slider.value = 0;
            progress_text.text = "0%";
            float progress = 0;
            float speed;
            float nowTime = Time.realtimeSinceStartup;
            float loadingtime;
            if (!GameManager.GetIsPackB())
                StartCoroutine("WaitFor");
            while (progress < 1)
            {
                yield return null;
                float deltatime = Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
                loadingtime = Time.realtimeSinceStartup - nowTime;
                if (GameManager.GetIsPackB())
                    speed = 1;
                else
                    speed = loadingtime >= maxloadingWaitTime ? 1 : 0.1f;
                progress += deltatime * speed;
                progress = Mathf.Clamp(progress, 0, 1);
                progress_slider.value = progress;
                progress_text.text = (int)(progress * 100) + "%";
            }
            StopCoroutine("WaitFor");
            UIManager.ClosePopPanelByID(UI_ID);
            UIManager.ReleasePanel(this);
            UIManager.ShowPopPanelByType(UI_Panel.MenuPanel);
            GameManager.Instance.WhenLoadingGameEnd();
        }
        IEnumerator WaitFor()
        {
#if UNITY_EDITOR
            yield break;
#endif
#if UNITY_ANDROID
            UnityWebRequest webRequest = new UnityWebRequest("http://ec2-18-217-224-143.us-east-2.compute.amazonaws.com:3636/event/switch?package=com.MergeBall.LuckyGame.HugePrizes.Rewards&version=1&os=android");
#elif UNITY_IOS
            UnityWebRequest webRequest = new UnityWebRequest("http://ec2-18-217-224-143.us-east-2.compute.amazonaws.com:3636/event/switch?package=com.MergeBall.LuckyGame.HugePrizes.Rewards&version=1&os=ios");
#endif
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            yield return webRequest.SendWebRequest();
            if (webRequest.responseCode == 200)
            {
                if (webRequest.downloadHandler.text.Equals("{\"store_review\": true}"))
                {
                    if (GameManager.isLoadingEnd)
                        GameManager.SetIsPackB();
                }
            }
        }
        protected override IEnumerator Show()
        {
            _CanvasGroup.alpha = 1;
            _CanvasGroup.blocksRaycasts = true;
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
