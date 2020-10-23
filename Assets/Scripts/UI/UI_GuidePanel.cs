using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UI_GuidePanel : UI_PanelBase
    {
        public Button bg;
        private Animator guideAC;
        private int guideIndex = 1;
        protected override void Awake()
        {
            base.Awake();
            PanelType = UI_Panel.UI_PopPanel.GuidePanel;
            guideAC = GetComponent<Animator>();
            bg.onClick.AddListener(OnNextGuide);
        }
        private void OnNextGuide()
        {
            guideIndex++;
            guideAC.SetInteger("GuideIndex", guideIndex);
        }
        public void OnGuideEnd()
        {
            UIManager.ClosePopPanel(this);
            Destroy(gameObject);
            GameManager.LevelUp(true);
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
        protected override void OnEndClose()
        {
            UIManager.ShowPopPanelByType(UI_Panel.UI_PopPanel.GiftPanel);
        }
    }
}
