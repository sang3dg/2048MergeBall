using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_PopWheelPanel : UI_PopPanelBase
    {
        public Button spinButton;
        public Button closeButton;
        public RectTransform wheelRect;
        public Text ticketNumText;
        public GameObject adicon;
        public RectTransform spinRect;
        static readonly List<Reward> rewardTypes = new List<Reward>()
        {
            Reward.Cash,
            Reward.WheelTicket,
            Reward.Coin,
            Reward.Prop2,
            Reward.Cash,
            Reward.Prop1,
            Reward.Coin,
            Reward.Cash,
            Reward.Prop1,
            Reward.Cash,
            Reward.Coin,
            Reward.Prop2
        };
        static readonly List<int> rewardNums = new List<int>()
        {
            1000,
            2,
            50,
            2,
            100,
            2,
            100,
            100000,
            1,
            10,
            200,
            1
        };
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.WheelPanel;
            spinButton.onClick.AddListener(OnSpinClick);
            closeButton.onClick.AddListener(OnCloseClick);
        }
        int endIndex = -1;
        bool isSpining = false;
        int clickAdTime = 0;
        private void OnSpinClick()
        {
            GameManager.PlayButtonClickSound();
            if (isSpining) return;
            if (hasFree)
            {
                OnAdSpinCallback();
            }
            else
            {
                if (GameManager.GetWheelTicket() <= 0)
                {
                    TipsManager.Instance.ShowTips("not enough ticet!");
                    return;
                }
                clickAdTime++;
                GameManager.PlayRV(OnAdSpinCallback, clickAdTime, "转动转盘");
            }
        }
        private void OnAdSpinCallback()
        {
            GameManager.AddSpinWheelTime();
            GameManager.SendAdjustSpinWheelEvent();
            isSpining = true;
            endIndex = GameManager.RandomWheelReward();
            StartCoroutine("StartSpinWheel");
        }
        const float roatateSpeed = 1000;
        const float rotateTime = 1;
        IEnumerator StartSpinWheel()
        {
            float endEularAngle = 360 - endIndex * 30;
            if (endEularAngle == 360)
                endEularAngle = 0;
            float rotateTimer = 0;
            AudioSource tempAs = GameManager.PlaySpinSound();
            while (rotateTimer <= rotateTime)
            {
                float deltaTime = Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
                wheelRect.Rotate(new Vector3(0, 0, -deltaTime * roatateSpeed));
                rotateTimer += deltaTime;
                yield return null;
            }
            tempAs.Stop();
            wheelRect.localEulerAngles = new Vector3(0, 0, (endEularAngle - 5+360) % 360);
            for (int i = 0; i < 7; i++)
            {
                yield return null;
            }
            wheelRect.localEulerAngles = new Vector3(0, 0, endEularAngle);
            for (int i = 0; i < 60; i++)
            {
                yield return null;
            }
            GameManager.ShowConfirmRewardPanel(rewardTypes[endIndex], rewardNums[endIndex]);
            if (hasFree)
                GameManager.UseFreeWheel();
            CheckHasFree();
            GameManager.AddWheelTicket(-1);
            RefreshTicketShowText();
            isSpining = false;
        }
        private void OnCloseClick()
        {
            GameManager.PlayButtonClickSound();
            if (isSpining) return;
            GameManager.PlayIV("关闭转盘");
            UIManager.ClosePopPanel(this);
        }
        static Vector2 noadSpinPos = new Vector2(0, 3.7f);
        static Vector2 adSpinPos = new Vector2(43.8f, 3.7f);
        bool hasFree = false;
        private void CheckHasFree()
        {
            hasFree = GameManager.GetTodayHasFreeWheel();
            if (hasFree)
            {
                adicon.SetActive(false);
                spinRect.localPosition = noadSpinPos;
            }
            else
            {
                adicon.SetActive(true);
                spinRect.localPosition = adSpinPos;
            }
        }
        public void RefreshTicketShowText()
        {
            ticketNumText.text = "x" + GameManager.GetWheelTicket();
        }
        protected override void OnStartShow()
        {
            clickAdTime = 0;
            CheckHasFree();
            wheelRect.rotation = Quaternion.identity;
            RefreshTicketShowText();
        }
        protected override void OnEndShow()
        {
            var menu = UIManager.GetUIPanel(UI_Panel.MenuPanel) as UI_MenuPanel;
            menu.rewardTargetTransform.Add(Reward.WheelTicket,ticketNumText.transform.parent);
        }
    }
}
