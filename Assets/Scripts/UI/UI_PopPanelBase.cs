using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UI_PopPanelBase : UI_PanelBase
    {
        protected override void Awake()
        {
            base.Awake();
            _CanvasGroup.interactable = false;
        }
        protected override IEnumerator Show()
        {
            _CanvasGroup.blocksRaycasts = true;
            Transform content = transform.GetChild(1);
            AnimationCurve scaleCurve = GameManager.PopPanelScaleAnimation;
            AnimationCurve alphaCurve = GameManager.PopPanelAlphaAnimation;
            float scaleEndTime = scaleCurve[scaleCurve.length - 1].time;
            float alphaEndTime = alphaCurve[alphaCurve.length - 1].time;
            float maxTime = Mathf.Max(scaleEndTime, alphaEndTime);
            content.localScale = Vector3.one * scaleCurve[0].value;
            _CanvasGroup.alpha = alphaCurve[0].value;
            float progress = 0;
            while (progress < maxTime)
            {
                progress += Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
                progress = Mathf.Clamp(progress, 0, maxTime);
                content.localScale = Vector3.one * scaleCurve.Evaluate(progress > scaleEndTime ? scaleEndTime : progress);
                _CanvasGroup.alpha = alphaCurve.Evaluate(progress > alphaEndTime ? alphaEndTime : progress);
                yield return null;
            }
            _CanvasGroup.interactable = true;
        }
        protected override IEnumerator Close()
        {
            _CanvasGroup.interactable = false;
            Transform content = transform.GetChild(1);
            AnimationCurve scaleCurve = GameManager.PopPanelScaleAnimation;
            AnimationCurve alphaCurve = GameManager.PopPanelAlphaAnimation;
            float scaleEndTime = scaleCurve[scaleCurve.length - 1].time;
            float alphaEndTime = alphaCurve[alphaCurve.length - 1].time;
            float maxTime = Mathf.Max(scaleEndTime, alphaEndTime);
            content.localScale = Vector3.one * scaleCurve[0].value;
            _CanvasGroup.alpha = alphaCurve[0].value;
            float progress = maxTime;
            while (progress > 0)
            {
                progress -= Mathf.Clamp(Time.unscaledDeltaTime, 0, 0.04f);
                progress = Mathf.Clamp(progress, 0, maxTime);
                content.localScale = Vector3.one * scaleCurve.Evaluate(progress > scaleEndTime ? scaleEndTime : progress);
                _CanvasGroup.alpha = alphaCurve.Evaluate(progress > alphaEndTime ? alphaEndTime : progress);
                yield return null;
            }
            _CanvasGroup.blocksRaycasts = false;
        }
    }
}
