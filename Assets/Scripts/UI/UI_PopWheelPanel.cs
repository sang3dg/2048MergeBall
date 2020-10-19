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
        private void OnSpinClick()
        {
            if (isSpining) return;
            if (hasFree)
            {
                Debug.Log("免费转动转盘");
            }
            else
            {
                if (GameManager.GetWheelTicket() <= 0) return;
                Debug.Log("观看广告转动转盘");
            }
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
            while (rotateTimer <= rotateTime)
            {
                float deltaTime = Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
                wheelRect.Rotate(new Vector3(0, 0, -deltaTime * roatateSpeed));
                rotateTimer += deltaTime;
                yield return null;
            }
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
            if (isSpining) return;
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
        private void RefreshTicketShowText()
        {
            ticketNumText.text = "x" + GameManager.GetWheelTicket();
        }
        protected override void OnStartShow()
        {
            CheckHasFree();
            wheelRect.rotation = Quaternion.identity;
            RefreshTicketShowText();
        }
    }
}
