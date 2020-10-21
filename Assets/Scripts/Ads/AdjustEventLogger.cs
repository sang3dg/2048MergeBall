﻿using com.adjust.sdk;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_IOS && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

public class AdjustEventLogger : MonoBehaviour
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string Getidfa();
#endif
#if UNITY_IOS
    public const string APP_TOKEN = "sdc4rfoe46ww";
    public const string TOKEN_open = "62yhul";
    public const string TOKEN_ad = "b0h2gy";
    public const string TOKEN_noads = "md89bu";
    public const string TOKEN_stage_end = "srnjyr";
    public const string TOKEN_stage_over = "1kk2dw";
    public const string TOKEN_item_change = "o1x2mh";
    public const string TOKEN_deeplink = "vtvj8n";
    public const string TOKEN_packb = "oauyh2";
    public const string TOKEN_box = "dt8ged";
    public const string TOKEN_wheel = "firn9u";
    public const string TOKEN_slots = "7vfj5z";
#elif UNITY_ANDROID
    public const string APP_TOKEN = "sdc4rfoe46ww";
    public const string TOKEN_open = "62yhul";
    public const string TOKEN_ad = "b0h2gy";
    public const string TOKEN_noads = "md89bu";
    public const string TOKEN_stage_end = "srnjyr";//每10次落球
    public const string TOKEN_stage_over = "1kk2dw";
    public const string TOKEN_item_change = "o1x2mh";//道具变化
    public const string TOKEN_deeplink = "vtvj8n";
    public const string TOKEN_packb = "oauyh2";
    public const string TOKEN_box = "dt8ged";//礼盒球
    public const string TOKEN_wheel = "firn9u";
    public const string TOKEN_slots = "7vfj5z";
#endif
    public static AdjustEventLogger Instance;
    private void Awake()
    {
        Instance = this;
#if UNITY_EDITOR
        AdjustConfig adjustConfig = new AdjustConfig(APP_TOKEN, AdjustEnvironment.Sandbox);
#else
        AdjustConfig adjustConfig = new AdjustConfig(APP_TOKEN, AdjustEnvironment.Production);
#endif
        adjustConfig.sendInBackground = true;
        adjustConfig.launchDeferredDeeplink = true;
        adjustConfig.eventBufferingEnabled = true;
        adjustConfig.logLevel = AdjustLogLevel.Info;
        adjustConfig.setAttributionChangedDelegate(OnAttributionChangedCallback);
        Adjust.start(adjustConfig);
    }
    private void Start()
    {
        GetAdID();
        StartCoroutine(CheckAttributeTo());
        GameManager.Instance.SendAdjustGameStartEvent();
    }
    public void AdjustEventNoParam(string token)
    {
        AdjustEvent adjustEvent = new AdjustEvent(token);
        Adjust.trackEvent(adjustEvent);
    }
    public void AdjustEvent(string token, params (string key, string value)[] list)
    {
        AdjustEvent adjustEvent = new AdjustEvent(token);
        foreach (var (key, value) in list)
        {
            adjustEvent.addCallbackParameter(key, value);
        }
        Adjust.trackEvent(adjustEvent);
    }
    void OnAttributionChangedCallback(AdjustAttribution attribution)
    {
        if (attribution.network.Equals("Organic"))
        {
        }
        else
        {
            if (!GameManager.isLoadingEnd)
                GameManager.Instance.SetIsPackB();
        }
    }
    private string AppName = Ads.AppName;
    private string ifa;
    private IEnumerator CheckAttributeTo()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(ifa));
        string url = string.Format("http://a.mobile-mafia.com:3838/adjust/is_unity_clicked?ifa={0}&app_name={1}", ifa, AppName);

        var web = new UnityWebRequest(url);
        web.downloadHandler = new DownloadHandlerBuffer();
        yield return web.SendWebRequest();
        if (web.responseCode == 200)
        {
            if (web.downloadHandler.text.Equals("1"))
            {
                if (!GameManager.isLoadingEnd)
                    GameManager.Instance.SetIsPackB();
            }
        }
    }
    private void GetAdID()
    {
#if UNITY_ANDROID
        Application.RequestAdvertisingIdentifierAsync(
           (string advertisingId, bool trackingEnabled, string error) =>
           {
               ifa = advertisingId;
           }
       );
#elif UNITY_IOS && !UNITY_EDITOR
        ifa = Getidfa();
#endif
    }
}

    