﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager
{
    private static GameObject Root;
    private const string LoadAudioFrontPath = "AudioClips/";
    private AudioSource bgmAs;
    public AudioManager(GameObject audioRoot)
    {
        Root = audioRoot;
        PlayBgm();
    }
    private static readonly Dictionary<AudioPlayArea, string> audioClipPathDic = new Dictionary<AudioPlayArea, string>()
    {
        {AudioPlayArea.BGM,"bgm" },
        {AudioPlayArea.Combo1,"combo1" },
        {AudioPlayArea.Combo2,"combo2" },
        {AudioPlayArea.Combo3,"combo3" },
        {AudioPlayArea.Button,"button" },
        {AudioPlayArea.Spin,"spin" },
    };
    private static readonly Dictionary<AudioPlayArea, AudioClip> loadedAudioclipDic = new Dictionary<AudioPlayArea, AudioClip>();
    private static readonly List<AudioSource> allPlayer = new List<AudioSource>();
    public AudioSource PlayOneShot(AudioPlayArea playArea,bool loop = false)
    {
        if(loadedAudioclipDic.TryGetValue(playArea,out AudioClip tempClip))
        {
            if (tempClip == null)
            {
                loadedAudioclipDic.Remove(playArea);
                Debug.LogError("音频路径配置错误，类型:" + playArea);
                return null;
            }
            int asCount = allPlayer.Count;
            AudioSource tempAs;
            for (int i = 0; i < asCount; i++)
            {
                tempAs = allPlayer[i];
                if (!tempAs.isPlaying)
                {
                    tempAs.clip = tempClip;
                    tempAs.loop = loop;
                    tempAs.mute = !GameManager.Instance.GetSoundOn();
                    tempAs.Play();
                    return tempAs;
                }
            }
            tempAs = Root.AddComponent<AudioSource>();
            allPlayer.Add(tempAs);
            tempAs.clip = tempClip;
            tempAs.loop = loop;
            tempAs.mute = !GameManager.Instance.GetSoundOn();
            tempAs.Play();
            return tempAs;
        }
        else
        {
            if(audioClipPathDic.TryGetValue(playArea,out string tempClipFileName))
            {
                tempClip = Resources.Load<AudioClip>(LoadAudioFrontPath + tempClipFileName);
                if (tempClip == null)
                {
                    Debug.LogError("配置的音频文件路径错误，类型:" + playArea);
                    return null;
                }
                int asCount = allPlayer.Count;
                AudioSource tempAs;
                for (int i = 0; i < asCount; i++)
                {
                    tempAs = allPlayer[i];
                    if (!tempAs.isPlaying)
                    {
                        tempAs.clip = tempClip;
                        tempAs.loop = loop;
                        tempAs.mute = !GameManager.Instance.GetSoundOn();
                        tempAs.Play();
                        return tempAs;
                    }
                }
                tempAs = Root.AddComponent<AudioSource>();
                allPlayer.Add(tempAs);
                tempAs.clip = tempClip;
                tempAs.loop = loop;
                tempAs.mute = !GameManager.Instance.GetSoundOn();
                tempAs.Play();
                return tempAs;
            }
            else
            {
                Debug.LogError("没有配置音频文件路径，类型:" + playArea);
                return null;
            }
        }
    }
    public AudioSource PlayLoop(AudioPlayArea playArea)
    {
        return PlayOneShot(playArea, true);
    }
    private void PlayBgm()
    {
        if(audioClipPathDic.TryGetValue(AudioPlayArea.BGM,out string bgmFileName))
        {
            AudioClip tempClip = Resources.Load<AudioClip>(LoadAudioFrontPath + bgmFileName);
            if(tempClip == null)
            {
                Debug.LogError("背景音乐文件路径配置错误");
                return;
            }
            bgmAs = Root.AddComponent<AudioSource>();
            bgmAs.clip = tempClip;
            bgmAs.loop = true;
            bgmAs.mute = !GameManager.Instance.GetMusicOn();
            bgmAs.Play();
        }
        else
        {
            Debug.LogError("背景音乐没有配置文件路径");
        }
    }
    public void SetMusicState(bool isOn)
    {
        bgmAs.mute = !isOn;
    }
    public void SetSoundState(bool isOn)
    {
        int count = allPlayer.Count;
        for(int i = 0; i < count; i++)
        {
            allPlayer[i].mute = !isOn;
        }
    }
}
public enum AudioPlayArea
{
    Button,
    BGM,
    Spin,
    Combo1,
    Combo2,
    Combo3,
}
