﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Ads : MonoBehaviour
{
    //ios FBID 229348018437156  Golden Luck
#if UNITY_ANDROID
    private const string APP_KEY = "dbcff7a9";
#elif UNITY_IOS
	private const string APP_KEY = "";
#endif
	public static Ads _instance;
	[NonSerialized]
	public string adDes = string.Empty;
	public const string AppName = "A040_MergeBall";
	private void Awake()
	{
		_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		//Dynamic config example
		IronSourceConfig.Instance.setClientSideCallbacks(true);

		string id = IronSource.Agent.getAdvertiserId();

		//IronSource.Agent.validateIntegration();

		// SDK init
		IronSource.Agent.init(APP_KEY);
		IronSource.Agent.loadInterstitial();

	}
	public bool ShowRewardVideo(Action rewardedCallback, int clickAdTime,string des,Action failCallback)
	{
		adDes = des;
		rewardCallback = rewardedCallback;
		rewardFailCallback = failCallback;
#if UNITY_EDITOR
		rewardedCallback();
		Debug.Log("RV:【" + des + "】");
		return true;
#endif
#if UNITY_IOS
		if (!GameManager.Instance.GetIsPackB())
		{
			rewardCallback();
			return true;
		}
#endif
		if (IronSource.Agent.isRewardedVideoAvailable())
		{
			IronSource.Agent.showRewardedVideo();
			return true;
		}
		else
		{
			StartCoroutine(WaitLoadAD(true,clickAdTime));
			return false;
		}
	}
	float interstialLasttime = 0;
	public void ShowInterstialAd(Action callback, string des)
	{
		popCallback = callback;
#if UNITY_EDITOR
		callback?.Invoke();
		Debug.Log("IV:【" + des + "】");
		return;
#endif
		adDes = des;
#if UNITY_IOS
		if (!GameManager.Instance.GetIsPackB()) 
		{
			callback();
			return;
		}
#endif
		if (Time.realtimeSinceStartup - interstialLasttime < 30)
        {
			callback();
			return;
        }
		if (IronSource.Agent.isInterstitialReady())
		{
			interstialLasttime = Time.realtimeSinceStartup;
			IronSource.Agent.showInterstitial();
		}
		else
		{
			callback();
			GameManager.Instance.SendAdjustPlayAdEvent(false, false, adDes);
		}
	}
	void OnApplicationPause(bool isPaused)
	{
		IronSource.Agent.onApplicationPause(isPaused);
	}
	public GameObject adLoadingTip;
	const string text = "No Video is ready , please try again later.";
	IEnumerator WaitLoadAD(bool isRewardedAd,int clickAdTime)
	{
		adLoadingTip.SetActive(true);
		StringBuilder content = new StringBuilder("Loading.");
		Text noticeText = adLoadingTip.GetComponentInChildren<Text>();
		noticeText.text = content.ToString();
		int timeOut = 6;
		while (timeOut > 0)
		{
			yield return new WaitForSeconds(Time.timeScale);
			timeOut--;
			content.Append('.');
			noticeText.text = content.ToString();
			if (isRewardedAd && IronSource.Agent.isRewardedVideoAvailable())
			{
				IronSource.Agent.showRewardedVideo();
				adLoadingTip.SetActive(false);
				yield break;
			}
		}
		adLoadingTip.SetActive(false);
		GameManager.Instance.SendAdjustPlayAdEvent(false, true, adDes);
		if (clickAdTime >= 2)
		{
			rewardFailCallback?.Invoke();
			TipsManager.Instance.ShowTips(text, 2);
		}
	}
	Action rewardCallback;
	Action rewardFailCallback;
	private bool canGetReward = false;
	public void GetReward()
	{
		canGetReward = true;
	}
	public void InvokeGetRewardMethod()
	{
		if (canGetReward)
		{
			rewardCallback();
			canGetReward = false;
		}
	}
	Action popCallback;
	public void InvokePopAd()
    {
		popCallback();
    }
}
       