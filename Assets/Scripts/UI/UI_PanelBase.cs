using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UI_PanelBase : MonoBehaviour
    {
        public static UIManager UIManager;
        public static GameManager GameManager;
        private static int UI_ID_Index = 0;
        private bool hasNewID = false;
        protected CanvasGroup _CanvasGroup;
        public UI_State State { get; private set; }
        public int PanelType { get; protected set; }
        public int UI_ID
        {
            get;
            private set;
        }
        protected virtual void Awake()
        {
            UI_ID = UI_ID_Index++;
            _CanvasGroup = GetComponent<CanvasGroup>();
            State = UI_State.Unknow;
            hasNewID = true;
        }
        public IEnumerator ShowThisPanel()
        {
            if (!hasNewID)
            {
                Debug.LogError("面板ID错误：未初始化面板ID.");
                yield break;
            }
            OnStartShow();
            yield return Show();
            State = UI_State.Show;
            OnEndShow();
        }
        protected virtual IEnumerator Show()
        {
            throw new System.Exception("未设置开启动画!");
        }
        protected virtual void OnStartShow()
        {

        }
        protected virtual void OnEndShow()
        {

        }
        public void PauseThisPanel()
        {
            OnPause();
        }
        protected virtual void OnPause()
        {
            State = UI_State.Pause;
        }
        public void ResumeThisPanel()
        {
            OnResume();
        }
        protected virtual void OnResume()
        {
            State = UI_State.Show;
        }
        public IEnumerator CloseThisPanel()
        {
            if (!hasNewID)
            {
                Debug.LogError("面板ID错误：未初始化面板ID.");
                yield break;
            }
            OnStartClose();
            yield return Close();
            State = UI_State.Close;
            OnEndClose();
        }
        protected virtual void OnStartClose()
        {

        }
        protected virtual IEnumerator Close()
        {
            throw new System.Exception("未设置关闭动画!");
        }
        protected virtual void OnEndClose()
        {

        }
    }
    public enum UI_State
    {
        Show,
        Pause,
        Close,
        Unknow,
    }
}
