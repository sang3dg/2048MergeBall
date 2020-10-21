﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlyReward : MonoBehaviour
{
    public static FlyReward Instance;
    CanvasGroup canvasGroup;
    private void Awake()
    {
        Instance = this;
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        list_allFly.Add(go_FlyOne.transform);
        list_allFlyImage.Add(go_FlyOne.GetComponent<Image>());
        f_OffsetX_EachPart = Screen.width * 0.1f;

    }
    Vector3 StartPos = Vector3.zero;
    Vector3 TargetPos = Vector3.zero;
    Reward flyType;
    int flyNum = 0;
    public void FlyToTarget(Vector3 startWorldPos, Vector3 targetWorldPos, int num, Reward flyType, Action<Reward> callback)
    {
        this.flyType = flyType;
        StartPos = Camera.main.WorldToScreenPoint(startWorldPos);
        TargetPos = Camera.main.WorldToScreenPoint(targetWorldPos);
        flyNum = num;

        RandomSpawnPos();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        if (cor_fly != null)
            StopCoroutine(cor_fly);
        cor_fly = StartCoroutine(StartMove(flyType, callback));
    }
    Coroutine cor_fly = null;
    IEnumerator StartMove(Reward _flyTarget, Action<Reward> callback)
    {
        float spreadTime = 0;
        float height = 50;
        while (spreadTime <= 0.2f)
        {
            yield return null;
            spreadTime += Time.deltaTime;
            float progress = spreadTime * 5;
            for (int i = 0; i < flyNum; i++)
            {
                list_allFly[i].position = Vector3.Lerp(StartPos, list_allEndPos[i], progress);
                list_allFly[i].position += new Vector3(0, -(spreadTime - 0.1f) * (spreadTime - 0.1f) * height * 100 + height, 0);
            }
        }
        yield return null;
        for (int i = 0; i < flyNum; i++)
        {
            list_allFly[i].position = list_allEndPos[i];
        }
        yield return null;
        while (spreadTime <= 0.3f)
        {
            yield return null;
            spreadTime += Time.deltaTime;
            for (int i = 0; i < flyNum; i++)
            {
                list_allFly[i].position = list_allEndPos[i] + new Vector3(0, -(spreadTime - 0.25f) * (spreadTime - 0.25f) * 8000 + 20, 0);
            }
        }
        yield return new WaitForSeconds(0.5f * Time.timeScale);
        float flyTime = 0;
        float delay = 0.3f;
        int startIndex = 0;
        while (startIndex <= flyNum - 1)
        {
            yield return null;
            flyTime += Time.deltaTime * 5;
            float progress;
            for (int i = startIndex; i < flyNum; i++)
            {
                if (flyTime - i * delay >= 1)
                {
                    callback(_flyTarget);
                    startIndex = i + 1;
                    list_allFlyImage[i].color = Color.clear;
                    progress = 1;
                    GameManager.Instance.PlayFlyOverSound();
                }
                else
                    progress = flyTime;
                float thisProgress = progress - i * delay;
                list_allFly[i].position = Vector3.Lerp(list_allEndPos[i], TargetPos, thisProgress);
            }
        }
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    public GameObject go_FlyOne;
    readonly List<Transform> list_allFly = new List<Transform>();
    readonly List<Image> list_allFlyImage = new List<Image>();
    readonly List<Vector3> list_allOffset = new List<Vector3>();
    readonly List<Vector3> list_allEndPos = new List<Vector3>();
    float f_OffsetX_EachPart;
    const int maxFlyNum = 8;
    void RandomSpawnPos()
    {
        list_allEndPos.Clear();
        list_allOffset.Clear();
        flyNum = Mathf.Clamp(flyNum, 0, maxFlyNum);
        int nowGoCount = list_allFly.Count;
        if (flyNum > nowGoCount)
        {
            int needCloneCount = flyNum - nowGoCount;
            for (int i = 0; i < needCloneCount; i++)
            {
                Transform tempFlyTrans = Instantiate(go_FlyOne, go_FlyOne.transform.parent).transform;
                list_allFly.Add(tempFlyTrans);
                list_allFlyImage.Add(tempFlyTrans.GetComponent<Image>());
            }
        }
        List<float> unRandomOffsetX = new List<float>();
        float f_startR = 0, f_startL = 0;
        int surplusNum = flyNum;
        if (flyNum % 2 == 0)
        {
            f_startR = f_OffsetX_EachPart * 0.5f;
            f_startL = -f_startR;
            unRandomOffsetX.Add(f_startR);
            unRandomOffsetX.Add(f_startL);
            surplusNum -= 2;
        }
        else
        {
            unRandomOffsetX.Add(0);
            surplusNum--;
        }
        int distance2Center = 0;
        while (surplusNum > 0)
        {
            distance2Center++;
            unRandomOffsetX.Add(f_startR + distance2Center * f_OffsetX_EachPart);
            unRandomOffsetX.Add(f_startL - distance2Center * f_OffsetX_EachPart);
            surplusNum -= 2;
        }
        Sprite targetSprite;
        string sp_name = flyType.ToString();
        targetSprite = SpriteManager.Instance.GetSprite(SpriteAtlas_Name.Fly, sp_name);

        for (int i = 0; i < flyNum; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, unRandomOffsetX.Count);
            list_allEndPos.Add(StartPos + new Vector3(unRandomOffsetX[randomIndex], 0, 0));
            unRandomOffsetX.RemoveAt(randomIndex);
            list_allFlyImage[i].color = Color.white;
            list_allFlyImage[i].sprite = targetSprite;
        }
        int totalCount = list_allFlyImage.Count;
        for (int i = flyNum; i < totalCount; i++)
        {
            list_allFlyImage[i].color = Color.clear;
        }
    }
}
