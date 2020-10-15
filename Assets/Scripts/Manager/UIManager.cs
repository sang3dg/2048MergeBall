﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        const string UI_Prefab_Path_Pre = "Prefabs/UI/";
        static readonly Dictionary<int, string> UI_Type_Path_Dic = new Dictionary<int, string>()
        {
            { UI_Panel.UI_PopPanel.LoadingPanel , "Pop Loading Panel" },
            { UI_Panel.UI_PopPanel.SettingPanel , "Pop Setting Panel" },
            { UI_Panel.UI_PopPanel.SlotsPanel , "Pop Slots Panel" },
            { UI_Panel.UI_PopPanel.RewardNoCashPanel , "Pop RewardNoCash Panel" },
            { UI_Panel.UI_PopPanel.RewardCashPanel , "Pop RewardCash Panel" },
            { UI_Panel.UI_PopPanel.WheelPanel , "Pop Wheel Panel" },
            { UI_Panel.UI_PopPanel.GiftPanel , "Pop Gift Panel" },
            {UI_Panel.MenuPanel,"Menu Panel" },
        };
        static readonly Dictionary<int, GameObject> UI_Type_Prefab_Dic = new Dictionary<int, GameObject>();
        static readonly Dictionary<int, List<UI_PanelBase>> UI_Type_Loaded_Dic = new Dictionary<int, List<UI_PanelBase>>();
        static readonly Dictionary<int, UI_PanelBase> UI_ID_Loaded_Dic = new Dictionary<int, UI_PanelBase>();
        static readonly Stack<UI_PanelBase> UI_PopPanel_Stack = new Stack<UI_PanelBase>();
        static GameManager GameManager;
        private RectTransform popRoot;
        public void Init(RectTransform popRoot,GameManager gameManager)
        {
            this.popRoot = popRoot;
            GameManager = gameManager;
            UI_PanelBase.UIManager = this;
            UI_PanelBase.GameManager = gameManager;
        }
        struct UI_Commond
        {
            public UI_Commond_Type operation_type;
            public int operation_id;
            public int operation_panelType;
        }
        enum UI_Commond_Type
        {
            Show,
            Close
        }
        static readonly Queue<UI_Commond> commonds_queue = new Queue<UI_Commond>();
        Coroutine taskCoroutine = null;
        IEnumerator ExcuteTask()
        {
            if (commonds_queue.Count == 0)
                yield break;
            while (commonds_queue.Count > 0)
            {
                UI_Commond commond = commonds_queue.Dequeue();
                if (commond.operation_type == UI_Commond_Type.Show)
                {
                    if (commond.operation_id != -1)
                        Debug.LogError("不能根据id显示界面!");
                    int panelType = commond.operation_panelType;
                    bool hasShow = false;
                    List<UI_PanelBase> loaded = null;
                    if (UI_Type_Loaded_Dic.ContainsKey(panelType))
                    {
                        loaded = UI_Type_Loaded_Dic[panelType];
                        if (loaded != null && loaded.Count > 0)
                        {
                            int loadedCount = loaded.Count;
                            for(int i = 0; i < loadedCount; i++)
                            {
                                if (loaded[i].State == UI_State.Close)
                                {
                                    loaded[i].transform.SetAsLastSibling();
                                    yield return loaded[i].ShowThisPanel();
                                    hasShow = true;
                                    while (UI_PopPanel_Stack.Count > 0)
                                    {
                                        UI_PanelBase top = UI_PopPanel_Stack.Peek();
                                        switch (top.State)
                                        {
                                            case UI_State.Show:
                                                top.PauseThisPanel();
                                                break;
                                            case UI_State.Pause:
                                                Debug.LogError("未知暂停状态的面板!");
                                                break;
                                            case UI_State.Close:
                                                UI_PopPanel_Stack.Pop();
                                                continue;
                                            case UI_State.Unknow:
                                                break;
                                        }
                                        break;
                                    }
                                    UI_PopPanel_Stack.Push(loaded[i]);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            UI_Type_Loaded_Dic.Remove(panelType);
                            loaded = new List<UI_PanelBase>();
                            UI_Type_Loaded_Dic.Add(panelType, loaded);
                        }
                    }
                    else
                    {
                        loaded = new List<UI_PanelBase>();
                        UI_Type_Loaded_Dic.Add(panelType, loaded);
                    }
                    if (!hasShow)
                    {
                        if (UI_Type_Prefab_Dic.ContainsKey(panelType))
                        {
                            if (UI_Type_Prefab_Dic[panelType] is null)
                            {
                                Debug.LogError("加载的预制体为空!");
                                continue;
                            }
                            UI_PanelBase panel = Instantiate(UI_Type_Prefab_Dic[panelType], popRoot).GetComponent<UI_PanelBase>();
                            panel.transform.SetAsLastSibling();
                            yield return panel.ShowThisPanel();
                            UI_Type_Loaded_Dic[panelType].Add(panel);
                            UI_ID_Loaded_Dic.Add(panel.UI_ID, panel);
                            hasShow = true;
                            while (UI_PopPanel_Stack.Count > 0)
                            {
                                UI_PanelBase top = UI_PopPanel_Stack.Peek();
                                switch (top.State)
                                {
                                    case UI_State.Show:
                                        top.PauseThisPanel();
                                        break;
                                    case UI_State.Pause:
                                        Debug.LogError("未知暂停状态的面板!");
                                        break;
                                    case UI_State.Close:
                                        UI_PopPanel_Stack.Pop();
                                        continue;
                                    case UI_State.Unknow:
                                        break;
                                }
                                break;
                            }
                            UI_PopPanel_Stack.Push(panel);
                        }
                        else
                        {
                            if (UI_Type_Path_Dic.ContainsKey(panelType))
                            {
                                GameObject prefab = Resources.Load<GameObject>(UI_Prefab_Path_Pre + UI_Type_Path_Dic[panelType]);
                                if (prefab is null)
                                {
                                    Debug.LogError("预制体路径错误!");
                                    continue;
                                }
                                else
                                {
                                    UI_Type_Prefab_Dic.Add(panelType, prefab);
                                    UI_PanelBase panel = Instantiate(UI_Type_Prefab_Dic[panelType], popRoot).GetComponent<UI_PanelBase>();
                                    panel.transform.SetAsLastSibling();
                                    yield return panel.ShowThisPanel();
                                    UI_Type_Loaded_Dic[panelType].Add(panel);
                                    UI_ID_Loaded_Dic.Add(panel.UI_ID, panel);
                                    hasShow = true;
                                    while (UI_PopPanel_Stack.Count > 0)
                                    {
                                        UI_PanelBase top = UI_PopPanel_Stack.Peek();
                                        switch (top.State)
                                        {
                                            case UI_State.Show:
                                                top.PauseThisPanel();
                                                break;
                                            case UI_State.Pause:
                                                Debug.LogError("未知暂停状态的面板!");
                                                break;
                                            case UI_State.Close:
                                                UI_PopPanel_Stack.Pop();
                                                continue;
                                            case UI_State.Unknow:
                                                break;
                                        }
                                        break;
                                    }
                                    UI_PopPanel_Stack.Push(panel);
                                }
                            }
                            else
                            {
                                Debug.LogError("没有配置预制体路径!");
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    if (UI_ID_Loaded_Dic.ContainsKey(commond.operation_id))
                    {
                        UI_State oldState = UI_ID_Loaded_Dic[commond.operation_id].State;
                        if (oldState != UI_State.Close)
                        {
                            yield return UI_ID_Loaded_Dic[commond.operation_id].CloseThisPanel();
                            if (oldState == UI_State.Show)
                            {
                                while (UI_PopPanel_Stack.Count>0&& UI_PopPanel_Stack.Peek().State==UI_State.Close)
                                {
                                    UI_PopPanel_Stack.Pop();
                                }
                                if (UI_PopPanel_Stack.Count > 0 && UI_PopPanel_Stack.Peek().State == UI_State.Pause)
                                {
                                    UI_PopPanel_Stack.Peek().ResumeThisPanel();
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("不存在该id的面板!");
                    }
                }
            }
            taskCoroutine = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="panelType">class UI_Panel</param>
        public void ShowPopPanelByType(int panelType)
        {
            UI_Commond newCommond = new UI_Commond()
            {
                operation_type = UI_Commond_Type.Show,
                operation_panelType = panelType,
                operation_id = -1
            };
            commonds_queue.Enqueue(newCommond);
            if (taskCoroutine is null)
                taskCoroutine = StartCoroutine(ExcuteTask());
        }
        public void ClosePopPanelByID(int panelID)
        {
            UI_Commond newCommond = new UI_Commond()
            {
                operation_type = UI_Commond_Type.Close,
                operation_id = panelID,
                operation_panelType = -1
            };
            commonds_queue.Enqueue(newCommond);
            if (taskCoroutine is null)
                taskCoroutine = StartCoroutine(ExcuteTask());
        }
        public void ClosePopPanel(UI_PanelBase _panel)
        {
            int panelID = _panel.UI_ID;
            UI_Commond newCommond = new UI_Commond()
            {
                operation_type = UI_Commond_Type.Close,
                operation_id = panelID,
                operation_panelType = -1
            };
            commonds_queue.Enqueue(newCommond);
            if (taskCoroutine is null)
                taskCoroutine = StartCoroutine(ExcuteTask());
        }
        public void ReleasePanel(UI_PanelBase panel)
        {
            int panelID = panel.UI_ID;
            int panelType = panel.PanelType;
            if (UI_ID_Loaded_Dic.ContainsKey(panelID))
                UI_ID_Loaded_Dic.Remove(panelID);
            if (UI_Type_Loaded_Dic.ContainsKey(panelType))
            {
                if (UI_Type_Loaded_Dic[panelType].Count == 1 && UI_Type_Loaded_Dic[panelType][0].UI_ID == panelID)
                    UI_Type_Loaded_Dic.Remove(panelType);
            }
            if (UI_Type_Prefab_Dic.ContainsKey(panelType))
                UI_Type_Prefab_Dic.Remove(panelType);
            DestroyImmediate(panel.gameObject);
            Resources.UnloadUnusedAssets();
        }
        public UI_PanelBase GetUIPanel(int panelType)
        {
            if (UI_Type_Loaded_Dic.ContainsKey(panelType))
            {
                List<UI_PanelBase> panels = UI_Type_Loaded_Dic[panelType];
                int count = panels.Count;
                for (int i = 0; i < count; i++)
                {
                    if (panels[i].State != UI_State.Close)
                        return panels[i];
                }
                Debug.LogWarning("没有显示该面板!");
            }
            else
                Debug.LogError("没有加载该面板!");
            return null;
        }
    }
    public struct UI_Panel
    {
        public const int MenuPanel = UI_PopPanel.PopPanelNum + 1;
        public struct UI_PopPanel
        {
            public const int LoadingPanel = 1;
            public const int SettingPanel = 2;
            public const int SlotsPanel = 3;
            public const int RewardNoCashPanel = 4;
            public const int RewardCashPanel = 5;
            public const int WheelPanel = 6;
            public const int GiftPanel = 7;
            public const int PopPanelNum = 7;
        }
    }
}